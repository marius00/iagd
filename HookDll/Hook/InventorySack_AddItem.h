#pragma once
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "GetPrivateStash.h"

class InventorySack_AddItem : public BaseMethodHook {
public:
	InventorySack_AddItem();
	InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;

private:
	struct Vec2f {
		float x,y;
	};

	static DataQueue* m_dataQueue;
	static HANDLE m_hEvent;
	static GetPrivateStash privateStashHook;


	typedef int* (__thiscall *GameEngine_GetTransferSack)(void* ge, int idx);

	typedef int(__thiscall *GameEngine_SetTransferOpen)(void*, bool);
	typedef int*(__thiscall *GameInfo_GameInfo_Param)(void*, void* info);
	typedef int*(__thiscall *GameInfo_GameInfo)(void*);
	typedef int*(__thiscall *GameInfo_SetHardcore)(void*, bool isHardcore);

	typedef int*(__thiscall *InventorySack_InventorySack)(void*);
	typedef int*(__thiscall *InventorySack_InventorySackParam)(void*, void* stdstring);

	typedef bool(__thiscall *GameInfo_GetHardcore)(void*);
	static GameInfo_GetHardcore dll_GameInfo_GetHardcore;

	typedef char* (__thiscall *GameEngine_GetGameInfo)(void* This);
	
	static GameEngine_SetTransferOpen dll_GameEngine_SetTransferOpen;
	static GameInfo_GameInfo_Param dll_GameInfo_GameInfo_Param;
	static GameInfo_GameInfo dll_GameInfo_GameInfo;
	static GameInfo_SetHardcore dll_GameInfo_SetHardcore;

	// Previously in a separate class
	static void __fastcall Hooked_GameEngine_SetTransferOpen(void* This, bool isOpen);

	// Game info is used to monitor IsHardcore and ModLabel
	static void* __fastcall Hooked_GameInfo_GameInfo_Param(void* This, void* info);
	static void* __fastcall Hooked_GameInfo_SetHardcore(void* This, bool isHardcore);
};