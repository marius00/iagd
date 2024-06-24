#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "CanUseDismantle.h"
#include "Exports.h"
#include <codecvt> // wstring_convert
#include "Logger.h"

CanUseDismantle* CanUseDismantle::g_self;

void CanUseDismantle::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		CAN_USE_DISMANTLE,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_CAN_USE_DISMANTLE
	);
}

CanUseDismantle::CanUseDismantle(DataQueue* dataQueue, HANDLE hEvent) {
	g_self = this;
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
}

CanUseDismantle::CanUseDismantle() {
	m_hEvent = nullptr;
}

void CanUseDismantle::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall CanUseDismantle::HookedMethod(void* This) {
	try {
		g_self->TransferData(0, nullptr);
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in CanUseDismantle.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in CanUseDismantle.. (triple-dot)");
	}

	void* v = g_self->originalMethod(This);
	return v;
}