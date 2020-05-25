#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class CanUseDismantle : public BaseMethodHook {
public:
	CanUseDismantle();
	CanUseDismantle(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;

private:
	typedef void* (__thiscall *OriginalMethodPtr)(void* This);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;

	static DataQueue* m_dataQueue;

	static void* __fastcall HookedMethod(void* This);
};