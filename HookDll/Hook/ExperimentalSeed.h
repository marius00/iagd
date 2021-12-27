#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <string>
#include "HookLog.h"
#include "GrimTypes.h"

typedef std::basic_string<char, std::char_traits<char>, std::allocator<char>> SpecificString;



class ExperimentalSeed : public BaseMethodHook {
public:
	ExperimentalSeed();
	ExperimentalSeed(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log);
	void EnableHook() override;
	void DisableHook() override;

private:
	// typedef void* (__thiscall *OriginalMethodPtr)(void* This);
	//typedef int* (__thiscall* InventorySack_InventorySackParam)(void* This, void* character, class mem::vector<struct GAME::GameTextLine>&);
	typedef void* (__thiscall* OriginalMethodPtr)(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;

	static DataQueue* m_dataQueue;
	static HookLog* g_log;

	static void* __fastcall HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines);
};