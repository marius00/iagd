#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class NpcEnchanterDetectionHook : public BaseMethodHook {
public:
	NpcEnchanterDetectionHook();
	NpcEnchanterDetectionHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;

private:
	typedef void* (__thiscall *OriginalMethodPtr)(void* This, unsigned int player, __int64 shift, __int64 ctrl);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;

	static DataQueue* m_dataQueue;

	static void* __fastcall HookedMethod(
		void* This,
		const unsigned int player,
		__int64 shift,
		__int64 ctrl
	);
};