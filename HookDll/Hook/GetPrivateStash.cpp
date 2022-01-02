#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "GetPrivateStash.h"
#include "Exports.h"

HANDLE GetPrivateStash::m_hEvent;
DataQueue* GetPrivateStash::m_dataQueue;
GetPrivateStash::OriginalMethodPtr GetPrivateStash::originalMethod;
void* GetPrivateStash::privateStashSack;

void GetPrivateStash::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle(L"Game.dll"), GET_PRIVATE_STASH);
	if (originalMethod == NULL) {
		DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_PRIVATE_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
	
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod64);
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
	DetourDetach((PVOID*)&originalMethod, HookedMethod64);
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
