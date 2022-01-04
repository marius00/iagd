#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "GameEngineUpdate.h"
#include "Exports.h"


GameEngineUpdate* GameEngineUpdate::g_self;
void GameEngineUpdate::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		"?Update@GameEngine@GAME@@QEAAXH@Z",
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_GAMEENGINE_UPDATE
	);
}

GameEngineUpdate::GameEngineUpdate(DataQueue* dataQueue, HANDLE hEvent) {
	g_self = this;
	this->m_dataQueue = dataQueue;
	this->m_hEvent = hEvent;
}

GameEngineUpdate::GameEngineUpdate() {
	GameEngineUpdate::m_hEvent = nullptr;
}

void GameEngineUpdate::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

//typedef bool(__thiscall* IsGameLoadingPtr)(void* This);
//typedef bool(__thiscall* IsGameWaitingPtr)(void* This, bool);
//IsGameLoadingPtr IsGameLoading = IsGameLoadingPtr(GetProcAddress(GetModuleHandle(L"game.dll"), "?IsGameLoading@GameEngine@GAME@@QEBA_NXZ"));
//IsGameLoadingPtr IsGameEngineOnline = IsGameLoadingPtr(GetProcAddress(GetModuleHandle(L"game.dll"), "?IsGameEngineOnline@GameEngine@GAME@@QEBA_NXZ"));
//IsGameWaitingPtr IsGameWaiting = IsGameWaitingPtr(GetProcAddress(GetModuleHandle(L"game.dll"), "?IsGameWaiting@GameEngine@GAME@@QEAA_N_N@Z"));


void Dump_ItemStats()
{
	std::wofstream itemStatsfile;
	itemStatsfile.open("ItemStats.txt");
	itemStatsfile << "Dump_ItemStats()\n";
	itemStatsfile.flush();

	itemStatsfile.close();
}
void* __fastcall GameEngineUpdate::HookedMethod(void* This, int v) {
	void* r = g_self->originalMethod(This, v);
	

	//std::wofstream itemStatsfile;
	//itemStatsfile.open("ItemStats.txt");
	//itemStatsfile << "Dump_ItemStats()\n";
	//itemStatsfile << "IsGameLoading " << (IsGameLoading(This) ? "true" : "false") << "\n";
	//itemStatsfile << "IsGameEngineOnline " << (IsGameEngineOnline(This) ? "true" : "false") << "\n";
	//itemStatsfile << "IsGameWaiting " << (IsGameWaiting(This, true) ? "true" : "false") << "\n";
	//itemStatsfile.flush();

	//itemStatsfile.close();

	return r;
}