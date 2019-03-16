#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "GetPrivateStash.h"

HANDLE GetPrivateStash::m_hEvent;
DataQueue* GetPrivateStash::m_dataQueue;
GetPrivateStash::OriginalMethodPtr GetPrivateStash::originalMethod;
void* GetPrivateStash::privateStashSack;

void GetPrivateStash::EnableHook() {
#if defined(_AMD64_)
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?GetPrivateStash@Player@GAME@@QEAAAEAV?$vector@PEAVInventorySack@GAME@@@mem@@XZ");
#else
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?GetPrivateStash@Player@GAME@@QAEAAV?$vector@PAVInventorySack@GAME@@@mem@@XZ");
#endif
	if (originalMethod == NULL) {
		DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_PRIVATE_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
	
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
#if defined(_AMD64_)
	DetourAttach((PVOID*)&originalMethod, HookedMethod64);
#else
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
#endif
	DetourTransactionCommit();
}

GetPrivateStash::GetPrivateStash(DataQueue* dataQueue, HANDLE hEvent) {
	GetPrivateStash::m_dataQueue = dataQueue;
	GetPrivateStash::m_hEvent = hEvent;
	GetPrivateStash::privateStashSack = NULL;
}

GetPrivateStash::GetPrivateStash() {
	GetPrivateStash::m_hEvent = NULL;
	GetPrivateStash::privateStashSack = NULL;
}

void GetPrivateStash::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
#if defined(_AMD64_)
	DetourDetach((PVOID*)&originalMethod, HookedMethod64);
#else
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
#endif
	DetourTransactionCommit();
}

void* GetPrivateStash::GetPrivateStashInventorySack() {
	return privateStashSack;
}

void* __stdcall GetPrivateStash::HookedMethod64(void* This) {
	void* v = originalMethod(This);

	char b[1];
	b[0] = 1;
	DataItemPtr item(new DataItem(TYPE_OPEN_PRIVATE_STASH, 1, (char*)b));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	privateStashSack = v;
	return v;
}

// This is spammed non stop when the private stash is open(not transfer)
void* __fastcall GetPrivateStash::HookedMethod(void* This, void* notUsed) {

	char b[1];
	b[0] = 1;
	DataItemPtr item(new DataItem(TYPE_OPEN_PRIVATE_STASH, 1, (char*)b));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = originalMethod(This);
	privateStashSack = v;
	return v;
}