#pragma once
#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class NpcDetectionHook13 : public BaseMethodHook {
public:
	NpcDetectionHook13();
	NpcDetectionHook13(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;

private:
	typedef void* (__thiscall* OriginalMethodPtr)(void* This, void* uk0, void* uk1, void* uk2, void* uk3, bool a, bool b, bool c);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;

	static DataQueue* m_dataQueue;

	static void* __fastcall HookedMethod(
		void* This,
		void* uk0,
		void* uk1,
		void* uk2,
		void* uk3,
		bool a,
		bool b,
		bool c
	);
};