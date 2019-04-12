#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "CloudRead.h"
#include "DataQueue.h"
#include "CUSTOM\Exports.h"

HANDLE CloudRead::m_hEvent;
DataQueue* CloudRead::m_dataQueue;
CloudRead::OriginalMethodPtr CloudRead::originalMethod;

void CloudRead::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookEngine(
		CLOUD_READ,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_CloudRead
	);
}

CloudRead::CloudRead(DataQueue* dataQueue, HANDLE hEvent) {
	CloudRead::m_dataQueue = dataQueue;
	CloudRead::m_hEvent = hEvent;
}

CloudRead::CloudRead() {
	CloudRead::m_hEvent = NULL;
}

void CloudRead::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

bool __fastcall CloudRead::HookedMethod(
	void* This, 
#if !defined(_AMD64_)
	void* notUsed,
#endif
	void* str_filename, 
	void* unknown0, 
	unsigned int unknown1
) {
	DataItemPtr item(new DataItem(TYPE_CloudRead, 0, 0));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	bool v = originalMethod(This, str_filename, unknown0, unknown1);
	return v;
}