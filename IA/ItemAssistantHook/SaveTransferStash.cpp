#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "SaveTransferStash.h"

HANDLE SaveTransferStash::m_hEvent;
DataQueue* SaveTransferStash::m_dataQueue;
SaveTransferStash::OriginalMethodPtr SaveTransferStash::originalMethod;
void* SaveTransferStash::privateStashSack;

void SaveTransferStash::EnableHook() {

	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?SaveTransferStash@GameEngine@GAME@@QAEXXZ");
	if (originalMethod == NULL) {
		DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_SAVETRANSFER_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
	
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

SaveTransferStash::SaveTransferStash(DataQueue* dataQueue, HANDLE hEvent) {
	SaveTransferStash::m_dataQueue = dataQueue;
	SaveTransferStash::m_hEvent = hEvent;
	SaveTransferStash::privateStashSack = NULL;
}

SaveTransferStash::SaveTransferStash() {
	SaveTransferStash::m_hEvent = NULL;
	SaveTransferStash::privateStashSack = NULL;
}

void SaveTransferStash::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

// This is spammed non stop when the private stash is open(not transfer)
void* __fastcall SaveTransferStash::HookedMethod(void* This, void* notUsed) {

	void* v = originalMethod(This);
	privateStashSack = v;

	// Neat to get this after the save is done -- not before.
	DataItemPtr item(new DataItem(TYPE_SAVE_TRANSFER_STASH, 0, 0));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	return v;
}