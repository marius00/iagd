#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class LogHook : public BaseMethodHook {
public:
	LogHook();
	LogHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x,y,z,u;
	};

	typedef bool(__thiscall *CombatManager_ApplyDamage)(void* This, void* a, void* PlayStatsDamageType, void* CombatAttributeType, void* vectors);

	typedef void(__cdecl *OriginalMethodPtr01)(void* This, int priority, unsigned int origin, char const*, ...);
	typedef void(__cdecl *OriginalMethodPtr02)(void* This, int priority, char const*, ...);
	static HANDLE m_hEvent;
	static OriginalMethodPtr01 originalMethod01;
	static OriginalMethodPtr02 originalMethod02;
	static CombatManager_ApplyDamage originalCombatManager_ApplyDamage;
	static DataQueue* m_dataQueue;


	static void __fastcall HookedMethod01(void* This, int priority, unsigned int origin, char const*, ...);
	static void __cdecl HookedMethod02(void* This, unsigned int origin, char const*, ...);
	static bool __fastcall HookedCombatManager_ApplyDamage(void* This, void* Unused, void* a, void* PlayStatsDamageType, void* CombatAttributeType, void* vectors);

};