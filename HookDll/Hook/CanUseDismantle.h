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
	OriginalMethodPtr originalMethod;

	static CanUseDismantle* g_self;
	static void* __fastcall HookedMethod(void* This);
};