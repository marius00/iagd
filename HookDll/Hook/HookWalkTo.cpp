#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "HookWalkTo.h"
#include "Exports.h"

HANDLE HookWalkTo::m_hEvent;
DataQueue* HookWalkTo::m_dataQueue;
HookWalkTo::OriginalMethodPtr HookWalkTo::originalMethod;
HookLog* HookWalkTo::m_logger;

void HookWalkTo::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		REQUEST_MOVE_ACTION_IDLE,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_ControllerPlayerStateIdleRequestMoveAction
	);
}

HookWalkTo::HookWalkTo(DataQueue* dataQueue, HANDLE hEvent, HookLog* logger) {
	HookWalkTo::m_dataQueue = dataQueue;
	HookWalkTo::m_hEvent = hEvent;
	HookWalkTo::m_logger = logger;
}

HookWalkTo::HookWalkTo() {
	HookWalkTo::m_hEvent = NULL;
}

void HookWalkTo::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}


// void GAME::ControllerPlayerStateIdle::RequestMoveAction(bool,bool,class GAME::WorldVec3 const &)
void* HookWalkTo::HookedMethod(
	void* This, 

#if !defined(_AMD64_)
	void* notUsed,
#endif
	bool a, 
	bool b, 
	Vec3f const& xyz
) {
	const size_t bufflen = sizeof(float) * 4 + sizeof(bool) * 2;
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

	memcpy(buffer + pos, &a, sizeof(bool) * 1);
	pos += sizeof(bool);

	memcpy(buffer + pos, &b, sizeof(bool) * 1);
	pos += sizeof(bool);

	DataItemPtr item(new DataItem(TYPE_ControllerPlayerStateIdleRequestMoveAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	void* v = originalMethod(This, a, b, xyz);
	return v;
}