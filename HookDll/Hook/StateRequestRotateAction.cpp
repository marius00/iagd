#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "StateRequestRotateAction.h"
#if defined(_AMD64_)
HANDLE StateRequestRotateAction::m_hEvent;
DataQueue* StateRequestRotateAction::m_dataQueue;
std::vector<StateRequestRotateAction::OriginalMethodPtr> StateRequestRotateAction::originalMethods;
int StateRequestRotateAction::m_currentPtr;

void StateRequestRotateAction::EnableHook() {

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

	OriginalMethodPtr originalMethod = (OriginalMethodPtr)HookGame(m_procAddress, hook, m_dataQueue, m_hEvent, TYPE_ControllerRequestRotateAction);
	originalMethods.push_back(originalMethod);
	m_currentPtr++;
}

StateRequestRotateAction::StateRequestRotateAction(DataQueue* dataQueue, HANDLE hEvent, char* procAddress) {
	StateRequestRotateAction::m_dataQueue = dataQueue;
	StateRequestRotateAction::m_hEvent = hEvent;
	m_procAddress = procAddress;
}

StateRequestRotateAction::StateRequestRotateAction() {
	StateRequestRotateAction::m_hEvent = NULL;
}

void StateRequestRotateAction::DisableHook() {
	// TODO: This will be a pain.. heh..
	//Unhook((PVOID*)&originalMethod, &HookedMethod);
}


#if defined(_AMD64_)
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap0(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[0]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap1(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[1]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap2(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[2]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap3(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[3]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap4(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[4]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap5(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[5]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap6(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[6]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap7(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[7]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap8(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[8]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap9(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[9]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap10(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[10]); }
void* __fastcall StateRequestRotateAction::HookedMethod_Wrap11(void* This, Vec3f const & xyz) { return HookedMethod(This, xyz, originalMethods[11]); }
#endif

void* __fastcall StateRequestRotateAction::HookedMethod(void* This, Vec3f const & xyz, OriginalMethodPtr original) {

	const size_t bufflen = sizeof(float) * 4;
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

	DataItemPtr item(new DataItem(TYPE_ControllerRequestRotateAction, bufflen, (char*)buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	void* v = original(This, xyz);
	return v;
}
#endif