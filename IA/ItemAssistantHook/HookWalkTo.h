#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "HookLog.h"
#include "CUSTOM/Vec3f.h"

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

	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;
	static HookLog* m_logger;

	static void* __fastcall HookedMethod(
		void* This, 
#if !defined(_AMD64_)
		void* notUsed,
#endif
		bool, 
		bool, 
		Vec3f const& xyz
	);

};