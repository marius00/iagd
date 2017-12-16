#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class SaveTransferStash : public BaseMethodHook {
public:
	SaveTransferStash();
	SaveTransferStash(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	typedef void* (__thiscall *OriginalMethodPtr)(void*);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;
	static void* privateStashSack;

	static void* __fastcall HookedMethod(void* This, void* notUsed);
};