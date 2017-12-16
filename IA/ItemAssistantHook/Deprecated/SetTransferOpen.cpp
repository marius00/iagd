#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "SetTransferOpen.h"

HANDLE SetTransferOpen::m_hEvent;
DataQueue* SetTransferOpen::m_dataQueue;
SetTransferOpen::OriginalMethodPtr SetTransferOpen::originalMethod;

void SetTransferOpen::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?SetTransferOpen@GameEngine@GAME@@QAEX_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void SetTransferOpen::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

SetTransferOpen::SetTransferOpen(DataQueue* dataQueue, HANDLE hEvent) {
	SetTransferOpen::m_dataQueue = dataQueue;
	SetTransferOpen::m_hEvent = hEvent;
}

SetTransferOpen::SetTransferOpen() {
	SetTransferOpen::m_hEvent = NULL;
}

void __fastcall SetTransferOpen::HookedMethod(void* This, void* notUsed, bool firstParam) {
	originalMethod(This, firstParam);

	char b[1];
	b[0] = (firstParam ? 1 : 0);
	DataItemPtr item(new DataItem(TYPE_OPEN_CLOSE_TRANSFER_STASH, 1, (char*)b));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	
}