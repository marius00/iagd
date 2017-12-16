#include "CanBePlacedInTransferStashHook.h"
#include <detours.h>
#include "Globals.h"
#include "MessageType.h"

HANDLE CanBePlacedInTransferStashHook::m_hEvent;
DataQueue* CanBePlacedInTransferStashHook::m_dataQueue;
CanBePlacedInTransferStashHook::Item_CanPlaceTransfer CanBePlacedInTransferStashHook::dll_Item_CanPlaceTransfer;
CanBePlacedInTransferStashHook::Item_CanPlaceTransfer CanBePlacedInTransferStashHook::dll_QuestItem_CanPlaceTransfer;

CanBePlacedInTransferStashHook::CanBePlacedInTransferStashHook(DataQueue* dataQueue, HANDLE hEvent) {
	CanBePlacedInTransferStashHook::m_dataQueue = dataQueue;
	CanBePlacedInTransferStashHook::m_hEvent = hEvent;
}

CanBePlacedInTransferStashHook::CanBePlacedInTransferStashHook() {
	CanBePlacedInTransferStashHook::m_hEvent = NULL;
}

void CanBePlacedInTransferStashHook::EnableHook() {
	dll_Item_CanPlaceTransfer = (Item_CanPlaceTransfer)HookGame(
		"?CanBePlacedInTransferStash@Item@GAME@@UBE_NXZ",
		Hooked_Item_CanPlaceTransfer,
		m_dataQueue,
		m_hEvent,
		-1
	);

	dll_QuestItem_CanPlaceTransfer = (Item_CanPlaceTransfer)HookGame(
		"?CanBePlacedInTransferStash@QuestItem@GAME@@UBE_NXZ",
		Hooked_Item_CanPlaceTransfer,
		m_dataQueue,
		m_hEvent,
		-1
	);
}

void CanBePlacedInTransferStashHook::DisableHook() {
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());

	DetourDetach((PVOID*)&dll_Item_CanPlaceTransfer, Hooked_Item_CanPlaceTransfer);
	DetourDetach((PVOID*)&dll_QuestItem_CanPlaceTransfer, Hooked_Item_CanPlaceTransfer);

	DetourTransactionCommit();
}
bool __fastcall CanBePlacedInTransferStashHook::Hooked_Item_CanPlaceTransfer(void* This, void* notUsed) {

	dll_Item_CanPlaceTransfer(This);
	return true;
}
