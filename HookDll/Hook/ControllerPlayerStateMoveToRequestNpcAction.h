#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "Vec3f.h"

/************************************************************************
/************************************************************************/
class ControllerPlayerStateMoveToRequestNpcAction : public BaseMethodHook {
public:
	ControllerPlayerStateMoveToRequestNpcAction();
	ControllerPlayerStateMoveToRequestNpcAction(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:

	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&, void*);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


#if !defined(_AMD64_)
	static void* __fastcall HookedMethod(void* This, void* notUsed, bool, bool, Vec3f const& xyz, void*);
#else
	static void* __fastcall HookedMethod(void* This, bool, bool, Vec3f const& xyz, void*);
#endif
	
};