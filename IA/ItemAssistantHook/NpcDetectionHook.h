#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class NpcDetectionHook : public BaseMethodHook {
public:
	NpcDetectionHook();
	NpcDetectionHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	typedef void* (__thiscall *OriginalMethodPtr)(void* This, void* uk0, void* uk1, void* uk2, void* uk3, bool a, bool b);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;

	static DataQueue* m_dataQueue;

	static void* __fastcall HookedMethod(void* This, void* notUsed, void* uk0, void* uk1, void* uk2, void* uk3, bool a, bool b);
};