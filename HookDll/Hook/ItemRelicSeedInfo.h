#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <string>
#include "HookLog.h"
#include "GrimTypes.h"

class ItemRelicSeedInfo : public BaseMethodHook {
public:
	ItemRelicSeedInfo();
	ItemRelicSeedInfo(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log);
	void EnableHook() override;
	void DisableHook() override;

protected:
	typedef void* (__thiscall* OriginalMethodPtr)(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines);
	OriginalMethodPtr originalMethod;

	static ItemRelicSeedInfo* g_self;
	static void* __fastcall HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines);
};