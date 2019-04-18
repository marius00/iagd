
#include "BaseMethodHook.h"
#include "MessageType.h"
#include <detours.h>

BaseMethodHook::BaseMethodHook() {}
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

void* BaseMethodHook::HookDll(char* dll, char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	void* originalMethod = GetProcAddress(::GetModuleHandle(dll), procAddress);
	if (originalMethod == NULL) {
		ReportHookError(m_dataQueue, m_hEvent, id);
	}
	else {
		ReportHookSuccess(m_dataQueue, m_hEvent, id);
	}

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();

	return originalMethod;
}

void* BaseMethodHook::HookGame(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	return HookDll("Game.dll", procAddress, HookedMethod, m_dataQueue, m_hEvent, id);
}

void* BaseMethodHook::HookEngine(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id) {
	return HookDll("Engine.dll", procAddress, HookedMethod, m_dataQueue, m_hEvent, id);
}

void BaseMethodHook::Unhook(void* originalMethod, void* Method) {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, Method);
	DetourTransactionCommit();
}