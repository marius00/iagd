#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "SetTransferOpen.h"
#include "Exports.h"
#include "Logger.h"

SetTransferOpen* SetTransferOpen::g_self;

void SetTransferOpen::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		SET_TRANSFER_OPEN,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_OPEN_CLOSE_TRANSFER_STASH
	);
}

SetTransferOpen::SetTransferOpen(DataQueue* dataQueue, HANDLE hEvent) {
	g_self = this;
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
}

SetTransferOpen::SetTransferOpen() {
	m_hEvent = nullptr;
}

void SetTransferOpen::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

void* __fastcall SetTransferOpen::HookedMethod(void* This, bool isOpen) {
	char b[1];
	b[0] = (isOpen ? 1 : 0);
	g_self->TransferData(1, (char*)b);
	LogToFile(L"Shared stash is " + std::wstring(isOpen ? L"opened" : L"closed"));

	void* v = g_self->originalMethod(This, isOpen);
	return v;
}