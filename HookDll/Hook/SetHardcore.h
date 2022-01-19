#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class SetHardcore : public BaseMethodHook {
public:
	SetHardcore();
	SetHardcore(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;

private:
	typedef void* (__thiscall *OriginalMethodPtr)(void* This, bool isHardcore);
	OriginalMethodPtr originalMethod;

	static SetHardcore* g_self;
	static void* __fastcall HookedMethod(void* This, bool isHardcore);
};