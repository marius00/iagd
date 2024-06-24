#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "SaveTransferStash.h"
#include "Exports.h"
#include "GrimTypes.h"
#include "Logger.h"
#include <codecvt> // wstring_convert

HANDLE SaveTransferStash::m_hEvent;
DataQueue* SaveTransferStash::m_dataQueue;
SaveTransferStash::OriginalMethodPtr SaveTransferStash::originalMethod;
void* SaveTransferStash::m_transferStashSack;

void SaveTransferStash::EnableHook() {

	originalMethod = (OriginalMethodPtr)GetProcAddressOrLogToFile(L"Game.dll", SAVE_TRANSFER_STASH);
	if (originalMethod == NULL) {
		DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_SAVETRANSFER_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
	
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

SaveTransferStash::SaveTransferStash(DataQueue* dataQueue, HANDLE hEvent) {
	SaveTransferStash::m_dataQueue = dataQueue;
	SaveTransferStash::m_hEvent = hEvent;
	SaveTransferStash::m_transferStashSack = NULL;
}

SaveTransferStash::SaveTransferStash() {
	SaveTransferStash::m_hEvent = NULL;
	SaveTransferStash::m_transferStashSack = NULL;
}

void SaveTransferStash::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

void* SaveTransferStash::GetTransferStashInventorySack() {
	return m_transferStashSack;
}

// This is spammed non stop when the private stash is open(not transfer)
void* __fastcall SaveTransferStash::HookedMethod(void* This) {
	void* v = originalMethod(This);
	try {

		m_transferStashSack = v;

		// Neat to get this after the save is done -- not before.
		DataItemPtr item(new DataItem(TYPE_SAVE_TRANSFER_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);

		LogToFile(L"Shared stash is closed (saved)");
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in SaveTransferStash.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in SaveTransferStash.. (triple-dot)");
	}

	return v;
}