#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <set>


/*  
	This class also needs access to sack functionality
	Since the items are added to sack->, and need to hook sack deconstructor to maintain sack3 pointer integrity.
	If ~Sack3() => Sack3 = NULL.

	Maybe this should be a child object of InventorySack_AddItem
*/
class ItemInjectorHook : public BaseMethodHook {

public:
	ItemInjectorHook();
	ItemInjectorHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook(void* inventorySack_AddItem, std::set<void*>* inventorySackSet);
	void DisableHook();
	void SetStash3(void* stash3);
	void* GetStash3();

	static void* gameEngine;
private:
	static std::set<void*>* inventorySackSet;
	const char* pipeName = "\\\\.\\pipe\\gdiahook";

	static DataQueue m_itemQueue;
	static HANDLE m_itemQueueEvent;
	static HANDLE m_thread;

	static DataQueue* m_dataQueue;
	static HANDLE m_hEvent;
	static HANDLE hPipe;
	static void* _inventorySack3;
	static bool isConnected;

	static void Process();
	static void AddItem(char* buffer, DWORD len);
	static void ThreadMain(void*);

	typedef void*(__cdecl *GameItem_CreateItem)(void* replica);
	static GameItem_CreateItem dll_GameItem_CreateItem;

	typedef int*(__thiscall *GameEngine_Update)(void*, void* sphere, void* frustum, bool b, void* frustum2);
	static GameEngine_Update dll_GameEngine_Update;

	// void GAME::Engine::Update(class GAME::Sphere const *,class GAME::WorldFrustum const *,bool,class GAME::WorldFrustum const *)
	static void* __fastcall Hooked_GameEngine_Update(void* This, void* notUsed, void* s, void* f, bool b, void* f2);

	
	//typedef int* (__thiscall *InventorySack_AddItem)(void* sack, void* item, bool findPosition, bool SkipPlaySound);
	typedef int* (__thiscall *InventorySack_AddItem)(void* sack, void* vec2, void* item, bool SkipPlaySound);
	static InventorySack_AddItem dll_InventorySack_AddItem;
};