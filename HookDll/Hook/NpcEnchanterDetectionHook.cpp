#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "NpcEnchanterDetectionHook.h"
#include "Exports.h"

HANDLE NpcEnchanterDetectionHook::m_hEvent;
DataQueue* NpcEnchanterDetectionHook::m_dataQueue;
NpcEnchanterDetectionHook::OriginalMethodPtr NpcEnchanterDetectionHook::originalMethod;

void NpcEnchanterDetectionHook::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		NPC_ENCHANTER_ON_PLAYER_INTERACT,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_DISPLAY_ENCHANTER
	);
}

NpcEnchanterDetectionHook::NpcEnchanterDetectionHook(DataQueue* dataQueue, HANDLE hEvent) {
	NpcEnchanterDetectionHook::m_dataQueue = dataQueue;
	NpcEnchanterDetectionHook::m_hEvent = hEvent;
}

NpcEnchanterDetectionHook::NpcEnchanterDetectionHook() {
	NpcEnchanterDetectionHook::m_hEvent = nullptr;
}

void NpcEnchanterDetectionHook::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall NpcEnchanterDetectionHook::HookedMethod(
	void* This, 
	const unsigned int player, 
	__int64 shift, 
	__int64 ctrl
) {
	const DataItemPtr item(new DataItem(TYPE_DISPLAY_ENCHANTER, 0, nullptr));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	void* v = originalMethod(This, player, shift, ctrl);
	return v;
}