#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class SeedTestHook : public BaseMethodHook {
public:
	SeedTestHook();
	SeedTestHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	
	typedef void(__thiscall *CharAttributeStore_Equipment_Load)(void* This, void* baseTable, void* prefixTable, void* suffixTable, void* modifierTable, bool unknownWeaponRelated);
	typedef void(__thiscall *CharAttribute_AddJitter)(void* This, float jitter, void* randomUniformPtr);
	typedef double(__stdcall *DefenseAttribute_Jitter)(float baseValue, float jitter, void* randomUniformPtr);
	
	static HANDLE m_hEvent;
	static DataQueue* m_dataQueue;

	static CharAttributeStore_Equipment_Load originalCharAttributeStore_Equipment_Load;
	static CharAttribute_AddJitter originalCharAttribute_AddJitter;
	static DefenseAttribute_Jitter originalDefenseAttribute_Jitter;


	static void __fastcall HookedCharAttribute_AddJitter(void* This, void* unused, float jitter, void* randomUniformPtr);
	static double __stdcall HookedDefenseAttribute_Jitter(float baseValue, float jitter, void* randomUniformPtr);
	static void __fastcall HookedCharAttributeStore_Equipment_Load(void* This, void* unused, void* baseTable, void* prefixTable, void* suffixTable, void* modifierTable, bool unknownWeaponRelated);

};