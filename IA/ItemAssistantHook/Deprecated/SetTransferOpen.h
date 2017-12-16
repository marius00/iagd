#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
	

/************************************************************************
 Hook for the in-game "SetTransferOpen" method
 Called when setting stash as open/closed (though not always)
/************************************************************************/
class SetTransferOpen : public BaseMethodHook {
public:
	SetTransferOpen();
	SetTransferOpen(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	typedef int (__thiscall *OriginalMethodPtr)(void*, bool);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;

	//void GAME::GameEngine::SetTransferOpen(bool)
	void static __fastcall HookedMethod(void* This, void* notUsed, bool firstParam);
	
};