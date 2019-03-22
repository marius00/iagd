#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "HookLog.h"

/************************************************************************
 Hook for the in-game "GetPrivateStash" method
 Called when opening the stash
/************************************************************************/
class HookWalkTo : public BaseMethodHook {
public:
	HookWalkTo();
	HookWalkTo(DataQueue* dataQueue, HANDLE hEvent, HookLog* logger);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x,y,z,u;
	};

	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;
	static HookLog* m_logger;

#if defined(_AMD64_)
	static void* HookedMethod(void* This, bool, bool, Vec3f const& xyz);
#else
	static void* __fastcall HookedMethod(void* This, void* notUsed, bool, bool, Vec3f const& xyz);
#endif
	
};