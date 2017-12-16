#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class CloudRead : public BaseMethodHook {
public:
	CloudRead();
	CloudRead(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x,y,z,u;
	};

	typedef bool (__thiscall *OriginalMethodPtr)(void*, void* str_filename, void* unknown0, unsigned int unknown1);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


	static bool __fastcall HookedMethod(void* This, void* notUsed, void* str_filename, void* unknown0, unsigned int unknown1);

};