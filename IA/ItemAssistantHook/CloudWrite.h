#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class CloudWrite : public BaseMethodHook {
public:
	CloudWrite();
	CloudWrite(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x,y,z,u;
	};

	typedef bool (__thiscall *OriginalMethodPtr)(void*, void* str_filename, void const* unknown0, unsigned int unknown1, bool unknown2);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


	static bool __fastcall HookedMethod(void* This, void* notUsed, void* str_filename, void const* unknown0, unsigned int unknown1, bool unknown2);

};