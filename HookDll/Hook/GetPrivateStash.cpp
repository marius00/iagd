#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "GetPrivateStash.h"
#include "Exports.h"
#include <codecvt> // wstring_convert
#include "Logger.h"
#include "GrimTypes.h"

HANDLE GetPrivateStash::m_hEvent;
DataQueue* GetPrivateStash::m_dataQueue;
GetPrivateStash::OriginalMethodPtr GetPrivateStash::originalMethod;
void* GetPrivateStash::privateStashSack;

void GetPrivateStash::EnableHook() {
	originalMethod = (OriginalMethodPtr)GetProcAddressOrLogToFile(L"Game.dll", GET_PRIVATE_STASH);
	if (originalMethod == NULL) {
		// Instaloot needs the private stash sack pointer; failure is logged but no longer reported to the client.
		LogToFile(LogLevel::FATAL, L"Failed to hook GetPrivateStash, instaloot private-stash deposits will not work");
	}
	
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod64);
	DetourTransactionCommit();
}

GetPrivateStash::GetPrivateStash(DataQueue* dataQueue, HANDLE hEvent) {
	GetPrivateStash::m_dataQueue = dataQueue;
	GetPrivateStash::m_hEvent = hEvent;
	GetPrivateStash::privateStashSack = NULL;
}

GetPrivateStash::GetPrivateStash() {
	GetPrivateStash::m_hEvent = NULL;
	GetPrivateStash::privateStashSack = NULL;
}

void GetPrivateStash::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod64);
	DetourTransactionCommit();
}

void* GetPrivateStash::GetPrivateStashInventorySack() {
	return privateStashSack;
}

void* __stdcall GetPrivateStash::HookedMethod64(void* This) {
	void* v = originalMethod(This);
	try {
		// Capture the private stash inventory sack pointer for instaloot; stash open/close status is no longer reported.
		privateStashSack = v;
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in GetPrivateStash.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in GetPrivateStash.. (triple-dot)");
	}

	return v;
}
