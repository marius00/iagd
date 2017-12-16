#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "SeedTestHook.h"
#include "DataQueue.h"

HANDLE SeedTestHook::m_hEvent;
DataQueue* SeedTestHook::m_dataQueue;

SeedTestHook::CharAttribute_AddJitter SeedTestHook::originalCharAttribute_AddJitter;
SeedTestHook::DefenseAttribute_Jitter SeedTestHook::originalDefenseAttribute_Jitter;
SeedTestHook::CharAttributeStore_Equipment_Load SeedTestHook::originalCharAttributeStore_Equipment_Load;

void SeedTestHook::EnableHook() {

	originalCharAttribute_AddJitter = (CharAttribute_AddJitter)GetProcAddress(::GetModuleHandle("Game.dll"), "?AddJitter@CharAttribute@GAME@@UAEXMPAVRandomUniform@2@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalCharAttribute_AddJitter, HookedCharAttribute_AddJitter);
	DetourTransactionCommit();



	
	originalDefenseAttribute_Jitter = (DefenseAttribute_Jitter)GetProcAddress(::GetModuleHandle("Game.dll"), "?Jitter@DefenseAttribute@GAME@@MAEMMMAAVRandomUniform@2@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalDefenseAttribute_Jitter, HookedDefenseAttribute_Jitter);
	DetourTransactionCommit();
	

	originalCharAttributeStore_Equipment_Load = (CharAttributeStore_Equipment_Load)GetProcAddress(::GetModuleHandle("Game.dll"), "?Load@CharAttributeStore_Equipment@GAME@@UAEXABVLoadTable@2@PBV32@11_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalCharAttributeStore_Equipment_Load, HookedCharAttributeStore_Equipment_Load);
	DetourTransactionCommit();

}
SeedTestHook::SeedTestHook(DataQueue* dataQueue, HANDLE hEvent) {
	SeedTestHook::m_dataQueue = dataQueue;
	SeedTestHook::m_hEvent = hEvent;
}

SeedTestHook::SeedTestHook() {
	SeedTestHook::m_hEvent = NULL;
}

void SeedTestHook::DisableHook() {
	// Whatever / TODO
}

void __fastcall SeedTestHook::HookedCharAttribute_AddJitter(void* This, void* unused, float jitter, void* randomUniformPtr) {

	size_t pos = 0;
	char buffer[4 * 2] = { 0 };

	memcpy(&buffer + pos, &jitter, 4);
	pos += 4;

	if (randomUniformPtr == NULL) {
		pos += 4;
	}
	else {
		memcpy(&buffer + pos, randomUniformPtr, 4);
		pos += 4;
	}


	DataItemPtr item(new DataItem(TYPE_DEBUG_CharAttribute_AddJitter, pos, (char*)&buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);


	return originalCharAttribute_AddJitter(This, jitter, randomUniformPtr);
}

double __stdcall SeedTestHook::HookedDefenseAttribute_Jitter(float baseValue, float jitter, void* randomUniformPtr) {

	size_t pos = 0;
	char buffer[4*3] = { 0 };

	memcpy(&buffer + pos, &baseValue, 4);
	pos += 4;

	memcpy(&buffer + pos, &jitter, 4);
	pos += 4;
	
	if (randomUniformPtr == NULL) {
		pos += 4;
	}
	else {
		memcpy(&buffer + pos, randomUniformPtr, 4);
		pos += 4;

	}
	

	DataItemPtr item(new DataItem(TYPE_DEBUG_DefenseAttribute_Jitter, pos, (char*)&buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);


	return originalDefenseAttribute_Jitter(baseValue, jitter, randomUniformPtr);
}

void __fastcall SeedTestHook::HookedCharAttributeStore_Equipment_Load(void* This, void* unused, void* baseTable, void* prefixTable, void* suffixTable, void* modifierTable, bool unknownWeaponRelated) {
	
	size_t pos = 0;
	char buffer[4] = { 0 };

	memcpy(&buffer + pos, &unknownWeaponRelated, sizeof(unknownWeaponRelated));
	pos += sizeof(unknownWeaponRelated);
	
	DataItemPtr item(new DataItem(TYPE_DEBUG_CharAttributeStore_Equipment_Load, pos, (char*)&buffer));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);
	
	return originalCharAttributeStore_Equipment_Load(This, baseTable, prefixTable, suffixTable, modifierTable, unknownWeaponRelated);
}

