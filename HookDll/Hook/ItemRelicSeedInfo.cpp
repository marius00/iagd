#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "ItemRelicSeedInfo.h"
#include "Exports.h"


ItemRelicSeedInfo* ItemRelicSeedInfo::g_self;
void ItemRelicSeedInfo::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		"?GetUIDisplayText@ItemArtifact@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@@Z",
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_ITEMSEEDDATA_EQ
	);
}

ItemRelicSeedInfo::ItemRelicSeedInfo(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log) {
	g_self = this;
	this->m_dataQueue = dataQueue;
	this->m_hEvent = hEvent;
}

ItemRelicSeedInfo::ItemRelicSeedInfo() {
	ItemRelicSeedInfo::m_hEvent = nullptr;
}

void ItemRelicSeedInfo::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

auto fnItemGetItemReplicaInfoRelic = ItemGetItemReplicaInfo(GetProcAddress(GetModuleHandle(TEXT("game.dll")), GET_ITEM_REPLICAINFO));

void* __fastcall ItemRelicSeedInfo::HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines) {
	void* v = g_self->originalMethod(This, character, gameTextLines);


	// dump the stats to a file
	std::wofstream itemStatsfile;
	itemStatsfile.open("ItemStats.txt", std::ofstream::out | std::ofstream::app);
	// TODO: GetItemReplicaInfo()
	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfoRelic(This, replica);

	// We don't care about items with transmutes, can't loot those.
	if (replica.enchantmentRecord.empty()) {
		std::wstringstream stream;
		stream << GAME::itemReplicaToString(replica) << "\n";

		// iterate through all text lines
		for (auto& it : gameTextLines) {
			stream << it.textClass << ";" << it.text.c_str() << "\n";
		}

		for (auto& it : gameTextLines)
			itemStatsfile << "TextClass: " << it.textClass << " Text: " << it.text.c_str() << "\n";

		std::wstring str = stream.str();
		g_self->TransferData(str.size() * sizeof(wchar_t), (char*)str.c_str());
	}
	else {
		itemStatsfile << "Skipping due to enchantmentRecord: '" << replica.enchantmentRecord.c_str() << "'\n";
	}

	itemStatsfile.flush();
	itemStatsfile.close();

	return v;
}