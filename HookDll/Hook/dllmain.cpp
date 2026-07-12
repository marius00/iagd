#include "stdafx.h"
#include <chrono>
#include <codecvt> // wstring_convert
#include <windows.h>
#include <stdlib.h>
#include <objbase.h>
#include <fstream>
#include "DataQueue.h"
#include "MessageType.h"
#include "InventorySack_AddItem.h"
#include "Exports.h"
#include "OnDemandSeedInfo.h"
#include "GameEngineUpdate.h"
#include "HookLog.h"
#include "Logger.h"
#include "SetHardcore.h"
#include "SettingsReader.h"
HookLog g_log;

#pragma region Variables
// Switches hook logging on/off
#if 1
#define LOG(streamdef) \
{ \
    std::wstring msg = (((std::wostringstream&)(std::wostringstream().flush() << streamdef)).str()); \
	g_log.out(logStartupTime() + msg); \
    msg += _T("\n"); \
    OutputDebugString(msg.c_str()); \
}
#else
#define LOG(streamdef) \
    __noop;
#endif




DWORD g_lastThreadTick = 0;
HANDLE g_hEvent;
HANDLE g_thread;

DataQueue g_dataQueue;
InventorySack_AddItem* g_InventorySack_AddItemInstance = NULL;

HWND g_targetWnd = NULL;

bool g_isRunningInWine = false;
std::wstring g_linuxHackFolder;

#pragma endregion

#pragma region CORE


std::wstring logStartupTime() {
	__time64_t rawtime;
	struct tm timeinfo;
	wchar_t buffer[80];

	_time64(&rawtime);
	localtime_s(&timeinfo, &rawtime);

	wcsftime(buffer, sizeof(buffer), L"%Y-%m-%d %H:%M:%S ", &timeinfo);
	std::wstring str(buffer);

	return str;
}

std::string logStartupTimeChar() {
	__time64_t rawtime;
	struct tm timeinfo;
	char buffer[80];

	_time64(&rawtime);
	localtime_s(&timeinfo, &rawtime);

	strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S ", &timeinfo);
	std::string str(buffer);

	return str;
}

std::wstring LogLevelToString(LogLevel level) {
	switch (level) {
	case LogLevel::INFO:
		return L"INFO ";

	case LogLevel::WARNING:
		return L"WARN ";

	case LogLevel::FATAL:
		return L"ERROR ";
	}

	return L"UNKNOWN";
}
std::string LogLevelToStringA(LogLevel level) {
	switch (level) {
	case LogLevel::INFO:
		return "INFO ";

	case LogLevel::WARNING:
		return "WARN ";

	case LogLevel::FATAL:
		return "ERROR ";
	}

	return "UNKNOWN";
}
void LogToFile(LogLevel level, const wchar_t* message) {
	g_log.out(logStartupTime() + LogLevelToString(level) + message);
}
void LogToFile(LogLevel level, const char* message) {
	g_log.out((logStartupTimeChar() + LogLevelToStringA(level).c_str() + std::string(message)).c_str());
}
void LogToFile(LogLevel level, const std::string message) {
	g_log.out((logStartupTimeChar() + LogLevelToStringA(level) + message).c_str());
}
void LogToFile(LogLevel level, std::wstring message) {
	g_log.out(logStartupTime() + LogLevelToString(level) + message);
}
void LogToFile(LogLevel level, std::wstringstream message) {
	g_log.out(logStartupTime() + LogLevelToString(level) + message.str());
}


// Wine/Proton file-based IPC: write message as binary file to linuxhack folder
// Format: [int32 type][int32 dataLength][raw data bytes]
// Write as .tmp then rename to .msg for atomic visibility
void WriteMessageToFile(DWORD dwData, void* lpData, DWORD cbData) {
	GUID guid;
	if (CoCreateGuid(&guid) != S_OK) {
		LogToFile(LogLevel::FATAL, L"Failed to create GUID for message file");
		return;
	}

	wchar_t guidStr[64];
	swprintf_s(guidStr, L"%08lX-%04X-%04X-%02X%02X-%02X%02X%02X%02X%02X%02X",
		guid.Data1, guid.Data2, guid.Data3,
		guid.Data4[0], guid.Data4[1], guid.Data4[2], guid.Data4[3],
		guid.Data4[4], guid.Data4[5], guid.Data4[6], guid.Data4[7]);

	std::wstring tmpPath = g_linuxHackFolder + guidStr + L".tmp";
	std::wstring msgPath = g_linuxHackFolder + guidStr + L".msg";

	HANDLE hFile = CreateFile(tmpPath.c_str(), GENERIC_WRITE, 0, NULL, CREATE_NEW, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE) {
		LogToFile(LogLevel::FATAL, L"Failed to create temp message file: " + tmpPath);
		return;
	}

	DWORD written;
	int32_t type = static_cast<int32_t>(dwData);
	int32_t dataLength = static_cast<int32_t>(cbData);

	WriteFile(hFile, &type, sizeof(type), &written, NULL);
	WriteFile(hFile, &dataLength, sizeof(dataLength), &written, NULL);
	if (cbData > 0 && lpData != nullptr) {
		WriteFile(hFile, lpData, cbData, &written, NULL);
	}
	CloseHandle(hFile);

	if (!MoveFile(tmpPath.c_str(), msgPath.c_str())) {
		LogToFile(LogLevel::FATAL, L"Failed to move message file to: " + msgPath);
		DeleteFile(tmpPath.c_str());
	}
}


/// Thread function that dispatches queued message blocks to the IA application.
void WorkerThreadMethod() {
	try {
		bool wineSetActiveDone = false;
		while ((g_hEvent != NULL) && (WaitForSingleObject(g_hEvent, INFINITE) == WAIT_OBJECT_0)) {
			if (g_hEvent == NULL) {
				break;
			}

			if (g_isRunningInWine) {
				// In Wine mode, keep InventorySack_AddItem permanently active (no FindWindow)
				if (!wineSetActiveDone && g_InventorySack_AddItemInstance != NULL) {
					g_InventorySack_AddItemInstance->SetActive(true);
					wineSetActiveDone = true;
				}
			}
			else {
				DWORD tick = GetTickCount();
				if (tick < g_lastThreadTick) {
					// Overflow
					g_lastThreadTick = tick;
				}

				if ((tick - g_lastThreadTick > 1000) || (g_targetWnd == NULL)) {
					g_targetWnd = FindWindow(L"GDIAWindowClass", NULL);
					g_lastThreadTick = GetTickCount();

					if (g_InventorySack_AddItemInstance != NULL) {
						g_InventorySack_AddItemInstance->SetActive(g_targetWnd != NULL);
					}
				}
			}

			while (!g_dataQueue.empty()) {
				DataItemPtr item = g_dataQueue.pop();

				if (g_isRunningInWine) {
					WriteMessageToFile(item->type(), item->data(), item->size());
				}
				else {
					if (g_targetWnd == NULL) {
						// We have data, but no target window, so just delete the message
						continue;
					}

					COPYDATASTRUCT data;
					data.dwData = item->type();
					data.lpData = item->data();
					data.cbData = item->size();

					SendMessage(g_targetWnd, WM_COPYDATA, 0, (LPARAM)&data);
					auto lastErrorCode = GetLastError();
					if (lastErrorCode != 0)
						LOG(L"After SendMessage error code is " << lastErrorCode);
				}
			}

		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"ERROR In the worker thread.." + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"ERROR In the worker thread.. (triple-dot)");
	}
}

OnDemandSeedInfo* listener = nullptr;
unsigned __stdcall WorkerThreadMethodWrap(void* argss) {


	LogToFile(LogLevel::INFO, L"Initiating class for seed info..");

	if (listener != nullptr) {
		listener->Start();
	}
	WorkerThreadMethod();
	return 0;
}

void StartWorkerThread() {
	LogToFile(LogLevel::INFO, L"Starting worker thread..");
	unsigned int pid;
	g_thread = (HANDLE)_beginthreadex(NULL, 0, &WorkerThreadMethodWrap, NULL, 0, &pid);


	DataItemPtr item(new DataItem(TYPE_REPORT_WORKER_THREAD_LAUNCHED, 0, NULL));
	g_dataQueue.push(item);
	SetEvent(g_hEvent);
	LogToFile(LogLevel::INFO, L"Started worker thread..");
}


void EndWorkerThread() {
	LogToFile(LogLevel::INFO, L"Ending worker thread..");
	if (g_hEvent != NULL) {
		SetEvent(g_hEvent);
		HANDLE h = g_hEvent;

		g_hEvent = NULL;
		Sleep(1500); // The worker thread might have just read from g_hEvent, seen that it is not NULL, then sent it in to WaitForSingleObject right after we close it.		
		CloseHandle(h);

		//WaitForSingleObject(g_thread, INFINITE);
		CloseHandle(g_thread);
	}
}

#pragma endregion

static void ConfigureInstalootHooks(std::vector<BaseMethodHook*>& hooks) {
	try {
		LogToFile(LogLevel::INFO, L"Configuring instaloot hook..");
		g_InventorySack_AddItemInstance = new InventorySack_AddItem(&g_dataQueue, g_hEvent);
		hooks.push_back(g_InventorySack_AddItemInstance); // Includes GetPrivateStash internally

		// In Wine mode, set active immediately since there's no FindWindow
		if (g_isRunningInWine && g_InventorySack_AddItemInstance != NULL) {
			g_InventorySack_AddItemInstance->SetActive(true);
		}
	}
	catch (std::exception& ex) {
		// For now just let it be. Known issue inside InventorySack_AddItem

		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());

		LogToFile(LogLevel::FATAL, L"ERROR Configuring instaloot hook.." + wide);

	}
	catch (...) {
		// For now just let it be. Known issue inside InventorySack_AddItem
		LogToFile(LogLevel::FATAL, L"ERROR Configuring instaloot hook.. (triple-dot)");
	}

	LogToFile(LogLevel::INFO, L"Configuring hc detection hook..");
	hooks.push_back(new SetHardcore(&g_dataQueue, g_hEvent));
}
/*
bool GetProductAndVersion()
{
	// get the filename of the executable containing the version resource
	TCHAR szFilename[MAX_PATH + 1] = { 0 };
	if (GetModuleFileName(NULL, szFilename, MAX_PATH) == 0)
	{
		LogToFile("GetModuleFileName failed with error");
		return false;
	}

	// allocate a block of memory for the version info
	DWORD dummy;
	DWORD dwSize = GetFileVersionInfoSize(szFilename, &dummy);
	if (dwSize == 0)
	{
		LogToFile(L"GetFileVersionInfoSize failed with error");
		return false;
	}
	std::vector<BYTE> data(dwSize);

	// load the version info
	if (!GetFileVersionInfo(szFilename, NULL, dwSize, &data[0]))
	{
		LogToFile("GetFileVersionInfo failed with error");
		return false;
	}

	// get the name and version strings
	LPVOID pvProductName = NULL;
	unsigned int iProductNameLen = 0;
	LPVOID pvProductVersion = NULL;
	unsigned int iProductVersionLen = 0;

	// replace "040904e4" with the language ID of your resources
	if (!VerQueryValue(&data[0], _T("\\StringFileInfo\\040904e4\\ProductName"), &pvProductName, &iProductNameLen) ||
		!VerQueryValue(&data[0], _T("\\StringFileInfo\\040904e4\\ProductVersion"), &pvProductVersion, &iProductVersionLen))
	{
		LogToFile("Can't obtain ProductName and ProductVersion from resources");
		return false;
	}

	LogToFile((wchar_t*)pvProductVersion);

	//strProductName.SetString((LPCSTR)pvProductName, iProductNameLen);
	//strProductVersion.SetString((LPCSTR)pvProductVersion, iProductVersionLen);

	return true;
}
*/

void ReportCancelledInjection() {
	if (g_isRunningInWine) {
		WriteMessageToFile(TYPE_INJECTION_CANCELLED, nullptr, 0);
		return;
	}

	auto hwnd = FindWindow(L"GDIAWindowClass", NULL);
	if (hwnd == nullptr) {
		return;
	}

	COPYDATASTRUCT data;
	data.dwData = TYPE_INJECTION_CANCELLED;
	data.lpData = nullptr;
	data.cbData = 0;

	// To avoid blocking the main thread, we should not have a lock on the queue while we process the message.
	SendMessage(hwnd, WM_COPYDATA, 0, (LPARAM)&data);
	auto lastErrorCode = GetLastError();
	if (lastErrorCode != 0)
		LOG(L"After SendMessage error code is " << lastErrorCode);
}

std::vector<BaseMethodHook*> hooks;
std::wstring GetIagdFolder();
int ProcessAttach(HINSTANCE _hModule) {
	//GetProductAndVersion();
	LogToFile(LogLevel::INFO, std::string("DLL Compiled: ") + std::string(__DATE__) + std::string(" ") + std::string(__TIME__));
	LogToFile(LogLevel::INFO, L"Attatching to process..");

	// Check if running in Wine/Proton
	try {
		SettingsReader settingsReader;
		g_isRunningInWine = settingsReader.GetIsRunningInWine();
	}
	catch (...) {
		LogToFile(LogLevel::WARNING, L"Failed to read Wine setting, defaulting to false");
		g_isRunningInWine = false;
	}

	if (g_isRunningInWine) {
		g_linuxHackFolder = GetIagdFolder() + L"linuxhack\\";
		CreateDirectory(g_linuxHackFolder.c_str(), NULL);
		LogToFile(LogLevel::INFO, L"Wine mode enabled, linuxhack folder: " + g_linuxHackFolder);

		// Write PID file to signal successful injection
		DWORD pid = GetCurrentProcessId();
		std::wstring pidFile = g_linuxHackFolder + std::to_wstring(pid) + L".PID";
		HANDLE hPid = CreateFile(pidFile.c_str(), GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		if (hPid != INVALID_HANDLE_VALUE) {
			CloseHandle(hPid);
			LogToFile(LogLevel::INFO, L"Wrote PID file: " + pidFile);
		}
		else {
			LogToFile(LogLevel::WARNING, L"Failed to write PID file: " + pidFile);
		}
	}


	GAME::GameEngine* gameEngine = fnGetGameEngine();
	if (gameEngine == nullptr) {
		ReportCancelledInjection();
		LogToFile(LogLevel::INFO, L"Could not find game engine ptr, aborting DLL injection..");
		return FALSE;
	}
	if (IsGameLoading(gameEngine)) {
		ReportCancelledInjection();
		LogToFile(LogLevel::INFO, L"Game is still loading, aborting DLL injection..");
		return FALSE;
	}
	else {
		LogToFile(LogLevel::INFO, L"Game is not loading..");
	}

	if (IsGameWaiting(gameEngine, true)) { // TODO: When on a PC with IDA installed, figure out what the boolean is.
		ReportCancelledInjection();
		LogToFile(LogLevel::INFO, L"Game is waiting, aborting DLL injection.. [true]");
		return FALSE;
	}
	else {
		LogToFile(LogLevel::INFO, L"Game is not waiting.. [true]");
	}

	if (IsGameWaiting(gameEngine, false)) { // TODO: When on a PC with IDA installed, figure out what the boolean is.
		ReportCancelledInjection();
		LogToFile(LogLevel::INFO, L"Game is waiting, aborting DLL injection.. [false]");
		return FALSE;
	}
	else {
		LogToFile(LogLevel::INFO, L"Game is not waiting.. [false]");
	}

	if (!IsGameEngineOnline(gameEngine)) {
		ReportCancelledInjection();
		LogToFile(LogLevel::INFO, L"Game engine is not yet online, aborting DLL injection..");
		return FALSE;
	}
	else {
		LogToFile(LogLevel::INFO, L"Game engine is online..");
	}

	LogToFile(LogLevel::INFO, L"Game is most likely running, proceeding with injection.");


	g_hEvent = CreateEvent(NULL, FALSE, FALSE, L"IA_Worker");

	LogToFile(LogLevel::INFO, L"DLL for GD 1.2");



	LogToFile(LogLevel::INFO, L"Preparing hooks..");
	ConfigureInstalootHooks(hooks);

	LogToFile(LogLevel::INFO, L"Preparing replica hooks..");

	LogToFile(LogLevel::INFO, L"Creating seed info container class..");
	listener = new OnDemandSeedInfo(&g_dataQueue, g_hEvent);
	if (listener != nullptr) {
		hooks.push_back(listener);
	}
	// hooks.push_back(new GameEngineUpdate(&g_dataQueue, g_hEvent));	 // Debug/test only

	LogToFile(LogLevel::INFO, L"Starting hook enabling.. " + std::to_wstring(hooks.size()) + L" hooks.");
	for (unsigned int i = 0; i < hooks.size(); i++) {
		LogToFile(LogLevel::INFO, L"Enabling hook..");
		hooks[i]->EnableHook();
	}
	LogToFile(LogLevel::INFO, L"Hooking complete..");


	StartWorkerThread();
	LogToFile(LogLevel::INFO, L"Initialization complete..");

	g_log.setInitialized(true);
	return TRUE;
}


#pragma region Attach_Detatch
int ProcessDetach(HINSTANCE _hModule) {
	// Signal that we are shutting down
	// This message is not at all guaranteed to get sent.

	LOG(L"Detatching DLL..");
	OutputDebugString(L"ProcessDetach");


	for (unsigned int i = 0; i < hooks.size(); i++) {
		hooks[i]->DisableHook();
		delete hooks[i];
	}
	hooks.clear();

	if (listener != nullptr) {
		listener->Stop();
		delete listener;
		listener = nullptr;
	}

	EndWorkerThread();

	// Best-effort cleanup of PID file in Wine mode
	if (g_isRunningInWine && !g_linuxHackFolder.empty()) {
		DWORD pid = GetCurrentProcessId();
		std::wstring pidFile = g_linuxHackFolder + std::to_wstring(pid) + L".PID";
		DeleteFile(pidFile.c_str());
	}

	LOG(L"DLL detached..");
	return TRUE;
}


BOOL APIENTRY DllMain(HINSTANCE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		return ProcessAttach(hModule);

	case DLL_PROCESS_DETACH:
		return ProcessDetach(hModule);
	}
	return TRUE;
}
#pragma endregion


