#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "SetHardcore.h"
#include "Exports.h"
#include "Logger.h"
#include <codecvt> // wstring_convert

SetHardcore* SetHardcore::g_self;

void SetHardcore::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookEngine(
		SET_IS_HARDCORE,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_GameInfo_IsHardcore
	);
}

SetHardcore::SetHardcore(DataQueue* dataQueue, HANDLE hEvent) {
	g_self = this;
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
}

SetHardcore::SetHardcore() {
	m_hEvent = nullptr;
}

void SetHardcore::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall SetHardcore::HookedMethod(void* This, bool isHardcore) {
	try {
		g_self->TransferData(sizeof(isHardcore), (char*)&isHardcore);
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in SetHardcore.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in SetHardcore.. (triple-dot)");
	}

	void* v = g_self->originalMethod(This, isHardcore);
	return v;
}