#pragma once
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "GetPrivateStash.h"
#define MOD_FUNCTIONALITY_ENABLED 0
/************************************************************************
AddItem called from DLL:
 Option #1:
 => Send data to IA
 => Skip adding it (if IA is not running, this is bad)
 (This may be problematic if the caller code uses the ID for anything)


 Option #2:
 => Add item (dll code)
 => Send Item + Id to IA
 => Have IA store the item in a cache
 => Have IA ask for a deletion by Id
 => Calls RemoveItem(id)
 (May be problematic in quick place & pickup situations)
 (May be problematic if player closes the stash? obj deleted? maybe not since crafting uses it?)
 => Signal IA a successful removal, so IA can activate the item?


 Sending items to GD:
 => IA marks item as 'inactive'
 => IA signals hook
 => Check IsSpaceForItem
 => Call AddItem on dll
 => Call SetItemAddedWhileNotTheCurrentlySelectedInventoryTab
 => Signal IA successful add, so IA can delete the item
 : All inactives are activated on start, or after N seconds.

 How do I
 char __thiscall GAME::GameEngine::AddTransferSack(GAME::GameEngine *this)
 void GAME::GameEngine::SetSelectedTransferSackNumber(unsigned int)

 I can always know when a transfer is being created/destroyed via its (des)constructor
 So I will know if stash+2 still exists



bool GAME::InventorySack::AddItem(class GAME::Item *,bool,bool)
bool GAME::InventorySack::AddItem(class GAME::Vec2 const &,class GAME::Item *,bool)
bool GAME::InventorySack::FindNextPosition(class GAME::Item const *,class GAME::Rect &,bool)
bool GAME::InventorySack::IsSpaceForItem(class GAME::Item const *,bool)
class GAME::Vec2 GAME::InventorySack::AddItemAndReturnPoint(class GAME::Item *)

class GAME::Item * GAME::Item::CreateItem(struct GAME::ItemReplicaInfo const &)

void GAME::Character::GiveItemToCharacter(class GAME::Item *,bool)

bool GAME::Player::IsInventorySpaceAvailable(class GAME::Item const *)
void GAME::Player::GiveItemToCharacter(class GAME::Item *,bool)

bool GAME::InventorySack::RemoveItem(unsigned int)
bool GAME::Player::RemoveItemFromPrivateStash(unsigned int)
void GAME::Inventory::RemoveItemFromInventory(unsigned int)

GAME::InventorySack::SetItemAddedWhileNotTheCurrentlySelectedInventoryTab(bool)
/************************************************************************/

#if !defined(_AMD64_)
#define DISCARD_ARG ,void* discarded
#else
#define DISCARD_ARG
#endif

class InventorySack_AddItem : public BaseMethodHook {
public:
	InventorySack_AddItem();
	InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec2f {
		float x,y;
	};
	//static OriginalAttachItem dll_AttachItem;
	static DataQueue* m_dataQueue;
	static HANDLE m_hEvent;
	static void* m_gameEngine;
	static GetPrivateStash privateStashHook;

	static bool IsTransferStash(void* stash, int idx);
	static int GetStashIndex(void* stash);

	static int m_isHardcore;
	static int m_stashToLootFrom;

	// void GAME::Item::GetItemReplicaInfo(struct GAME::ItemReplicaInfo &)
	typedef void* (__thiscall *Item_GetItemReplicaInfo)(void* This, void* replicaInfo);

	typedef int* (__thiscall *GameEngine_GetTransferSack)(void* ge, int idx);

	typedef int* (__thiscall *GameEngine_AddItemToTransfer_01)(void*, unsigned int, void* Vec2, unsigned int index, bool);
	typedef int* (__thiscall *GameEngine_AddItemToTransfer_02)(void*, unsigned int, unsigned int index, bool);

	typedef int(__thiscall *GameEngine_SetTransferOpen)(void*, bool);
	typedef int*(__thiscall *GameInfo_GameInfo_Param)(void*, void* info);
	typedef int*(__thiscall *GameInfo_GameInfo)(void*);
	typedef int*(__thiscall *GameInfo_SetHardcore)(void*, bool isHardcore);

	typedef int*(__thiscall *InventorySack_InventorySack)(void*);
	typedef int*(__thiscall *InventorySack_InventorySackParam)(void*, void* stdstring);

	typedef bool(__thiscall *InventorySack_Sort)(void*, unsigned int unknown);

	typedef bool(__thiscall *GameInfo_GetHardcore)(void*);
	static GameInfo_GetHardcore dll_GameInfo_GetHardcore;


	typedef char* (__thiscall *GameEngine_GetGameInfo)(void* This);
	static GameEngine_GetGameInfo dll_GameEngine_GetGameInfo;


	
	static GameEngine_GetTransferSack dll_GameEngine_GetTransferSack;
	static GameEngine_SetTransferOpen dll_GameEngine_SetTransferOpen;
	static GameInfo_GameInfo_Param dll_GameInfo_GameInfo_Param;
	static GameInfo_GameInfo dll_GameInfo_GameInfo;
	static GameInfo_SetHardcore dll_GameInfo_SetHardcore;
	static InventorySack_InventorySackParam dll_InventorySack_InventorySackParam;
	static InventorySack_InventorySack dll_InventorySack_InventorySack;
	static InventorySack_Sort dll_InventorySack_Sort;

	// Previously in a separate class
	// void GAME::GameEngine::SetTransferOpen(bool)
	static void __fastcall Hooked_GameEngine_SetTransferOpen(void* This DISCARD_ARG, bool firstParam);


	// Game info is used to monitor IsHardcore and ModLabel
	//GAME::GameInfo::GameInfo(class GAME::GameInfo const &)
	static void* __fastcall Hooked_GameInfo_GameInfo_Param(void* This DISCARD_ARG, void* info);
	static void* __fastcall Hooked_GameInfo_GameInfo(void* This DISCARD_ARG);

	//void GAME::GameInfo::SetHardcore(bool)
	static void* __fastcall Hooked_GameInfo_SetHardcore(void* This DISCARD_ARG, bool isHardcore);
	static bool __fastcall Hooked_InventorySack_Sort(void* This DISCARD_ARG, unsigned int unknown);
	static int* __fastcall Hooked_GameEngine_GetTransferSack(void* This DISCARD_ARG, int idx);
};