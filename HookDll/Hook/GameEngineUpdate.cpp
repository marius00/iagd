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


void Dump_ItemStats()
{
	std::wofstream itemStatsfile;
	itemStatsfile.open("ItemStats.txt");
	itemStatsfile << "Dump_ItemStats()\n";
	itemStatsfile.flush();

	// check if we have access to the game.dll
	if (GetModuleHandleA("Game.dll"))
	{
		itemStatsfile << "Got GameModuleHandle\n";
		// example: Vampiric Dermapteran Slicer of Piercing Darkness
		GAME::ItemReplicaInfo replica;
		replica.baseRecord = "records/items/gearshoulders/d126_shoulder.dbr"; // Dermapteran Slicer
		replica.seed = 0x4d807ecb;
		replica.relicSeed = 0x41fab3a4;
		replica.stackSize = 1;

		// create the item
		GAME::Item* newItem = fnCreateItem(&replica);
		if (newItem)
		{
			itemStatsfile << "Item OK\n";
			itemStatsfile.flush();

			// this vector gets filled with the item stats 
			std::vector<GAME::GameTextLine> textLine = {};

			// get stats
			fnItemEquipmentGetUIDisplayText((GAME::ItemEquipment*)newItem, (GAME::Character*)fnGetMainPlayer(fnGetgGameEngine()), &textLine);

			itemStatsfile << "fnItemEquipmentGetUIDisplayText OK\n";
			itemStatsfile.flush();

			// delete item
			fnDestroyObjectEx(fnGetObjectManager(), (GAME::Object*)newItem, nullptr, 0);
			itemStatsfile << "fnDestroyObjectEx OK\n";
			itemStatsfile.flush();

			// dump the stats to a file

			// iterate through all text lines
			for (auto& it : textLine)
				itemStatsfile << "TextClass: " << it.textClass << " Text: " << it.text.c_str() << "\n";

			// close file again
			itemStatsfile.flush();

		}
		else {
			itemStatsfile << "Item == NULL\n";
			itemStatsfile.flush();
		}
	}
	else {
		itemStatsfile << "GetModuleHandleA == NULL\n";
		itemStatsfile.flush();
	}

	itemStatsfile << "=============\n\n";
	itemStatsfile.flush();
	itemStatsfile.close();
}

void* __fastcall GameEngineUpdate::HookedMethod(void* This, int v) {
	void* r = g_self->originalMethod(This, v);
	Dump_ItemStats();
	return r;
}