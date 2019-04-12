#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "CloudGetNumFiles.h"
#include "DataQueue.h"
#include "CUSTOM\Exports.h"

HANDLE CloudGetNumFiles::m_hEvent;
DataQueue* CloudGetNumFiles::m_dataQueue;
CloudGetNumFiles::OriginalMethodPtr CloudGetNumFiles::originalMethod;

void CloudGetNumFiles::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookEngine(
		CLOUD_GET_NUM_FILES,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_CloudGetNumFiles
	);
}

CloudGetNumFiles::CloudGetNumFiles(DataQueue* dataQueue, HANDLE hEvent) {
	CloudGetNumFiles::m_dataQueue = dataQueue;
	CloudGetNumFiles::m_hEvent = hEvent;
}

CloudGetNumFiles::CloudGetNumFiles() {
	CloudGetNumFiles::m_hEvent = NULL;
}

void CloudGetNumFiles::DisableHook() {
	LONG res1 = DetourTransactionBegin();
	LONG res2 = DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&originalMethod, HookedMethod);
	DetourTransactionCommit();
}

unsigned int __fastcall CloudGetNumFiles::HookedMethod(
	void* This
#if !defined(_AMD64_)
	, void* notUsed
#endif
) {
	DataItemPtr item(new DataItem(TYPE_CloudGetNumFiles, 0, 0));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	unsigned int v = originalMethod(This);
	return v;
}