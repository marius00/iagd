#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "NpcDetectionHook13.h"
#include "Exports.h"
#include <codecvt> // wstring_convert
#include "Logger.h"

HANDLE NpcDetectionHook13::m_hEvent;
DataQueue* NpcDetectionHook13::m_dataQueue;
NpcDetectionHook13::OriginalMethodPtr NpcDetectionHook13::originalMethod;

void NpcDetectionHook13::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		"?GetItemCountInStashes@Player@GAME@@UEBAHAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEBV?$vector@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@mem@@1AEAV?$vector@I@6@_N33@Z",
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_DISPLAY_CRAFTER
	);
}

NpcDetectionHook13::NpcDetectionHook13(DataQueue* dataQueue, HANDLE hEvent) {
	NpcDetectionHook13::m_dataQueue = dataQueue;
	NpcDetectionHook13::m_hEvent = hEvent;
}

NpcDetectionHook13::NpcDetectionHook13() {
	NpcDetectionHook13::m_hEvent = NULL;
}

void NpcDetectionHook13::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall NpcDetectionHook13::HookedMethod(
	void* This,
	void* uk0,
	void* uk1,
	void* uk2,
	void* uk3,
	bool a,
	bool b,
	bool c
) {
	try {
		DataItemPtr item(new DataItem(TYPE_DISPLAY_CRAFTER, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in NpcDetectionHook13.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in NpcDetectionHook13.. (triple-dot)");
	}

	// TODO: This could be used to show items held in IA, inside the smith.
	// But would need to manually remove the items from IA upon craft
	// Complexity: High
	void* v = originalMethod(This, uk0, uk1, uk2, uk3, a, b, c);
	return v;
}