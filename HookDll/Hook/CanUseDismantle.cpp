#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "CanUseDismantle.h"
#include "Exports.h"

HANDLE CanUseDismantle::m_hEvent;
DataQueue* CanUseDismantle::m_dataQueue;
CanUseDismantle::OriginalMethodPtr CanUseDismantle::originalMethod;

void CanUseDismantle::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		CAN_USE_DISMANTLE,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_CAN_USE_DISMANTLE
	);
}

CanUseDismantle::CanUseDismantle(DataQueue* dataQueue, HANDLE hEvent) {
	CanUseDismantle::m_dataQueue = dataQueue;
	CanUseDismantle::m_hEvent = hEvent;
}

CanUseDismantle::CanUseDismantle() {
	CanUseDismantle::m_hEvent = nullptr;
}

void CanUseDismantle::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall CanUseDismantle::HookedMethod(void* This) {
	const DataItemPtr item(new DataItem(TYPE_CAN_USE_DISMANTLE, 0, nullptr));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	void* v = originalMethod(This);
	return v;
}