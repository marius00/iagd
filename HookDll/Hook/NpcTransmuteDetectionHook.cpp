#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "NpcTransmuteDetectionHook.h"
#include "Exports.h"

HANDLE NpcTransmuteDetectionHook::m_hEvent;
DataQueue* NpcTransmuteDetectionHook::m_dataQueue;
NpcTransmuteDetectionHook::OriginalMethodPtr NpcTransmuteDetectionHook::originalMethod;

void NpcTransmuteDetectionHook::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		NPC_TRANSMUTE_ON_PLAYER_INTERACT,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_DISPLAY_TRANSMUTER
	);
}

NpcTransmuteDetectionHook::NpcTransmuteDetectionHook(DataQueue* dataQueue, HANDLE hEvent) {
	NpcTransmuteDetectionHook::m_dataQueue = dataQueue;
	NpcTransmuteDetectionHook::m_hEvent = hEvent;
}

NpcTransmuteDetectionHook::NpcTransmuteDetectionHook() {
	NpcTransmuteDetectionHook::m_hEvent = nullptr;
}

void NpcTransmuteDetectionHook::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall NpcTransmuteDetectionHook::HookedMethod(
	void* This, 
	const unsigned int player, 
	__int64 shift, 
	__int64 ctrl
) {
	const DataItemPtr item(new DataItem(TYPE_DISPLAY_TRANSMUTER, 0, nullptr));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	void* v = originalMethod(This, player, shift, ctrl);
	return v;
}