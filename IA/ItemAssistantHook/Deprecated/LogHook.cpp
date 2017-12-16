#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "LogHook.h"
#include "DataQueue.h"

HANDLE LogHook::m_hEvent;
DataQueue* LogHook::m_dataQueue;
LogHook::OriginalMethodPtr01 LogHook::originalMethod01;
LogHook::OriginalMethodPtr02 LogHook::originalMethod02;
LogHook::CombatManager_ApplyDamage LogHook::originalCombatManager_ApplyDamage;

void LogHook::EnableHook() {
	originalMethod01 = (OriginalMethodPtr01)GetProcAddress(::GetModuleHandle("Engine.dll"), "?Log@Engine@GAME@@UBAXW4LogPriority@2@IPBDZZ");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod01, HookedMethod01);
	DetourTransactionCommit();


	originalCombatManager_ApplyDamage = (CombatManager_ApplyDamage)GetProcAddress(::GetModuleHandle("Game.dll"), "?ApplyDamage@CombatManager@GAME@@QAE_NMABUPlayStatsDamageType@2@W4CombatAttributeType@2@ABV?$vector@I@mem@@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalCombatManager_ApplyDamage, HookedCombatManager_ApplyDamage);
	DetourTransactionCommit();

	
/*
	originalMethod02 = (OriginalMethodPtr02)GetProcAddress(::GetModuleHandle("Engine.dll"), "?Log@Engine@GAME@@UBAXW4LogPriority@2@PBDZZ");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod02, HookedMethod02);
	DetourTransactionCommit();*/
	
}

LogHook::LogHook(DataQueue* dataQueue, HANDLE hEvent) {
	LogHook::m_dataQueue = dataQueue;
	LogHook::m_hEvent = hEvent;
}

LogHook::LogHook() {
	LogHook::m_hEvent = NULL;
}

void LogHook::DisableHook() {
	{
		LONG res1 = DetourTransactionBegin();
		LONG res2 = DetourUpdateThread(GetCurrentThread());
		DetourDetach((PVOID*)&originalMethod01, HookedMethod01);
		DetourTransactionCommit();
	}

	{
		LONG res1 = DetourTransactionBegin();
		LONG res2 = DetourUpdateThread(GetCurrentThread());
		DetourDetach((PVOID*)&originalMethod02, HookedMethod02);
		DetourTransactionCommit();
	}
}

bool __fastcall LogHook::HookedCombatManager_ApplyDamage(void* This, void* Unused, void* a, void* PlayStatsDamageType, void* CombatAttributeType, void* vectors) {
	{
		DataItemPtr item(new DataItem(TYPE_HookedCombatManager_ApplyDamage, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}

	bool r = originalCombatManager_ApplyDamage(This, a, PlayStatsDamageType, CombatAttributeType, vectors);

	{
		DataItemPtr item(new DataItem(TYPE_HookedCombatManager_ApplyDamage_Exit, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}

	return r;
}
// http://reverseengineering.stackexchange.com/questions/2989/hooking-to-a-function-with-variable-argument-list
void __fastcall LogHook::HookedMethod01(void* This, int priority, unsigned int origin, char const* data, ...) {
/*
	{
		size_t pos = 0;
		char buffer[1024] = { 0 };
		memcpy(&buffer + pos, &priority, 4);
		pos += 4;

		memcpy(&origin + pos, &origin, 4);
		pos += 4;


		if (data != NULL) {
			size_t len = strlen(data);
			memcpy(&origin + pos, &len, 4);
			pos += sizeof(len);
		}

		DataItemPtr item(new DataItem(TYPE_LOG01, pos, (char*)&data));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}*/
	{
		size_t pos = 0;
		char buffer[1024] = { 0 };
		void ** Stack = (void**)_AddressOfReturnAddress();

		memcpy(&buffer + pos, Stack[1], 4);
		pos += 4;

		int _priority = (int)Stack[2];
		memcpy(&buffer + pos, &Stack[2], 4);
		pos += 4;

		int _origin = (int)Stack[3];
		memcpy(&buffer + pos, &Stack[3], 4);
		pos += 4;

		char* _data = (char*)Stack[4];
		memcpy(&buffer + pos, &Stack[4], 4);
		pos += 4;

		memcpy(&buffer + pos, &Stack[5], 4);
		pos += 4;

		memcpy(&buffer + pos, &Stack[6], 4);
		pos += 4;

		memcpy(&buffer + pos, &Stack[7], 4);
		pos += 4;

		memcpy(&buffer + pos, &Stack[8], 4);
		pos += 4;

		DataItemPtr item(new DataItem(TYPE_LOG02, pos, (char*)&data));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
	{
		size_t pos = 0;
		char buffer[1024] = { 0 };
		void ** Stack = (void**)_AddressOfReturnAddress();

		char* c = (char*)Stack[1];
		memcpy(&buffer + pos, c, 4);
		pos += 4;

		c = (char*)Stack[2];
		memcpy(&buffer + pos, c, 4);
		pos += 4;

		c = (char*)Stack[3];
		memcpy(&buffer + pos, c, 4);
		pos += 4;

		c = (char*)Stack[4];
		memcpy(&buffer + pos, c, 4);
		pos += 4;

		c = (char*)Stack[5];
		memcpy(&buffer + pos, c, 4);
		pos += 4;

		c = (char*)Stack[6];
		memcpy(&buffer + pos, c, 4);
		pos += 4;

		DataItemPtr item(new DataItem(TYPE_LOG02_, pos, (char*)&data));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}

	//originalMethod01(This, priority, origin, data, origin, data);
}
void __cdecl LogHook::HookedMethod02(void* This, unsigned int origin, char const* data, ...) {
	size_t pos = 0;
	char buffer[1024] = { 0 };

	int priority = -1;
	memcpy(&buffer + pos, &priority, 4);
	pos += 4;

	memcpy(&origin + pos, &origin, 4);
	pos += 4;

/*
	size_t len = min(1023 - pos, strlen(data));
	memcpy(&origin + pos, data, len);
	pos += len + 1;*/


	DataItemPtr item(new DataItem(TYPE_LOG02, pos, (char*)&data));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	//originalMethod02(This, origin, data);
}