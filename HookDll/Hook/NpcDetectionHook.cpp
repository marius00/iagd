#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "NpcDetectionHook.h"
#include "Exports.h"

HANDLE NpcDetectionHook::m_hEvent;
DataQueue* NpcDetectionHook::m_dataQueue;
NpcDetectionHook::OriginalMethodPtr NpcDetectionHook::originalMethod;
/*
int GetItemCountInStashes(
	class std::basic_string<char, struct std::char_traits<char>, class std::allocator<char> > const &, 
	class mem::vector<class std::basic_string<char, struct std::char_traits<char>, class std::allocator<char> > > const &, 
	class mem::vector<class std::basic_string<char, struct std::char_traits<char>, class std::allocator<char> > > const &, 
	class mem::vector<unsigned int> &, 
	bool, 
	bool) {
}
*/
void NpcDetectionHook::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		GET_PLAYER_ITEM_COUNT_IN_STASHES,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_DISPLAY_CRAFTER
	);
}

NpcDetectionHook::NpcDetectionHook(DataQueue* dataQueue, HANDLE hEvent) {
	NpcDetectionHook::m_dataQueue = dataQueue;
	NpcDetectionHook::m_hEvent = hEvent;
}

NpcDetectionHook::NpcDetectionHook() {
	NpcDetectionHook::m_hEvent = NULL;
}

void NpcDetectionHook::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall NpcDetectionHook::HookedMethod(
	void* This, 
#if !defined(_AMD64_)
	void* notUsed, 
#endif
	void* uk0, 
	void* uk1, 
	void* uk2, 
	void* uk3, 
	bool a, 
	bool b
) {
	DataItemPtr item(new DataItem(TYPE_DISPLAY_CRAFTER, 0, 0));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);
	
	// TODO: This could be used to show items held in IA, inside the smith.
	// But would need to manually remove the items from IA upon craft
	// Complexity: High
	void* v = originalMethod(This, uk0, uk1, uk2, uk3, a, b);
	return v;
}