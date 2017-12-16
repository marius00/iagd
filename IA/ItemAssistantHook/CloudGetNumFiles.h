#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class CloudGetNumFiles : public BaseMethodHook {
public:
	CloudGetNumFiles();
	CloudGetNumFiles(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x,y,z,u;
	};

	typedef unsigned int (__thiscall *OriginalMethodPtr)(void*);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


	static unsigned int __fastcall HookedMethod(void* This, void* notUsed);

};