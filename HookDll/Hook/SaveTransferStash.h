#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class SaveTransferStash : public BaseMethodHook {
public:
	SaveTransferStash();
	SaveTransferStash(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;

	void* GetTransferStashInventorySack();
private:
	typedef void* (__thiscall *OriginalMethodPtr)(void*);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;
	static void* m_transferStashSack;

	static void* __fastcall HookedMethod(void* This);
};