#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "ItemSeedInfo.h"
#include "Exports.h"


ItemSeedInfo* ItemSeedInfo::g_self;
void ItemSeedInfo::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		ITEM_GETUIDISPLAYTEXT,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_ITEMSEEDDATA_BASE
	);
}

ItemSeedInfo::ItemSeedInfo(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log) {
	g_self = this;
	this->m_dataQueue = dataQueue;
	this->m_hEvent = hEvent;
}

ItemSeedInfo::ItemSeedInfo() {
	ItemSeedInfo::m_hEvent = nullptr;
}

void ItemSeedInfo::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

auto fnItemGetItemReplicaInfo = ItemGetItemReplicaInfo(GetProcAddress(GetModuleHandle(TEXT("game.dll")), GET_ITEM_REPLICAINFO));

// void GAME::ItemEquipment::GetUIDisplayText(class GAME::Character const *,class mem::vector<struct GAME::GameTextLine> &)
void* __fastcall ItemSeedInfo::HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines) {
	void* v = g_self->originalMethod(This, character, gameTextLines);

	std::wstringstream stream;

	// TODO: GetItemReplicaInfo()
	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfo(This, replica);
	stream << GAME::itemReplicaToString(replica) << "\n";

	// iterate through all text lines
	for (auto& it : gameTextLines) {
		stream << it.textClass << ";" << it.text.c_str() << "\n";
	}

	std::wstring str = stream.str();
	g_self->TransferData(str.size() * sizeof(wchar_t), (char*)str.c_str());

	return v;
}