#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "CloudWrite.h"
#include "DataQueue.h"
#include "Exports.h"

HANDLE CloudWrite::m_hEvent;
DataQueue* CloudWrite::m_dataQueue;
CloudWrite::OriginalMethodPtr CloudWrite::originalMethod;

void CloudWrite::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookEngine(
		CLOUD_WRITE,
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_CloudWrite
	);
}

CloudWrite::CloudWrite(DataQueue* dataQueue, HANDLE hEvent) {
	CloudWrite::m_dataQueue = dataQueue;
	CloudWrite::m_hEvent = hEvent;
}

CloudWrite::CloudWrite() {
	CloudWrite::m_hEvent = NULL;
}

void CloudWrite::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

bool __fastcall CloudWrite::HookedMethod(
	void* This, 
#if !defined(_AMD64_)
	void* notUsed,
#endif
	void* str_filename, 
	void const* unknown0, 
	unsigned int unknown1, 
	bool unknown2
) {
	DataItemPtr item(new DataItem(TYPE_CloudWrite, 0, 0));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

	bool v = originalMethod(This, str_filename, unknown0, unknown1, unknown2);
	return v;
}