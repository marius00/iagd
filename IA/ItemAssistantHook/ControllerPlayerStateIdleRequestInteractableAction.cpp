#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "ControllerPlayerStateIdleRequestInteractableAction.h"

HANDLE ControllerPlayerStateIdleRequestInteractableAction::m_hEvent;
DataQueue* ControllerPlayerStateIdleRequestInteractableAction::m_dataQueue;
ControllerPlayerStateIdleRequestInteractableAction::OriginalMethodPtr ControllerPlayerStateIdleRequestInteractableAction::originalMethod;

void ControllerPlayerStateIdleRequestInteractableAction::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?RequestInteractableAction@ControllerPlayerStateIdle@GAME@@MAEX_N0ABVWorldVec3@2@PBVFixedActor@2@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

ControllerPlayerStateIdleRequestInteractableAction::ControllerPlayerStateIdleRequestInteractableAction(DataQueue* dataQueue, HANDLE hEvent) {
	ControllerPlayerStateIdleRequestInteractableAction::m_dataQueue = dataQueue;
	ControllerPlayerStateIdleRequestInteractableAction::m_hEvent = hEvent;
}

ControllerPlayerStateIdleRequestInteractableAction::ControllerPlayerStateIdleRequestInteractableAction() {
	ControllerPlayerStateIdleRequestInteractableAction::m_hEvent = NULL;
}

void ControllerPlayerStateIdleRequestInteractableAction::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void* __fastcall ControllerPlayerStateIdleRequestInteractableAction::HookedMethod(void* This, void* notUsed, bool a, bool b, Vec3f const & xyz, void* actor) {

	const size_t bufflen = sizeof(Vec3f) + sizeof(bool)*2;
	char buffer[bufflen];

	size_t pos = 0;

	memcpy(buffer + pos, &xyz, sizeof(Vec3f));
	pos += sizeof(Vec3f);

	memcpy(buffer + pos, &a, sizeof(bool)*1);
	pos += sizeof(bool);

	memcpy(buffer + pos, &b, sizeof(bool)*1);
	pos += sizeof(bool);

	DataItemPtr item(new DataItem(TYPE_ControllerPlayerStateIdleRequestInteractableAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = originalMethod(This, a, b, xyz, actor);
	return v;
}