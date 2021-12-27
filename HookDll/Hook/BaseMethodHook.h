#pragma once
#include <windows.h>
#include "DataQueue.h"

class BaseMethodHook {
public:
	BaseMethodHook();
	BaseMethodHook(DataQueue* dataQueue, HANDLE hEvent);
	virtual void EnableHook();
	virtual void DisableHook();
protected:
	static void ReportHookError(DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	static void ReportHookSuccess(DataQueue* m_dataQueue, HANDLE m_hEvent, int id);

	void* HookGame(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	void* HookEngine(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	void* HookDll(char* dll, char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	void Unhook(void* originalMethod, void* Method);
	void TransferData(unsigned int size, char* data);

	int m_messageId;
	DataQueue* m_dataQueue;
	// HookLog* m_log;
	HANDLE m_hEvent;
};