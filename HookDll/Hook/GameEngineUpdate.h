#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <string>
#include "HookLog.h"
#include "GrimTypes.h"

class GameEngineUpdate : public BaseMethodHook {
public:
	GameEngineUpdate();
	GameEngineUpdate(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log);
	void EnableHook() override;
	void DisableHook() override;

protected:
	// void GAME::GameEngine::Update(int)
	typedef void* (__thiscall* OriginalMethodPtr)(void* This, int v);
	OriginalMethodPtr originalMethod;

	static GameEngineUpdate* g_self;
	static void* __fastcall HookedMethod(void* This, int v);
};