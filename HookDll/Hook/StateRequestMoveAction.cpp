#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "StateRequestMoveAction.h"

HANDLE StateRequestMoveAction::m_hEvent;
DataQueue* StateRequestMoveAction::m_dataQueue;
std::vector<StateRequestMoveAction::OriginalMethodPtr> StateRequestMoveAction::originalMethods;
int StateRequestMoveAction::m_currentPtr;
void StateRequestMoveAction::EnableHook() {
	void* hook;

	switch (m_currentPtr)
	{
	case 0:
		hook = HookedMethod_Wrap0;
		break;
	case 1:
		hook = HookedMethod_Wrap1;
		break;
	case 2:
		hook = HookedMethod_Wrap2;
		break;
	case 3:
		hook = HookedMethod_Wrap3;
		break;
	case 4:
		hook = HookedMethod_Wrap4;
		break;
	case 5:
		hook = HookedMethod_Wrap5;
		break;
	case 6:
		hook = HookedMethod_Wrap6;
		break;
	case 7:
		hook = HookedMethod_Wrap7;
		break;
	case 8:
		hook = HookedMethod_Wrap8;
		break;
	case 9:
		hook = HookedMethod_Wrap9;
		break;
	case 10:
		hook = HookedMethod_Wrap10;
		break;
	case 11:
		hook = HookedMethod_Wrap11;
		break;

	default:
		// TODO: Better handling of this scenario
		return;
	}

	OriginalMethodPtr originalMethod = (OriginalMethodPtr)HookGame(m_procAddress, hook, m_dataQueue, m_hEvent, TYPE_ControllerRequestMoveAction);
	originalMethods.push_back(originalMethod);
	m_currentPtr++;
}

StateRequestMoveAction::StateRequestMoveAction(DataQueue* dataQueue, HANDLE hEvent, char* procAddress) {
	StateRequestMoveAction::m_dataQueue = dataQueue;
	StateRequestMoveAction::m_hEvent = hEvent;
	m_procAddress = procAddress;
}

StateRequestMoveAction::StateRequestMoveAction() {
	StateRequestMoveAction::m_hEvent = NULL;
}

void StateRequestMoveAction::DisableHook() {
	// TODO: This will be a pain.. heh..
	//Unhook((PVOID*)&originalMethod, &HookedMethod);
}

#if defined(_AMD64_)
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap0(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[0]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap1(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[1]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap2(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[2]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap3(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[3]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap4(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[4]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap5(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[5]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap6(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[6]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap7(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[7]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap8(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[8]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap9(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[9]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap10(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[10]); }
void* __fastcall StateRequestMoveAction::HookedMethod_Wrap11(void* This, bool a, bool b, Vec3f const & xyz) { return HookedMethod(This, a, b, xyz, originalMethods[11]); }
#endif

void* __fastcall StateRequestMoveAction::HookedMethod(void* This, bool a,  bool b,  Vec3f const & xyz, OriginalMethodPtr original) {
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

	DataItemPtr item(new DataItem(TYPE_ControllerRequestMoveAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = original(This, a, b, xyz);
	return v;
}