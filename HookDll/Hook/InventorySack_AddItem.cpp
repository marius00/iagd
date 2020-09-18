#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "InventorySack_AddItem.h"
#include "Exports.h"

#define STASH_1 0
#define STASH_2 1
#define STASH_3 2
#define STASH_4 3
#define STASH_5 4
#define STASH_PRIVATE 1000

HANDLE InventorySack_AddItem::m_hEvent;
DataQueue* InventorySack_AddItem::m_dataQueue;

InventorySack_AddItem::GameEngine_GetTransferSack InventorySack_AddItem::dll_GameEngine_GetTransferSack;
InventorySack_AddItem::GameEngine_SetTransferOpen InventorySack_AddItem::dll_GameEngine_SetTransferOpen;
InventorySack_AddItem::GameInfo_GameInfo_Param InventorySack_AddItem::dll_GameInfo_GameInfo_Param;
InventorySack_AddItem::GameInfo_SetHardcore InventorySack_AddItem::dll_GameInfo_SetHardcore;
InventorySack_AddItem::GameInfo_GetHardcore InventorySack_AddItem::dll_GameInfo_GetHardcore;
InventorySack_AddItem::GameEngine_GetGameInfo InventorySack_AddItem::dll_GameEngine_GetGameInfo;
InventorySack_AddItem::InventorySack_Sort InventorySack_AddItem::dll_InventorySack_Sort;
GetPrivateStash InventorySack_AddItem::privateStashHook;

InventorySack_AddItem::InventorySack_InventorySack InventorySack_AddItem::dll_InventorySack_InventorySack;
InventorySack_AddItem::InventorySack_InventorySackParam InventorySack_AddItem::dll_InventorySack_InventorySackParam;

int InventorySack_AddItem::m_isHardcore;
void* InventorySack_AddItem::m_gameEngine = NULL;

// Check if this sack is the Nth transfer sack
// Zero indexed, so 3 == stash 4
bool InventorySack_AddItem::IsTransferStash(void* stash, int idx) {
	if (m_gameEngine != NULL) {
		// class GAME::InventorySack * GAME::GameEngine::GetTransferSack(int)
		if (dll_GameEngine_GetTransferSack != NULL) {
			return dll_GameEngine_GetTransferSack(m_gameEngine, idx) == stash;
		}
	}

	return false;
}

void InventorySack_AddItem::EnableHook() {
	dll_GameEngine_GetTransferSack = (GameEngine_GetTransferSack)GetProcAddress(::GetModuleHandle("Game.dll"), GET_TRANSFER_SACK);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_GetTransferSack, Hooked_GameEngine_GetTransferSack);
	DetourTransactionCommit();

	dll_GameEngine_SetTransferOpen = (GameEngine_SetTransferOpen)GetProcAddress(::GetModuleHandle("Game.dll"), SET_TRANSFER_OPEN);
	if (dll_GameEngine_SetTransferOpen != NULL) {
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach((PVOID*)&dll_GameEngine_SetTransferOpen, Hooked_GameEngine_SetTransferOpen);
		DetourTransactionCommit();
	}
	else {
		DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_TRANSFER_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}


	// GameInfo::
	dll_GameInfo_GameInfo_Param = (GameInfo_GameInfo_Param)GetProcAddress(::GetModuleHandle("Engine.dll"), GAMEINFO_CONSTRUCTOR_ARGS);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourTransactionCommit();

	dll_GameInfo_SetHardcore = (GameInfo_SetHardcore)GetProcAddress(::GetModuleHandle("Engine.dll"), SET_IS_HARDCORE);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_SetHardcore, Hooked_GameInfo_SetHardcore);
	DetourTransactionCommit();

	privateStashHook.EnableHook();

	dll_InventorySack_Sort = (InventorySack_Sort)GetProcAddress(::GetModuleHandle("Game.dll"), SORT_INVENTORY);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_Sort, Hooked_InventorySack_Sort);
	DetourTransactionCommit();



	// bool GAME::GameInfo::GetHardcore(void)
	dll_GameInfo_GetHardcore = (GameInfo_GetHardcore)GetProcAddress(::GetModuleHandle("Engine.dll"), GET_HARDCORE);

	// class GAME::GameInfo * GAME::Engine::GetGameInfo(void)
	dll_GameEngine_GetGameInfo = (GameEngine_GetGameInfo)GetProcAddress(::GetModuleHandle("Engine.dll"), GET_GAME_INFO);

}

InventorySack_AddItem::InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent) {
	InventorySack_AddItem::m_dataQueue = dataQueue;
	InventorySack_AddItem::m_hEvent = hEvent;
	privateStashHook = GetPrivateStash(dataQueue, hEvent);

	m_isHardcore = -1;
}

InventorySack_AddItem::InventorySack_AddItem() {
	InventorySack_AddItem::m_hEvent = NULL;
	m_isHardcore = -1;
}

void InventorySack_AddItem::DisableHook() {
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());


	DetourDetach((PVOID*)&dll_GameEngine_GetTransferSack, Hooked_GameEngine_GetTransferSack);
	DetourDetach((PVOID*)&dll_GameEngine_SetTransferOpen, Hooked_GameEngine_SetTransferOpen);

	DetourDetach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourDetach((PVOID*)&dll_GameInfo_SetHardcore, Hooked_GameInfo_SetHardcore);

	DetourDetach((PVOID*)&dll_InventorySack_Sort, Hooked_InventorySack_Sort);
	
	DetourTransactionCommit();

	privateStashHook.DisableHook();
}

void __fastcall InventorySack_AddItem::Hooked_GameEngine_SetTransferOpen(void* This , bool isOpen) {
	dll_GameEngine_SetTransferOpen(This, isOpen);
	m_gameEngine = This;

	char b[1];
	b[0] = (isOpen ? 1 : 0);
	DataItemPtr item(new DataItem(TYPE_OPEN_CLOSE_TRANSFER_STASH, 1, (char*)b));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

}

// Since were creating from an existing object we'll need to call Get() on isHardcore and ModLabel
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_GameInfo_Param(void* This , void* info) {
	void* result = dll_GameInfo_GameInfo_Param(This, info);

	bool isHardcore = dll_GameInfo_GetHardcore(This);
	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore_via_init, sizeof(isHardcore), (char*)&isHardcore));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);
	m_isHardcore = isHardcore ? 1 : 0;

	return result;
}

//void GAME::GameInfo::SetHardcore(bool)
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetHardcore(void* This , bool isHardcore) {
	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore, sizeof(isHardcore), (char*)&isHardcore));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);
	m_isHardcore = isHardcore ? 1 : 0;

	return dll_GameInfo_SetHardcore(This, isHardcore);
}


// When stash 3 is sorted, IA no longer knows where items are placed
bool __fastcall InventorySack_AddItem::Hooked_InventorySack_Sort(void* This , unsigned int unknown) {
	if (IsTransferStash(This, 2)) { // TODO: This is now dynamic....
		DataItemPtr dataEvent(new DataItem(TYPE_InventorySack_Sort, 0, 0));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}

	return dll_InventorySack_Sort(This, unknown);
}

int* __fastcall InventorySack_AddItem::Hooked_GameEngine_GetTransferSack(void* This , int idx
) {
	int* result = dll_GameEngine_GetTransferSack(This, idx);
	return result;
}
