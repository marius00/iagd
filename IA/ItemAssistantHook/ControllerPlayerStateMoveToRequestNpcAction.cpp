#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "ControllerPlayerStateMoveToRequestNpcAction.h"
#include "CUSTOM\Exports.h"

HANDLE ControllerPlayerStateMoveToRequestNpcAction::m_hEvent;
DataQueue* ControllerPlayerStateMoveToRequestNpcAction::m_dataQueue;
ControllerPlayerStateMoveToRequestNpcAction::OriginalMethodPtr ControllerPlayerStateMoveToRequestNpcAction::originalMethod;

void ControllerPlayerStateMoveToRequestNpcAction::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), REQUEST_NPC_ACTION_MOVETO);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

ControllerPlayerStateMoveToRequestNpcAction::ControllerPlayerStateMoveToRequestNpcAction(DataQueue* dataQueue, HANDLE hEvent) {
	ControllerPlayerStateMoveToRequestNpcAction::m_dataQueue = dataQueue;
	ControllerPlayerStateMoveToRequestNpcAction::m_hEvent = hEvent;
}

ControllerPlayerStateMoveToRequestNpcAction::ControllerPlayerStateMoveToRequestNpcAction() {
	ControllerPlayerStateMoveToRequestNpcAction::m_hEvent = NULL;
}

void ControllerPlayerStateMoveToRequestNpcAction::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void* __fastcall ControllerPlayerStateMoveToRequestNpcAction::HookedMethod(
	void* This, 
#if !defined(_AMD64_)
	void* notUsed, 
#endif
	bool a, 
	bool b, 
	Vec3f const & xyz, 
	void* npc) {

	const size_t bufflen = sizeof(float) * 4 + sizeof(bool)*2;
	char buffer[bufflen];

	size_t pos = 0;

	memcpy(buffer + pos, &xyz.unknown, sizeof(float));
	pos += sizeof(float);

	memcpy(buffer + pos, &xyz.x, sizeof(float));
	pos += sizeof(float);

	memcpy(buffer + pos, &xyz.y, sizeof(float));
	pos += sizeof(float);

	memcpy(buffer + pos, &xyz.z, sizeof(float));
	pos += sizeof(float);

	memcpy(buffer + pos, &a, sizeof(bool)*1);
	pos += sizeof(bool);

	memcpy(buffer + pos, &b, sizeof(bool)*1);
	pos += sizeof(bool);

	DataItemPtr item(new DataItem(TYPE_ControllerPlayerStateMoveToRequestNpcAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = originalMethod(This, a, b, xyz, npc);
	return v;
}