#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "Vec3f.h"

class StateRequestRotateAction : public BaseMethodHook {
public:
	StateRequestRotateAction();
	StateRequestRotateAction(DataQueue* dataQueue, HANDLE hEvent, char* procAddress);
	void EnableHook();
	void DisableHook();

private:
	typedef int* (__thiscall *OriginalMethodPtr)(void*, Vec3f const&);
	static int m_currentPtr;
	static HANDLE m_hEvent;
	static DataQueue* m_dataQueue;
	char* m_procAddress;
	static std::vector<OriginalMethodPtr> originalMethods;

	static void* __fastcall HookedMethod(void* This, Vec3f const& xyz, OriginalMethodPtr original);
	static void* __fastcall HookedMethod_Wrap0(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap1(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap2(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap3(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap4(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap5(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap6(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap7(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap8(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap9(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap10(void* This, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap11(void* This, Vec3f const& xyz);
};