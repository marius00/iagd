#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "HookWalkTo.h"

HANDLE HookWalkTo::m_hEvent;
DataQueue* HookWalkTo::m_dataQueue;
HookWalkTo::OriginalMethodPtr HookWalkTo::originalMethod;

void HookWalkTo::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddress(::GetModuleHandle("Game.dll"), "?RequestMoveAction@ControllerPlayerStateIdle@GAME@@MAEX_N0ABVWorldVec3@2@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

HookWalkTo::HookWalkTo(DataQueue* dataQueue, HANDLE hEvent) {
	HookWalkTo::m_dataQueue = dataQueue;
	HookWalkTo::m_hEvent = hEvent;
}

HookWalkTo::HookWalkTo() {
	HookWalkTo::m_hEvent = NULL;
}

void HookWalkTo::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void* __fastcall HookWalkTo::HookedMethod(void* This, void* notUsed, bool a, bool b, Vec3f const & xyz) {

	const size_t bufflen = sizeof(Vec3f) + sizeof(bool)*2;
	char buffer[bufflen];

	size_t pos = 0;

	memcpy(buffer + pos, &xyz, sizeof(Vec3f));
	pos += sizeof(Vec3f);

	memcpy(buffer + pos, &a, sizeof(bool)*1);
	pos += sizeof(bool);

	memcpy(buffer + pos, &b, sizeof(bool)*1);
	pos += sizeof(bool);

	DataItemPtr item(new DataItem(TYPE_WALKTO, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = originalMethod(This, a, b, xyz);
	return v;
}