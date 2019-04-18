#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include "ReadPlayerTransfer.h"
#include "DataQueue.h"
#include "MessageType.h"
#include "Exports.h"

HANDLE ReadPlayerTransfer::m_hEvent;
DataQueue* ReadPlayerTransfer::m_dataQueue;
ReadPlayerTransfer::OriginalMethodPtr ReadPlayerTransfer::originalMethod;

void ReadPlayerTransfer::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		READ_PLAYER_TRANSFER,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_ReadPlayerTransfer
	);
}

ReadPlayerTransfer::ReadPlayerTransfer(DataQueue* dataQueue, HANDLE hEvent) {
	ReadPlayerTransfer::m_dataQueue = dataQueue;
	ReadPlayerTransfer::m_hEvent = hEvent;
}

ReadPlayerTransfer::ReadPlayerTransfer() {
	ReadPlayerTransfer::m_hEvent = NULL;
}

void ReadPlayerTransfer::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

int __fastcall ReadPlayerTransfer::HookedMethod(
	void* This,
#if !defined(_AMD64_)
	void* notUsed,
#endif
	void* checkedReader
) {
	// Who knows what this'll do..
	int res = originalMethod(This, checkedReader);

	DataItemPtr item(new DataItem(TYPE_ReadPlayerTransfer, sizeof(res), (char*)&res));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	return res;
}