#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "EquipmentSeedInfo.h"
#include "Exports.h"


EquipmentSeedInfo* EquipmentSeedInfo::g_self;
void EquipmentSeedInfo::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		ITEM_EQUIPMENT_GETUIDISPLAYTEXT,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_ITEMSEEDDATA_EQ
	);
}

EquipmentSeedInfo::EquipmentSeedInfo(DataQueue* dataQueue, HANDLE hEvent) {
	g_self = this;
	this->m_dataQueue = dataQueue;
	this->m_hEvent = hEvent;
}

EquipmentSeedInfo::EquipmentSeedInfo() {
	EquipmentSeedInfo::m_hEvent = nullptr;
}

void EquipmentSeedInfo::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}


// void GAME::ItemEquipment::GetUIDisplayText(class GAME::Character const *,class mem::vector<struct GAME::GameTextLine> &)
void* __fastcall EquipmentSeedInfo::HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines) {
	void* v = g_self->originalMethod(This, character, gameTextLines);


	// TODO: GetItemReplicaInfo()
	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfo(This, replica);

	// We don't care about items with transmutes, can't loot those.
	if (replica.enchantmentRecord.empty()) {
		std::wstringstream stream;
		stream << GAME::itemReplicaToString(replica) << "\n";

		// iterate through all text lines
		for (auto& it : gameTextLines) {
			stream << it.textClass << ";" << it.text.c_str() << "\n";
		}

		std::wstring str = stream.str();
		g_self->TransferData(str.size() * sizeof(wchar_t), (char*)str.c_str());
	}

	return v;
}