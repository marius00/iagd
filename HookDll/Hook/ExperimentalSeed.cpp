#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "ExperimentalSeed.h"
#include "Exports.h"

HANDLE ExperimentalSeed::m_hEvent;
DataQueue* ExperimentalSeed::m_dataQueue;
ExperimentalSeed::OriginalMethodPtr ExperimentalSeed::originalMethod;
HookLog* ExperimentalSeed::g_log;

void ExperimentalSeed::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		EXPERIMENTAL_HOOK,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_EXPERIMENTAL
	);
}

ExperimentalSeed::ExperimentalSeed(DataQueue* dataQueue, HANDLE hEvent, HookLog* g_log) {
	ExperimentalSeed::m_dataQueue = dataQueue;
	ExperimentalSeed::m_hEvent = hEvent;
	ExperimentalSeed::g_log = g_log;
}

ExperimentalSeed::ExperimentalSeed() {
	ExperimentalSeed::m_hEvent = nullptr;
}

void ExperimentalSeed::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

#define LOG(streamdef) \
{ \
    std::string msg = (((std::ostringstream&)(std::ostringstream().flush() << streamdef)).str()); \
    ExperimentalSeed::g_log.out(msg); \
    msg += _T("\n"); \
    OutputDebugString(msg.c_str()); \
}

auto fnItemGetItemReplicaInfo = ItemGetItemReplicaInfo(GetProcAddress(GetModuleHandle(TEXT("game.dll")), GET_ITEM_REPLICAINFO));

// void GAME::ItemEquipment::GetUIDisplayText(class GAME::Character const *,class mem::vector<struct GAME::GameTextLine> &)
void* __fastcall ExperimentalSeed::HookedMethod(void* This, void* character, std::vector<GAME::GameTextLine>& gameTextLines) {
	void* v = originalMethod(This, character, gameTextLines);

	std::wstringstream stream;

	// TODO: GetItemReplicaInfo()
	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfo(This, replica);
	stream << GAME::itemReplicaToString(replica) << "\n";

	// iterate through all text lines
	for (auto& it : gameTextLines) {
		stream << it.textClass << "@@$$::" << it.text.c_str() << "\n";
	}

	std::wstring str = stream.str();
	const DataItemPtr item(new DataItem(TYPE_EXPERIMENTAL, str.size() * sizeof(wchar_t), (char*)str.c_str()));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	return v;
}