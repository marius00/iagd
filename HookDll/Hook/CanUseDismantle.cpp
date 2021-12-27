#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "CanUseDismantle.h"
#include "Exports.h"

CanUseDismantle* CanUseDismantle::g_self;

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
	g_self = this;
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
}

CanUseDismantle::CanUseDismantle() {
	m_hEvent = nullptr;
}

void CanUseDismantle::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall CanUseDismantle::HookedMethod(void* This) {
	g_self->TransferData(0, nullptr);

	void* v = g_self->originalMethod(This);
	return v;
}