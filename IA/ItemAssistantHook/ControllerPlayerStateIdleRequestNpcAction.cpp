#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "ControllerPlayerStateIdleRequestNpcAction.h"
#include "CUSTOM\Exports.h"

HANDLE ControllerPlayerStateIdleRequestNpcAction::m_hEvent;
DataQueue* ControllerPlayerStateIdleRequestNpcAction::m_dataQueue;
ControllerPlayerStateIdleRequestNpcAction::OriginalMethodPtr ControllerPlayerStateIdleRequestNpcAction::originalMethod;

void ControllerPlayerStateIdleRequestNpcAction::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), REQUEST_NPC_ACTION);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

ControllerPlayerStateIdleRequestNpcAction::ControllerPlayerStateIdleRequestNpcAction(DataQueue* dataQueue, HANDLE hEvent) {
	ControllerPlayerStateIdleRequestNpcAction::m_dataQueue = dataQueue;
	ControllerPlayerStateIdleRequestNpcAction::m_hEvent = hEvent;
}

ControllerPlayerStateIdleRequestNpcAction::ControllerPlayerStateIdleRequestNpcAction() {
	ControllerPlayerStateIdleRequestNpcAction::m_hEvent = NULL;
}

void ControllerPlayerStateIdleRequestNpcAction::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void* __fastcall ControllerPlayerStateIdleRequestNpcAction::HookedMethod(
	void* This, 
#if !defined(_AMD64_)
	void* notUsed, 
#endif
	bool a, 
	bool b, 
	Vec3f const & xyz, 
	void* npc
) {

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

	DataItemPtr item(new DataItem(TYPE_ControllerPlayerStateIdleRequestNpcAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = originalMethod(This, a, b, xyz, npc);
	return v;
}