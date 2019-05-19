#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "Vec3f.h"

/************************************************************************
/************************************************************************/
class StateRequestMoveAction : public BaseMethodHook {
public:
	StateRequestMoveAction();
	StateRequestMoveAction(DataQueue* dataQueue, HANDLE hEvent, char* procAddress);
	void EnableHook();
	void DisableHook();

private:
	typedef int* (__thiscall *OriginalMethodPtr)(void*, bool, bool, Vec3f const&);
	static int m_currentPtr;
	static HANDLE m_hEvent;
	static DataQueue* m_dataQueue;
	char* m_procAddress;
	static std::vector<OriginalMethodPtr> originalMethods;


	static void* __fastcall HookedMethod(void* This, bool a, bool b, Vec3f const& xyz, OriginalMethodPtr original);
	static void* __fastcall HookedMethod_Wrap0(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap1(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap2(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap3(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap4(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap5(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap6(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap7(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap8(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap9(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap10(void* This, bool a, bool b, Vec3f const& xyz);
	static void* __fastcall HookedMethod_Wrap11(void* This, bool a, bool b, Vec3f const& xyz);
};