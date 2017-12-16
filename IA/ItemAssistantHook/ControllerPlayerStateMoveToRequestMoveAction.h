#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class ControllerPlayerStateMoveToRequestMoveAction : public BaseMethodHook {
public:
	ControllerPlayerStateMoveToRequestMoveAction();
	ControllerPlayerStateMoveToRequestMoveAction(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x,y,z,u;
	};

	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


	static void* __fastcall HookedMethod(void* This, void* notUsed, bool, bool, Vec3f const& xyz);
	
};