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
		TYPE_ITEMSEEDDATA
	);
}

EquipmentSeedInfo::EquipmentSeedInfo(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log) {
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

auto fnItemGetItemReplicaInfoA = ItemGetItemReplicaInfo(GetProcAddress(GetModuleHandle(TEXT("game.dll")), GET_ITEM_REPLICAINFO));

// void GAME::ItemEquipment::GetUIDisplayText(class GAME::Character const *,class mem::vector<struct GAME::GameTextLine> &)
void* __fastcall EquipmentSeedInfo::HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines) {
	void* v = g_self->originalMethod(This, character, gameTextLines);

	std::wstringstream stream;

	// TODO: GetItemReplicaInfo()
	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfoA(This, replica);
	stream << GAME::itemReplicaToString(replica) << "\n";

	// iterate through all text lines
	for (auto& it : gameTextLines) {
		stream << it.textClass << "@@$$::" << it.text.c_str() << "\n";
	}

	std::wstring str = stream.str();
	g_self->TransferData(str.size() * sizeof(wchar_t), (char*)str.c_str());

	return v;
}