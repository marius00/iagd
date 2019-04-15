#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "CUSTOM/Vec3f.h"

/************************************************************************
/************************************************************************/
class ControllerPlayerStateMoveToRequestMoveAction : public BaseMethodHook {
public:
	ControllerPlayerStateMoveToRequestMoveAction();
	ControllerPlayerStateMoveToRequestMoveAction(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


	static void* __fastcall HookedMethod(
		void* This, 
#if !defined(_AMD64_)
		void* notUsed, 
#endif
		bool, 
		bool, 
		Vec3f const& xyz
	);
	
};