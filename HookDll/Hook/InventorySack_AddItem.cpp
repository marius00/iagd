#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "InventorySack_AddItem.h"
#include "Exports.h"


HANDLE InventorySack_AddItem::m_hEvent;
DataQueue* InventorySack_AddItem::m_dataQueue;

InventorySack_AddItem::GameEngine_SetTransferOpen InventorySack_AddItem::dll_GameEngine_SetTransferOpen;
InventorySack_AddItem::GameInfo_GameInfo_Param InventorySack_AddItem::dll_GameInfo_GameInfo_Param;
InventorySack_AddItem::GameInfo_SetHardcore InventorySack_AddItem::dll_GameInfo_SetHardcore;
InventorySack_AddItem::GameInfo_GetHardcore InventorySack_AddItem::dll_GameInfo_GetHardcore;
GetPrivateStash InventorySack_AddItem::privateStashHook;


void InventorySack_AddItem::EnableHook() {


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

	// bool GAME::GameInfo::GetHardcore(void)
	dll_GameInfo_GetHardcore = (GameInfo_GetHardcore)GetProcAddress(::GetModuleHandle("Engine.dll"), GET_HARDCORE);


}

InventorySack_AddItem::InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent) {
	InventorySack_AddItem::m_dataQueue = dataQueue;
	InventorySack_AddItem::m_hEvent = hEvent;
	privateStashHook = GetPrivateStash(dataQueue, hEvent);
}

InventorySack_AddItem::InventorySack_AddItem() {
	InventorySack_AddItem::m_hEvent = NULL;
}

void InventorySack_AddItem::DisableHook() {
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());


	DetourDetach((PVOID*)&dll_GameEngine_SetTransferOpen, Hooked_GameEngine_SetTransferOpen);

	DetourDetach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourDetach((PVOID*)&dll_GameInfo_SetHardcore, Hooked_GameInfo_SetHardcore);
	
	DetourTransactionCommit();

	privateStashHook.DisableHook();
}

void __fastcall InventorySack_AddItem::Hooked_GameEngine_SetTransferOpen(void* This , bool isOpen) {
	dll_GameEngine_SetTransferOpen(This, isOpen);

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

	return result;
}

//void GAME::GameInfo::SetHardcore(bool)
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetHardcore(void* This , bool isHardcore) {
	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore, sizeof(isHardcore), (char*)&isHardcore));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);

	return dll_GameInfo_SetHardcore(This, isHardcore);
}
