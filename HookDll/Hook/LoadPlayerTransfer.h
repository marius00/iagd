#pragma once
#pragma once
#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class DataQueue;

/************************************************************************
/************************************************************************/
class LoadPlayerTransfer : public BaseMethodHook {
public:
	LoadPlayerTransfer();
	LoadPlayerTransfer(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	typedef int(__thiscall *OriginalMethodPtr)(void* This);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;
	static int __fastcall HookedMethod(void* This);

};
