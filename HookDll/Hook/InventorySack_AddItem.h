#pragma once
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include "GetPrivateStash.h"
#include "GrimTypes.h"
#include "SaveTransferStash.h"
#include "SettingsReader.h"

class InventorySack_AddItem : public BaseMethodHook {
public:
	InventorySack_AddItem();
	InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook() override;
	void DisableHook() override;
	void SetActive(bool isActive);

private:
	struct Vec2f {
		float x,y;
	};

	static DataQueue* m_dataQueue;
	static HANDLE m_hEvent;
	static GetPrivateStash privateStashHook;
	static int m_stashTabLootFrom;
	static bool m_instalootEnabled;
	static bool m_isGrimDawnParsed;
	static SettingsReader m_settingsReader;
	static bool m_isActive;
	static int m_gameUpdateIterationsRun;


	typedef int* (__thiscall *GameEngine_GetTransferSack)(void* This, int idx);

	typedef int*(__thiscall *GameInfo_GameInfo_Param)(void*, void* info);
	typedef int*(__thiscall *GameInfo_GameInfo)(void*);

	typedef int*(__thiscall *InventorySack_InventorySack)(void*);
	typedef int*(__thiscall *InventorySack_InventorySackParam)(void*, void* stdstring);

	typedef int*(__thiscall *InventorySack_AddItem_Drop)(void* This, GAME::Item* item, bool findPosition, bool SkipPlaySound);
	typedef int*(__thiscall* InventorySack_AddItem_Vec2)(void* This, void* position, GAME::Item* item, bool SkipPlaySound);

	typedef bool(__thiscall *GameInfo_GetHardcore)(void*);
	static GameInfo_GetHardcore dll_GameInfo_GetHardcore;


	typedef int* (__thiscall* GameEngine_Update)(void*, void* sphere, void* frustum, bool b, void* frustum2);
	static GameEngine_Update dll_GameEngine_Update;

	typedef char* (__thiscall *GameEngine_GetGameInfo)(void* This);
	
	static GameInfo_GameInfo_Param dll_GameInfo_GameInfo_Param;
	static GameInfo_GameInfo dll_GameInfo_GameInfo;
	static InventorySack_AddItem_Drop dll_InventorySack_AddItem_Drop;
	static InventorySack_AddItem_Vec2 dll_InventorySack_AddItem_Vec2;
	static std::wstring m_storageFolder;
	static ULONGLONG m_lastNotificationTickTime;


	// Game info is used to monitor IsHardcore and ModLabel
	static void* __fastcall Hooked_GameInfo_GameInfo_Param(void* This, void* info);

	static void* __fastcall Hooked_InventorySack_AddItem_Drop(void* This, GAME::Item* item, bool findPosition, bool SkipPlaySound);
	static void* __fastcall Hooked_InventorySack_AddItem_Vec2(void* This, void*, GAME::Item* item, bool SkipPlaySound);


	// void GAME::Engine::Update(class GAME::Sphere const *,class GAME::WorldFrustum const *,bool,class GAME::WorldFrustum const *)
	static void* __fastcall Hooked_GameEngine_Update(void* This, void* notUsed, void* s, void* f, bool b, void* f2);

	static bool HandleItem(void* stash, GAME::Item* item);
	static bool Persist(GAME::ItemReplicaInfo replicaInfo, bool isHardcore, std::wstring mod);
	static void DisplayMessage(std::wstring, std::wstring);
	static bool IsRelevant(const GAME::ItemReplicaInfo& item);
};
