#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "ControllerPlayerStateMoveToRequestMoveAction.h"

HANDLE ControllerPlayerStateMoveToRequestMoveAction::m_hEvent;
DataQueue* ControllerPlayerStateMoveToRequestMoveAction::m_dataQueue;
ControllerPlayerStateMoveToRequestMoveAction::OriginalMethodPtr ControllerPlayerStateMoveToRequestMoveAction::originalMethod;

void ControllerPlayerStateMoveToRequestMoveAction::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?RequestMoveAction@ControllerPlayerStateMoveTo@GAME@@MAEX_N0ABVWorldVec3@2@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

ControllerPlayerStateMoveToRequestMoveAction::ControllerPlayerStateMoveToRequestMoveAction(DataQueue* dataQueue, HANDLE hEvent) {
	ControllerPlayerStateMoveToRequestMoveAction::m_dataQueue = dataQueue;
	ControllerPlayerStateMoveToRequestMoveAction::m_hEvent = hEvent;
}

ControllerPlayerStateMoveToRequestMoveAction::ControllerPlayerStateMoveToRequestMoveAction() {
	ControllerPlayerStateMoveToRequestMoveAction::m_hEvent = NULL;
}

void ControllerPlayerStateMoveToRequestMoveAction::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void* __fastcall ControllerPlayerStateMoveToRequestMoveAction::HookedMethod(void* This, void* notUsed, bool a, bool b, Vec3f const & xyz) {

	const size_t bufflen = sizeof(Vec3f) + sizeof(bool)*2;
	char buffer[bufflen];

	size_t pos = 0;

	memcpy(buffer + pos, &xyz, sizeof(Vec3f));
	pos += sizeof(Vec3f);

	memcpy(buffer + pos, &a, sizeof(bool)*1);
	pos += sizeof(bool);

	memcpy(buffer + pos, &b, sizeof(bool)*1);
	pos += sizeof(bool);

	DataItemPtr item(new DataItem(TYPE_ControllerPlayerStateMoveToRequestMoveAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = originalMethod(This, a, b, xyz);
	return v;
}