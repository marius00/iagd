#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "InventorySack_AddItem.h"

#include <codecvt>

#include "Exports.h"
#include <random>
#include <boost/property_tree/ptree.hpp>                                        
#include <boost/property_tree/json_parser.hpp>       
#include <tuple>



#define STASH_1 0
#define STASH_2 1
#define STASH_3 2
#define STASH_4 3
#define STASH_5 4
#define STASH_6 5
#define STASH_PRIVATE 1000

std::wstring GetIagdFolder();
void LogToFile(const wchar_t* message);
void LogToFile(std::wstring message);

HANDLE InventorySack_AddItem::m_hEvent;
DataQueue* InventorySack_AddItem::m_dataQueue;

InventorySack_AddItem::GameInfo_GameInfo_Param InventorySack_AddItem::dll_GameInfo_GameInfo_Param;
InventorySack_AddItem::GameInfo_GetHardcore InventorySack_AddItem::dll_GameInfo_GetHardcore;
GetPrivateStash InventorySack_AddItem::privateStashHook;
InventorySack_AddItem::InventorySack_AddItem_Drop InventorySack_AddItem::dll_InventorySack_AddItem_Drop;
InventorySack_AddItem::InventorySack_AddItem_Vec2 InventorySack_AddItem::dll_InventorySack_AddItem_Vec2;
std::wstring InventorySack_AddItem::m_storageFolder;
int InventorySack_AddItem::m_stashTabLootFrom;
ULONGLONG InventorySack_AddItem::m_lastNotificationTickTime;

int ReadPreferredStashLootTab() {
	boost::property_tree::ptree loadPtreeRoot;


	const auto settingsJson = GetIagdFolder() + L"settings.json";
	const std::string settingsJsonAnsi = std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(settingsJson);


	boost::property_tree::read_json(settingsJsonAnsi, loadPtreeRoot);
	const int stashToLootFrom = loadPtreeRoot.get<int>("local.stashToLootFrom");

	if (stashToLootFrom == 0) {
		LogToFile(L"Configured to loot from last stash tab");
		
	} else {
		LogToFile(L"Configured to loot from tab: " + std::to_wstring(stashToLootFrom));
	}

	return stashToLootFrom;
}

void InventorySack_AddItem::EnableHook() {
	// GameInfo::
	dll_GameInfo_GameInfo_Param = (GameInfo_GameInfo_Param)GetProcAddress(::GetModuleHandle(L"Engine.dll"), GAMEINFO_CONSTRUCTOR_ARGS);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourTransactionCommit();

	dll_InventorySack_AddItem_Drop = (InventorySack_AddItem_Drop)GetProcAddress(::GetModuleHandle(L"Game.dll"), "?AddItem@InventorySack@GAME@@QEAA_NPEAVItem@2@_N1@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_AddItem_Drop, Hooked_InventorySack_AddItem_Drop);
	DetourTransactionCommit();


	dll_InventorySack_AddItem_Vec2 = (InventorySack_AddItem_Vec2)GetProcAddress(::GetModuleHandle(L"Game.dll"), "?AddItem@InventorySack@GAME@@QEAA_NAEBVVec2@2@PEAVItem@2@_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_AddItem_Vec2, Hooked_InventorySack_AddItem_Vec2);
	DetourTransactionCommit();

	
	privateStashHook.EnableHook();

	// bool GAME::GameInfo::GetHardcore(void)
	dll_GameInfo_GetHardcore = (GameInfo_GetHardcore)GetProcAddress(::GetModuleHandle(L"Engine.dll"), GET_HARDCORE);


}


InventorySack_AddItem::InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent) {
	InventorySack_AddItem::m_dataQueue = dataQueue;
	InventorySack_AddItem::m_hEvent = hEvent;
	privateStashHook = GetPrivateStash(dataQueue, hEvent);
	m_storageFolder = GetIagdFolder() + L"itemqueue\\";

	CreateDirectoryW(m_storageFolder.c_str(), nullptr);
	m_stashTabLootFrom = ReadPreferredStashLootTab();
	m_lastNotificationTickTime = 0;
}

InventorySack_AddItem::InventorySack_AddItem() {
	InventorySack_AddItem::m_hEvent = NULL;
}

void InventorySack_AddItem::DisableHook() {
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());


	DetourDetach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	
	DetourTransactionCommit();

	privateStashHook.DisableHook();
}



// Since were creating from an existing object we'll need to call Get() on isHardcore and ModLabel
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_GameInfo_Param(void* This , void* info) {
	void* result = dll_GameInfo_GameInfo_Param(This, info);

	bool isHardcore = dll_GameInfo_GetHardcore(This);
	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore_via_init, sizeof(isHardcore), (char*)&isHardcore));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);

	return result;
}



/// <summary>
/// Called primarily when dropping an item into a closed stash (eg drop item from tab 3 to tab 4)
/// </summary>
/// <param name="This"></param>
/// <param name="item"></param>
/// <param name="findPosition"></param>
/// <param name="SkipPlaySound"></param>
/// <returns></returns>
void* __fastcall InventorySack_AddItem::Hooked_InventorySack_AddItem_Drop(void* This, GAME::Item *item, bool findPosition, bool SkipPlaySound) {
	if (HandleItem(This, item)) {
		return (void*)1;
	}

	void* v = dll_InventorySack_AddItem_Drop(This, item, findPosition, SkipPlaySound);
	return v;
}

/// <summary>
/// Regular "add to tab/stash" move. Ctrl+click and regular item drop
/// </summary>
/// <param name="This"></param>
/// <param name="position"></param>
/// <param name="item"></param>
/// <param name="SkipPlaySound"></param>
/// <returns></returns>
void* __fastcall InventorySack_AddItem::Hooked_InventorySack_AddItem_Vec2(void* This, void* position, GAME::Item* item, bool SkipPlaySound) {
	if (HandleItem(This, item)) {
		return (void*)1;
	}

	return dll_InventorySack_AddItem_Vec2(This, position, item, SkipPlaySound);
}

/// <summary>
/// Checks if the provided replica info contains any of the exclusion parameters
/// We are not interested in potions, quest items, components or soulbound items.
/// </summary>
/// <param name="item"></param>
/// <returns></returns>
bool IsRelevant(GAME::ItemReplicaInfo& item) {
	if (item.stackSize > 1) {
		return false;
	}

	if (item.baseRecord.find("/storyelements/") != std::string::npos) {
		return false;
	}

	if (item.baseRecord.find("/materia/") != std::string::npos) {
		return false;
	}

	if (item.baseRecord.find("/questitems/") != std::string::npos) {
		return false;
	}

	if (item.baseRecord.find("/crafting/") != std::string::npos) {
		return false;
	}

	// Transmute
	if (!item.enchantmentRecord.empty()) {
		return false;
	}

	// The item has a materia, it's probably fine to loot, but lets err on the safe side and assume its not.
	if (!item.materiaRecord.empty()) {
		return false;
	}

	return true;
}



std::wstring randomFilename() {
	std::wstring str(L"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");

	std::random_device rd;
	std::mt19937 generator(rd());

	std::shuffle(str.begin(), str.end(), generator);

	return str.substr(0, 32) + L".csv";    // assumes 32 < number of characters in str         
}



bool InventorySack_AddItem::Persist(GAME::ItemReplicaInfo replicaInfo, bool isHardcore, std::wstring mod) {
	std::wstring fullPath = m_storageFolder + randomFilename();
	std::wofstream stream;
	stream.open(fullPath);
	stream << mod.c_str() << ";" << (isHardcore ? 1 : 0)  << ";" << GAME::itemReplicaToString(replicaInfo) << "\n";
	stream.flush();
	stream.close();

	LogToFile(L"Storing to " + fullPath);

	std::ifstream verification;
	verification.open(fullPath);
	if (verification) {
		return true;
	}

	LogToFile(L"Error: written CSV file does not exist");


	return false;
}

void InventorySack_AddItem::NotifyLooted() {
	const ULONGLONG now = GetTickCount64();

	// Limit notifications to 1 per 3s, roughly the fade time.
	if (now - m_lastNotificationTickTime > 3000) {
		GAME::Color color;
		color.r = 1;
		color.g = 1;
		color.b = 1;
		color.a = 1;

		// TODO: How can translation support be added?
		auto header = std::wstring(L"Item looted");
		auto body = std::wstring(L"By Item Assistant");

		GAME::Engine* engine = fnGetEngine();
		fnShowCinematicText(engine, &header, &body, 5, &color);
		m_lastNotificationTickTime = now;
	}
}

/// <summary>
/// Attempt to classify the item as relevant <-> not relevant for looting, and pass it on for persisting.
/// </summary>
/// <param name="stash"></param>
/// <param name="item"></param>
/// <returns></returns>
bool InventorySack_AddItem::HandleItem(void* stash, GAME::Item* item) {
	GAME::GameEngine* gameEngine = fnGetGameEngine();
	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfo(item, replica);
	if (!IsRelevant(replica)) {
		return false;
	}

	void* realPtr = fnGetPlayerTransfer(gameEngine);
	if (realPtr == nullptr) {
		return false;
	}

	auto sacks = static_cast<std::vector<GAME::InventorySack*>*>(realPtr);

	if (sacks->size() < 2) {
		return false;
	}


	// Determine the correct stash sack..
	int toLootFrom;
	if (m_stashTabLootFrom == 0) {
		toLootFrom = sacks->size() - 1;
	} else {
		// m_stashTabLootFrom is index from 1, we never want to go <0 nor >= size.
		toLootFrom = max(0, 
			min(sacks->size() - 1, m_stashTabLootFrom - 1)
		);
	}

	const auto lastSackPtr = sacks->at(toLootFrom);
	if (static_cast<void*>(lastSackPtr) != stash) {
		return false;
	}

	GAME::Engine* engine = fnGetEngine();
	if (engine == nullptr) {
		LogToFile(L"Engine is null, aborting..");
		return false;
	}
	GAME::GameInfo* gameInfo = fnGetGameInfo(fnGetEngine());
	if (gameInfo == nullptr) {
		LogToFile(L"GameInfo is null, aborting..");
		return false;
	}

	std::wstring modName;
	if (fnGetGameInfoMode(gameInfo) != 1) { // != Crucible
		fnGetModNameArg(gameInfo, &modName);
		modName.erase(std::remove(modName.begin(), modName.end(), '\r'), modName.end());
		modName.erase(std::remove(modName.begin(), modName.end(), '\n'), modName.end());
	}
	
	if (Persist(replica, fnGetHardcore(gameInfo), modName)) {
		NotifyLooted();
		fnPlayDropSound(item);
		return true;
	}

	return false;

}