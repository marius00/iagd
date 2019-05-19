#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "Vec3f.h"

/************************************************************************
/************************************************************************/
class StateRequestNpcAction : public BaseMethodHook {
public:
	StateRequestNpcAction();
	StateRequestNpcAction(DataQueue* dataQueue, HANDLE hEvent, char* procAddress);
	void EnableHook();
	void DisableHook();

private:
	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&, void* npc);
	static int m_currentPtr;
	static HANDLE m_hEvent;
	static DataQueue* m_dataQueue;
	char* m_procAddress;
	static std::vector<OriginalMethodPtr> originalMethods;

	static void* __fastcall HookedMethod(void* This, bool, bool, Vec3f const& xyz, void* npc, OriginalMethodPtr original);
	// TODO: Ideally this should be possible to write with generics.. but the x64 "some arguments are in registers" complicates that.
	static void* __fastcall HookedMethod_Wrap0(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap1(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap2(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap3(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap4(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap5(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap6(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap7(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap8(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap9(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap10(void* This, bool a, bool b, Vec3f const& xyz, void* npc);
	static void* __fastcall HookedMethod_Wrap11(void* This, bool a, bool b, Vec3f const& xyz, void* npc);

};