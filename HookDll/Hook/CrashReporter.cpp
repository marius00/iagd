#include "stdafx.h"
#include "CrashReporter.h"
#include "Logger.h"

#include <dbghelp.h>
#include <stdio.h>
#include <string>

std::wstring GetIagdFolder();

namespace {

	// ---------------------------------------------------------------------------------------------------
	// State resolved during Install(), so the handler itself needs no allocation and no folder lookup.
	// ---------------------------------------------------------------------------------------------------

	PVOID   g_handler = nullptr;
	wchar_t g_outputFolder[MAX_PATH] = { 0 };
	LONG    g_reportsWritten = 0;

	/// Enough to see the shape of a crash without turning a fault into a log flood.
	const LONG MAX_REPORTS_PER_PROCESS = 3;

	// ---------------------------------------------------------------------------------------------------
	// Breadcrumbs
	//
	// A stack alone cannot distinguish "the hook faulted" from "the hook corrupted something and the game
	// faulted later somewhere unrelated" -- in the second case there is no IA frame on the faulting stack at
	// all. The ring records what the hook was doing, per thread, so the report can answer that.
	// ---------------------------------------------------------------------------------------------------

	struct Breadcrumb {
		ULONGLONG   tick;
		DWORD       threadId;
		const char* tag;
		uint64_t    a;
		uint64_t    b;
	};

	const unsigned BREADCRUMB_COUNT = 128; // Power of two, so the index wraps with a mask.
	Breadcrumb     g_breadcrumbs[BREADCRUMB_COUNT] = { 0 };
	volatile LONG  g_breadcrumbCursor = 0;

	// ---------------------------------------------------------------------------------------------------
	// Formatting helpers. Fixed buffer, no std::string, no streams -- see the header for why.
	// ---------------------------------------------------------------------------------------------------

	struct TextBuffer {
		char   data[64 * 1024];
		size_t used;

		TextBuffer() : used(0) { data[0] = '\0'; }

		void AppendFormat(const char* format, ...) {
			if (used >= sizeof(data) - 1) {
				return;
			}

			va_list args;
			va_start(args, format);
			const int written = _vsnprintf_s(data + used, sizeof(data) - used, _TRUNCATE, format, args);
			va_end(args);

			if (written > 0) {
				used += written;
			}
		}
	};

	/// Maps an address back to "module+offset", which is all that is needed to symbolise against the PDB we
	/// ship alongside the DLL. Returns false when the address belongs to no loaded module, which is itself
	/// worth printing -- a return address pointing into free memory is the signature of a stale hook.
	bool DescribeAddress(void* address, char* moduleName, size_t moduleNameSize, DWORD64& offset) {
		HMODULE module = NULL;
		if (!GetModuleHandleExW(
			GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
			(LPCWSTR)address,
			&module) || module == NULL) {
			return false;
		}

		wchar_t fullPath[MAX_PATH] = { 0 };
		if (GetModuleFileNameW(module, fullPath, MAX_PATH) == 0) {
			return false;
		}

		const wchar_t* leaf = wcsrchr(fullPath, L'\\');
		leaf = (leaf != nullptr) ? leaf + 1 : fullPath;

		WideCharToMultiByte(CP_UTF8, 0, leaf, -1, moduleName, (int)moduleNameSize, NULL, NULL);
		offset = (DWORD64)address - (DWORD64)module;
		return true;
	}

	void AppendAddress(TextBuffer& out, const char* label, void* address) {
		char     moduleName[MAX_PATH] = { 0 };
		DWORD64  offset = 0;

		if (DescribeAddress(address, moduleName, sizeof(moduleName), offset)) {
			out.AppendFormat("%s0x%016llX  %s+0x%llX\n", label, (DWORD64)address, moduleName, offset);
		}
		else {
			out.AppendFormat("%s0x%016llX  <no module -- unmapped or freed>\n", label, (DWORD64)address);
		}
	}

	const char* ExceptionName(DWORD code) {
		switch (code) {
		case EXCEPTION_ACCESS_VIOLATION:      return "ACCESS_VIOLATION";
		case EXCEPTION_ARRAY_BOUNDS_EXCEEDED: return "ARRAY_BOUNDS_EXCEEDED";
		case EXCEPTION_DATATYPE_MISALIGNMENT: return "DATATYPE_MISALIGNMENT";
		case EXCEPTION_ILLEGAL_INSTRUCTION:   return "ILLEGAL_INSTRUCTION";
		case EXCEPTION_IN_PAGE_ERROR:         return "IN_PAGE_ERROR";
		case EXCEPTION_PRIV_INSTRUCTION:      return "PRIV_INSTRUCTION";
		case EXCEPTION_STACK_OVERFLOW:        return "STACK_OVERFLOW";
		case 0xC0000374:                      return "HEAP_CORRUPTION";
		case 0xC0000409:                      return "STACK_BUFFER_OVERRUN";
		default:                              return "UNKNOWN";
		}
	}

	/// Only genuinely fatal faults. The game raises and handles its own structured exceptions during normal
	/// play, and C++ throws (0xE06D7363) pass through here on every caught exception in the process -- both
	/// this DLL's and the game's. Reporting those would bury the one that matters.
	bool IsFatal(DWORD code) {
		switch (code) {
		case EXCEPTION_ACCESS_VIOLATION:
		case EXCEPTION_ARRAY_BOUNDS_EXCEEDED:
		case EXCEPTION_DATATYPE_MISALIGNMENT:
		case EXCEPTION_ILLEGAL_INSTRUCTION:
		case EXCEPTION_IN_PAGE_ERROR:
		case EXCEPTION_PRIV_INSTRUCTION:
		case EXCEPTION_STACK_OVERFLOW:
		case 0xC0000374:
		case 0xC0000409:
			return true;
		default:
			return false;
		}
	}

	void AppendTimestamp(TextBuffer& out) {
		SYSTEMTIME now;
		GetLocalTime(&now);
		out.AppendFormat("%04u-%02u-%02u %02u:%02u:%02u.%03u",
			now.wYear, now.wMonth, now.wDay, now.wHour, now.wMinute, now.wSecond, now.wMilliseconds);
	}

	void AppendModuleBases(TextBuffer& out) {
		out.AppendFormat("\n--- module bases (for symbolising the offsets above) ---\n");

		const wchar_t* interesting[] = {
			L"ItemAssistantHook_x64.dll",
			L"Game.dll",
			L"Engine.dll",
			L"Grim Dawn.exe",
		};

		for (const wchar_t* name : interesting) {
			HMODULE module = GetModuleHandleW(name);
			char narrow[MAX_PATH] = { 0 };
			WideCharToMultiByte(CP_UTF8, 0, name, -1, narrow, sizeof(narrow), NULL, NULL);

			if (module != NULL) {
				out.AppendFormat("  %-28s 0x%016llX\n", narrow, (DWORD64)module);
			}
			else {
				out.AppendFormat("  %-28s <not loaded>\n", narrow);
			}
		}
	}

	void AppendBreadcrumbs(TextBuffer& out, DWORD faultingThread) {
		const ULONGLONG now = GetTickCount64();
		const LONG      cursor = g_breadcrumbCursor;

		out.AppendFormat("\n--- hook activity leading up to the fault (newest first) ---\n");
		out.AppendFormat("  faulting thread = %lu\n", faultingThread);

		bool any = false;
		for (unsigned i = 1; i <= BREADCRUMB_COUNT; i++) {
			const Breadcrumb& crumb = g_breadcrumbs[(cursor - (LONG)i) & (BREADCRUMB_COUNT - 1)];
			if (crumb.tag == nullptr) {
				continue;
			}

			any = true;
			out.AppendFormat("  -%6llums  tid=%-6lu %s%-34s a=0x%llX b=0x%llX\n",
				now - crumb.tick,
				crumb.threadId,
				crumb.threadId == faultingThread ? "* " : "  ",
				crumb.tag,
				crumb.a,
				crumb.b);
		}

		if (!any) {
			out.AppendFormat("  (none -- the hook had not run since injection)\n");
		}
	}

	void AppendStack(TextBuffer& out) {
		out.AppendFormat("\n--- stack (captured from the faulting thread) ---\n");

		void*  frames[62] = { 0 };
		const USHORT captured = RtlCaptureStackBackTrace(0, ARRAYSIZE(frames), frames, NULL);

		if (captured == 0) {
			out.AppendFormat("  (no frames captured)\n");
			return;
		}

		for (USHORT i = 0; i < captured; i++) {
			char label[32] = { 0 };
			_snprintf_s(label, sizeof(label), _TRUNCATE, "  [%02u] ", i);
			AppendAddress(out, label, frames[i]);
		}
	}

	/// Freezes a copy of the hook log next to the crash report.
	///
	/// The live log rotates once it gets large, and a player who crashes restarts and keeps playing rather than
	/// preserving a file. Copying it here gives the report, the dump and the log that explains them one shared
	/// name prefix, so "send me the crash_* files" is the whole instruction.
	///
	/// Raw handles with full sharing, because our own std::wofstream still has the file open. Every log line
	/// ends with std::endl, which flushes, so what is on disk is current up to the last line written.
	void SnapshotLog(const wchar_t* sourcePath, const wchar_t* destPath) {
		HANDLE source = CreateFileW(sourcePath, GENERIC_READ,
			FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
			NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		if (source == INVALID_HANDLE_VALUE) {
			return;
		}

		HANDLE dest = CreateFileW(destPath, GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		if (dest == INVALID_HANDLE_VALUE) {
			CloseHandle(source);
			return;
		}

		// Static rather than stack: this runs on a thread that has already faulted, and the remaining stack
		// is not worth spending on a copy buffer.
		static char buffer[64 * 1024];
		for (;;) {
			DWORD read = 0;
			if (!ReadFile(source, buffer, sizeof(buffer), &read, NULL) || read == 0) {
				break;
			}

			DWORD written = 0;
			if (!WriteFile(dest, buffer, read, &written, NULL)) {
				break;
			}
		}

		FlushFileBuffers(dest);
		CloseHandle(dest);
		CloseHandle(source);
	}

	void WriteFileRaw(const wchar_t* path, const void* data, DWORD length) {
		HANDLE file = CreateFileW(path, GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		if (file == INVALID_HANDLE_VALUE) {
			return;
		}

		DWORD written = 0;
		WriteFile(file, data, length, &written, NULL);
		FlushFileBuffers(file);
		CloseHandle(file);
	}

	/// A real minidump, so a crash can be opened in WinDbg or Visual Studio with full stacks for every thread.
	/// dbghelp is loaded on demand: linking against it would add a hard dependency to a DLL whose whole job is
	/// to load quietly into someone else's process.
	void WriteMinidump(const wchar_t* path, EXCEPTION_POINTERS* exception) {
		typedef BOOL(WINAPI *MiniDumpWriteDumpPtr)(
			HANDLE, DWORD, HANDLE, MINIDUMP_TYPE,
			PMINIDUMP_EXCEPTION_INFORMATION, PMINIDUMP_USER_STREAM_INFORMATION, PMINIDUMP_CALLBACK_INFORMATION);

		HMODULE dbghelp = LoadLibraryW(L"dbghelp.dll");
		if (dbghelp == NULL) {
			return;
		}

		MiniDumpWriteDumpPtr writeDump = (MiniDumpWriteDumpPtr)GetProcAddress(dbghelp, "MiniDumpWriteDump");
		if (writeDump == NULL) {
			FreeLibrary(dbghelp);
			return;
		}

		HANDLE file = CreateFileW(path, GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		if (file != INVALID_HANDLE_VALUE) {
			MINIDUMP_EXCEPTION_INFORMATION info;
			info.ThreadId = GetCurrentThreadId();
			info.ExceptionPointers = exception;
			info.ClientPointers = FALSE;

			// WithIndirectlyReferencedMemory pulls in the memory the registers and stacks point at, which is
			// what makes it possible to inspect the GameEngine/GameInfo/Item objects the hook was touching.
			const MINIDUMP_TYPE type = (MINIDUMP_TYPE)(
				MiniDumpWithThreadInfo |
				MiniDumpWithIndirectlyReferencedMemory |
				MiniDumpWithUnloadedModules);

			writeDump(GetCurrentProcess(), GetCurrentProcessId(), file, type, &info, NULL, NULL);
			FlushFileBuffers(file);
			CloseHandle(file);
		}

		FreeLibrary(dbghelp);
	}

	void BuildReport(TextBuffer& out, EXCEPTION_POINTERS* exception) {
		const EXCEPTION_RECORD* record = exception->ExceptionRecord;
		const CONTEXT*          context = exception->ContextRecord;
		const DWORD             threadId = GetCurrentThreadId();

		out.AppendFormat("Item Assistant hook -- crash report\n");
		out.AppendFormat("===================================\n");
		out.AppendFormat("time         : ");
		AppendTimestamp(out);
		out.AppendFormat("\n");
		out.AppendFormat("hook built   : %s %s\n", __DATE__, __TIME__);
		out.AppendFormat("process      : %lu\n", GetCurrentProcessId());
		out.AppendFormat("thread       : %lu\n", threadId);
		out.AppendFormat("exception    : 0x%08lX (%s)\n", record->ExceptionCode, ExceptionName(record->ExceptionCode));

		AppendAddress(out, "fault at     : ", record->ExceptionAddress);

		if (record->ExceptionCode == EXCEPTION_ACCESS_VIOLATION && record->NumberParameters >= 2) {
			const ULONG_PTR operation = record->ExceptionInformation[0];
			const char*     verb = (operation == 0) ? "reading" : (operation == 1) ? "writing" : "executing";
			out.AppendFormat("             : %s address 0x%016llX\n", verb, (DWORD64)record->ExceptionInformation[1]);
		}

		out.AppendFormat("\n--- registers ---\n");
		out.AppendFormat("  RIP=0x%016llX RSP=0x%016llX RBP=0x%016llX\n", context->Rip, context->Rsp, context->Rbp);
		out.AppendFormat("  RAX=0x%016llX RBX=0x%016llX RCX=0x%016llX RDX=0x%016llX\n",
			context->Rax, context->Rbx, context->Rcx, context->Rdx);
		out.AppendFormat("  RSI=0x%016llX RDI=0x%016llX R8 =0x%016llX R9 =0x%016llX\n",
			context->Rsi, context->Rdi, context->R8, context->R9);
		out.AppendFormat("  R10=0x%016llX R11=0x%016llX R12=0x%016llX R13=0x%016llX\n",
			context->R10, context->R11, context->R12, context->R13);
		out.AppendFormat("  R14=0x%016llX R15=0x%016llX\n", context->R14, context->R15);

		AppendStack(out);
		AppendModuleBases(out);
		AppendBreadcrumbs(out, threadId);

		out.AppendFormat("\n--- end of report ---\n");
	}

	LONG CALLBACK OnException(EXCEPTION_POINTERS* exception) {
		if (exception == nullptr || exception->ExceptionRecord == nullptr || exception->ContextRecord == nullptr) {
			return EXCEPTION_CONTINUE_SEARCH;
		}

		if (!IsFatal(exception->ExceptionRecord->ExceptionCode)) {
			return EXCEPTION_CONTINUE_SEARCH;
		}

		if (g_outputFolder[0] == L'\0') {
			return EXCEPTION_CONTINUE_SEARCH;
		}

		const LONG index = InterlockedIncrement(&g_reportsWritten);
		if (index > MAX_REPORTS_PER_PROCESS) {
			return EXCEPTION_CONTINUE_SEARCH;
		}

		// One timestamped stem shared by the report, the dump and the log snapshot. Timestamped rather than
		// numbered because pids are recycled, and a collision would overwrite an earlier crash.
		SYSTEMTIME now;
		GetLocalTime(&now);

		wchar_t stem[MAX_PATH] = { 0 };
		_snwprintf_s(stem, MAX_PATH, _TRUNCATE, L"%scrash_%04u%02u%02u_%02u%02u%02u_%lu_%ld",
			g_outputFolder,
			now.wYear, now.wMonth, now.wDay, now.wHour, now.wMinute, now.wSecond,
			GetCurrentProcessId(), index);

		wchar_t path[MAX_PATH] = { 0 };

		_snwprintf_s(path, MAX_PATH, _TRUNCATE, L"%s.dmp", stem);
		WriteMinidump(path, exception);

		// A stack overflow leaves no room to build a 64KB report on this stack, and trying turns a
		// diagnosable fault into a second one. The minidump does not need our stack, so take just that.
		const bool stackExhausted = exception->ExceptionRecord->ExceptionCode == EXCEPTION_STACK_OVERFLOW;
		if (!stackExhausted) {
			static TextBuffer report; // Static: 64KB is more than this thread's remaining stack is worth risking.
			report.used = 0;
			report.data[0] = '\0';

			BuildReport(report, exception);

			_snwprintf_s(path, MAX_PATH, _TRUNCATE, L"%s.txt", stem);
			WriteFileRaw(path, report.data, (DWORD)report.used);
		}

		// Only alongside the first report. Later faults in the same process are almost always fallout from the
		// first, and three copies of a multi-megabyte log helps nobody.
		if (index == 1) {
			wchar_t livePath[MAX_PATH] = { 0 };
			_snwprintf_s(livePath, MAX_PATH, _TRUNCATE, L"%siagd_hook.log", g_outputFolder);

			_snwprintf_s(path, MAX_PATH, _TRUNCATE, L"%s.log", stem);
			SnapshotLog(livePath, path);
		}

		// Always continue searching. This is a diagnostic, not a recovery mechanism: swallowing the fault
		// would leave the game running on corrupted state, which is the very failure mode that made this
		// crash so hard to pin down in the first place.
		return EXCEPTION_CONTINUE_SEARCH;
	}

} // namespace

namespace CrashReporter {

	void Install() {
		if (g_handler != nullptr) {
			return;
		}

		const std::wstring folder = GetIagdFolder();
		if (folder.empty() || folder.size() >= MAX_PATH) {
			LogToFile(LogLevel::WARNING, L"Crash reporter: could not resolve the IAGD folder, crash reports are disabled.");
			return;
		}

		wcscpy_s(g_outputFolder, MAX_PATH, folder.c_str());

		// First in the chain: the game installs handlers of its own, and a report is worthless if something
		// else has already decided the fault is none of our business.
		g_handler = AddVectoredExceptionHandler(1, OnException);
		if (g_handler == nullptr) {
			LogToFile(LogLevel::WARNING, L"Crash reporter: AddVectoredExceptionHandler failed, crash reports are disabled.");
			return;
		}

		LogToFile(LogLevel::INFO, L"Crash reporter installed, reports will be written to " + folder);
	}

	void Uninstall() {
		if (g_handler == nullptr) {
			return;
		}

		RemoveVectoredExceptionHandler(g_handler);
		g_handler = nullptr;
	}

	void Note(const char* tag, uint64_t a, uint64_t b) {
		const LONG slot = InterlockedIncrement(&g_breadcrumbCursor) - 1;
		Breadcrumb& crumb = g_breadcrumbs[slot & (BREADCRUMB_COUNT - 1)];

		// Torn reads are possible if the ring wraps while the report is being built. That is acceptable: a
		// garbled line in the tail costs nothing, and locking here would put a mutex on the game's hot path.
		crumb.tick = GetTickCount64();
		crumb.threadId = GetCurrentThreadId();
		crumb.a = a;
		crumb.b = b;
		crumb.tag = tag; // Written last, so a reader never sees a tag with another crumb's payload.
	}

}
