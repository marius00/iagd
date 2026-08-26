#pragma once
#include <windows.h>
#include "DataQueue.h"

class BaseMethodHook {
public:
	BaseMethodHook();
	BaseMethodHook(DataQueue* dataQueue, HANDLE hEvent);

	/// ProcessDetach holds these as BaseMethodHook* and deletes them through that pointer, which is undefined
	/// behaviour without a virtual destructor and skips ~OnDemandSeedInfo along with its queue and mutex.
	virtual ~BaseMethodHook() = default;

	virtual void EnableHook();
	virtual void DisableHook();
protected:
	static void ReportHookError(DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	static void ReportHookSuccess(DataQueue* m_dataQueue, HANDLE m_hEvent, int id);

	void* HookGame(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	void* HookEngine(char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	void* HookDll(const wchar_t* dll, char* procAddress, void* HookedMethod, DataQueue* m_dataQueue, HANDLE m_hEvent, int id);
	void Unhook(void** originalMethod, void* Method);
	void TransferData(unsigned int size, const char* data);

	int m_messageId;
	DataQueue* m_dataQueue;
	HANDLE m_hEvent;
};