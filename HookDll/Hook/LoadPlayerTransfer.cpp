#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>

#include "LoadPlayerTransfer.h"
#include "DataQueue.h"
#include "MessageType.h"
#include "Exports.h"

HANDLE LoadPlayerTransfer::m_hEvent;
DataQueue* LoadPlayerTransfer::m_dataQueue;
LoadPlayerTransfer::OriginalMethodPtr LoadPlayerTransfer::originalMethod;

void LoadPlayerTransfer::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		LOAD_PLAYER_TRANSFER,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_LoadPlayerTransfer
	);
}

LoadPlayerTransfer::LoadPlayerTransfer(DataQueue* dataQueue, HANDLE hEvent) {
	LoadPlayerTransfer::m_dataQueue = dataQueue;
	LoadPlayerTransfer::m_hEvent = hEvent;
}

LoadPlayerTransfer::LoadPlayerTransfer() {
	LoadPlayerTransfer::m_hEvent = NULL;
}

void LoadPlayerTransfer::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

int __fastcall LoadPlayerTransfer::HookedMethod(void* This) {
	// Who knows what this'll do..
	int res = originalMethod(This);

	DataItemPtr item(new DataItem(TYPE_LoadPlayerTransfer, sizeof(res), (char*)&res));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	return res;
}