#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "StateRequestNpcAction.h"

HANDLE StateRequestNpcAction::m_hEvent;
DataQueue* StateRequestNpcAction::m_dataQueue;
std::vector<StateRequestNpcAction::OriginalMethodPtr> StateRequestNpcAction::originalMethods;
int StateRequestNpcAction::m_currentPtr;

void StateRequestNpcAction::EnableHook() {
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

	OriginalMethodPtr originalMethod = (OriginalMethodPtr)HookGame(m_procAddress, hook, m_dataQueue, m_hEvent, TYPE_ControllerRequestNpcAction);
	originalMethods.push_back(originalMethod);
	m_currentPtr++;
}

StateRequestNpcAction::StateRequestNpcAction(DataQueue* dataQueue, HANDLE hEvent, char* procAddress) {
	StateRequestNpcAction::m_dataQueue = dataQueue;
	StateRequestNpcAction::m_hEvent = hEvent;
	m_procAddress = procAddress;
}

StateRequestNpcAction::StateRequestNpcAction() {
	StateRequestNpcAction::m_hEvent = NULL;
}

void StateRequestNpcAction::DisableHook() {
	// TODO: This will be a pain.. heh..
	//Unhook((PVOID*)&originalMethod, &HookedMethod);
}

void* __fastcall StateRequestNpcAction::HookedMethod_Wrap0(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[0]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap1(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[1]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap2(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[2]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap3(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[3]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap4(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[4]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap5(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[5]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap6(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[6]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap7(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[7]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap8(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[8]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap9(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[9]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap10(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[10]); }
void* __fastcall StateRequestNpcAction::HookedMethod_Wrap11(void* This, bool a, bool b, Vec3f const & xyz, void* npc) { return HookedMethod(This, a, b, xyz, npc, originalMethods[11]); }

void* __fastcall StateRequestNpcAction::HookedMethod(void* This, bool a, bool b, Vec3f const & xyz, void* npc, OriginalMethodPtr original) {

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

	DataItemPtr item(new DataItem(TYPE_ControllerRequestNpcAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);	

	void* v = original(This, a, b, xyz, npc);
	return v;
}