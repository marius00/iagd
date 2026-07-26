
#include "BaseMethodHook.h"
#include "MessageType.h"
#include "GrimTypes.h"
#include <detours.h>

BaseMethodHook::BaseMethodHook() = default;
BaseMethodHook::BaseMethodHook(DataQueue* dataQueue, HANDLE hEvent) {}
void BaseMethodHook::EnableHook() {}
void BaseMethodHook::DisableHook() {}

void BaseMethodHook::ReportHookError(DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_GENERIC, sizeof(id), (char*)&id));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);
}

void BaseMethodHook::ReportHookSuccess(DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	DataItemPtr item(new DataItem(TYPE_SUCCESS_HOOKING_GENERIC, sizeof(id), (char*)&id));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);
}

void BaseMethodHook::TransferData(unsigned int size, const char* data) {
	DataItemPtr item(new DataItem(m_messageId, size, data));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);
}

void* BaseMethodHook::HookDll(const wchar_t* dll, char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	void* originalMethod = GetProcAddressOrLogToFile(dll, procAddress);
	m_messageId = id;
	if (originalMethod == NULL) {
		// Export missing -- almost always a game patch that changed a signature, which
		// changes the mangled name. Bail out instead of falling through to DetourAttach
		// on a null target: that leaves the caller holding a null "original method"
		// pointer which it will happily call through if the detour ever does install.
		ReportHookError(m_dataQueue, m_hEvent, id);
		return NULL;
	}

	ReportHookSuccess(m_dataQueue, m_hEvent, id);

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();


	return originalMethod;
}

void* BaseMethodHook::HookGame(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	return HookDll(L"Game.dll", procAddress, HookedMethod, m_dataQueue, m_hEvent, id);
}

void* BaseMethodHook::HookEngine(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	return HookDll(L"Engine.dll", procAddress, HookedMethod, m_dataQueue, m_hEvent, id);
}

// "originalMethod" must be the address of the caller's trampoline pointer, not the pointer
// itself: Detours reads *originalMethod to find the trampoline and writes the restored
// target back into it. Taking the parameter by value and detaching &parameter (as this used
// to) hands Detours the address of a local, so every detach failed with ERROR_INVALID_BLOCK
// and the detour stayed installed -- which means the game jumps into freed memory once the
// DLL unloads.
void BaseMethodHook::Unhook(void** originalMethod, void* Method) {
	// A hook that never installed (missing export after a game patch) leaves this null.
	// Nothing to detach, and DetourDetach would just fail on it.
	if (originalMethod == NULL || *originalMethod == NULL) {
		return;
	}

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(originalMethod, Method);
	DetourTransactionCommit();
}