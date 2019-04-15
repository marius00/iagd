#include "../stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>

#include <detours.h>
#include "SaveManager.h"
#include "../DataQueue.h"
#include "../MessageType.h"
#include "Exports.h"

HANDLE SaveManager::m_hEvent;
DataQueue* SaveManager::m_dataQueue;
SaveManager::OriginalMethodPtr SaveManager::originalMethod;

void SaveManager::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookEngine(
		SAVE_MANAGER_DIRECTREAD,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_SaveManager
	);
}

SaveManager::SaveManager(DataQueue* dataQueue, HANDLE hEvent) {
	SaveManager::m_dataQueue = dataQueue;
	SaveManager::m_hEvent = hEvent;
}

SaveManager::SaveManager() {
	SaveManager::m_hEvent = NULL;
}

void SaveManager::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

int __fastcall SaveManager::HookedMethod(
	void* This,
#if !defined(_AMD64_)
	void* notUsed,
#endif
	void* filename,
	void const* dstBuf,
	unsigned int numBytesToRead,
	bool unknown1,
	bool unknown2
) {
	// Who knows what this'll do..
	int res = originalMethod(This, filename, dstBuf, numBytesToRead, unknown1, unknown2);

	DataItemPtr item(new DataItem(TYPE_InterceptDirectRead, sizeof(res), (char*)&res));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	return 0;
}