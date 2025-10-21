#include "stdafx.h"
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "InventorySack_AddItem.h"

#include <codecvt>

#include "Exports.h"
#include <random>
#include <boost/property_tree/ptree.hpp>                                        
#include <boost/property_tree/json_parser.hpp>       
#include <boost/filesystem.hpp>
#include <boost/range/iterator_range.hpp>
#include <boost/algorithm/string/predicate.hpp>
#include <iostream>
#include "Logger.h"


#define STASH_1 0
#define STASH_2 1
#define STASH_3 2
#define STASH_4 3
#define STASH_5 4
#define STASH_6 5
#define STASH_PRIVATE 1000

std::wstring GetIagdFolder();

HANDLE InventorySack_AddItem::m_hEvent;
DataQueue* InventorySack_AddItem::m_dataQueue;

InventorySack_AddItem::GameInfo_GameInfo_Param InventorySack_AddItem::dll_GameInfo_GameInfo_Param;
InventorySack_AddItem::GameInfo_GetHardcore InventorySack_AddItem::dll_GameInfo_GetHardcore;
GetPrivateStash InventorySack_AddItem::privateStashHook;
InventorySack_AddItem::InventorySack_AddItem_Drop InventorySack_AddItem::dll_InventorySack_AddItem_Drop;
InventorySack_AddItem::InventorySack_AddItem_Vec2 InventorySack_AddItem::dll_InventorySack_AddItem_Vec2;
InventorySack_AddItem::InventorySack_SetTransferOpen InventorySack_AddItem::dll_InventorySack_SetTransferOpen;
InventorySack_AddItem::InventorySack_FindNextPosition InventorySack_AddItem::dll_InventorySack_FindNextPosition;
std::wstring InventorySack_AddItem::m_storageFolder;
int InventorySack_AddItem::m_stashTabLootFrom;
int InventorySack_AddItem::m_stashTabDepositTo;
ULONGLONG InventorySack_AddItem::m_lastNotificationTickTime;
bool InventorySack_AddItem::m_instalootEnabled;
bool InventorySack_AddItem::m_isGrimDawnParsed;
SettingsReader InventorySack_AddItem::m_settingsReader;
bool InventorySack_AddItem::m_isActive;
int InventorySack_AddItem::m_gameUpdateIterationsRun;
InventorySack_AddItem::GameEngine_Update InventorySack_AddItem::dll_GameEngine_Update;
bool InventorySack_AddItem::m_isTransferStashOpen;

std::set<std::wstring> InventorySack_AddItem::m_depositQueue;
boost::mutex InventorySack_AddItem::m_mutex;

void InventorySack_AddItem::EnableHook() {
	// GameInfo::
	dll_GameInfo_GameInfo_Param = (GameInfo_GameInfo_Param)GetProcAddressOrLogToFile(L"Engine.dll", GAMEINFO_CONSTRUCTOR_ARGS);
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourTransactionCommit();

	dll_InventorySack_AddItem_Drop = (InventorySack_AddItem_Drop)GetProcAddressOrLogToFile(L"Game.dll", "?AddItem@InventorySack@GAME@@QEAA_NPEAVItem@2@_N1@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_AddItem_Drop, Hooked_InventorySack_AddItem_Drop);
	DetourTransactionCommit();


	dll_InventorySack_AddItem_Vec2 = (InventorySack_AddItem_Vec2)GetProcAddressOrLogToFile(L"Game.dll", "?AddItem@InventorySack@GAME@@QEAA_NAEBVVec2@2@PEAVItem@2@_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_AddItem_Vec2, Hooked_InventorySack_AddItem_Vec2);
	DetourTransactionCommit();


	dll_InventorySack_SetTransferOpen = (InventorySack_SetTransferOpen)HookGame(
		SET_TRANSFER_OPEN,
		Hooked_InventorySack_SetTransferOpen,
		m_dataQueue,
		m_hEvent,
		TYPE_OPEN_CLOSE_TRANSFER_STASH
	);

	m_isTransferStashOpen = false;

	dll_GameEngine_Update = (GameEngine_Update)HookGame(
		"?Update@GameEngine@GAME@@QEAAXH@Z",
		Hooked_GameEngine_Update,
		m_dataQueue,
		m_hEvent,
		TYPE_GAMEENGINE_UPDATE
	);

	
	m_isActive = false;
	m_gameUpdateIterationsRun = 0;
	privateStashHook.EnableHook();

	// bool GAME::GameInfo::GetHardcore(void)
	dll_GameInfo_GetHardcore = (GameInfo_GetHardcore)GetProcAddressOrLogToFile(L"Engine.dll", GET_HARDCORE);
	dll_InventorySack_FindNextPosition = (InventorySack_FindNextPosition)GetProcAddressOrLogToFile(L"Game.dll", "?FindNextPosition@InventorySack@GAME@@IEBA_NPEBVItem@2@AEAVRect@2@_N@Z");

	
	if (m_isGrimDawnParsed) {
		LogToFile(LogLevel::INFO, L"Grim is parsed, displaying message..");
		DisplayMessage(L"Item Assistant", L"Item monitoring enabled");
	}
	else {
		LogToFile(LogLevel::INFO, L"Grim is not parsed, skipping message..");
	}

	LogToFile(LogLevel::INFO, L"Instaloot hook enabled");
}


InventorySack_AddItem::InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent) {
	InventorySack_AddItem::m_dataQueue = dataQueue;
	InventorySack_AddItem::m_hEvent = hEvent;
	privateStashHook = GetPrivateStash(dataQueue, hEvent);
	m_storageFolder = GetIagdFolder() + L"itemqueue\\ingoing\\";
	LogToFile(LogLevel::INFO, L"Storing instaloot items into " + m_storageFolder);

	boost::filesystem::create_directories(m_storageFolder);
	m_settingsReader = SettingsReader();
	m_stashTabLootFrom = m_settingsReader.GetStashTabToLootFrom();
	m_stashTabDepositTo = m_settingsReader.GetStashTabToDepositTo();
	m_instalootEnabled = m_settingsReader.GetPreferLegacyMode();
	m_isGrimDawnParsed = m_settingsReader.GetIsGrimDawnParsed();
	m_lastNotificationTickTime = 0;
	m_isActive = false;
	m_gameUpdateIterationsRun = 0;
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

/// <summary>
/// Allows for the activation / deactivation of instaloot when Item Assistant is not running.
/// This prevents GD from instalooting items when the IA client is closed
/// </summary>
/// <param name="isActive"></param>
void InventorySack_AddItem::SetActive(bool isActive)
{
	bool isActivating = isActive && !m_isActive;
	m_isActive = isActive;

	// IA has been shut down, start the thread now
	if (isActivating) {
		(HANDLE)_beginthread(ThreadMain, NULL, 0);
	}
}



// Since were creating from an existing object we'll need to call Get() on isHardcore and ModLabel
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_GameInfo_Param(void* This , void* info) {
	void* result = dll_GameInfo_GameInfo_Param(This, info);
	try {
		bool isHardcore = dll_GameInfo_GetHardcore(This);
		DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore_via_init, sizeof(isHardcore), (char*)&isHardcore));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in InventorySack_AddItem::Hooked_GameInfo_GameInfo_Param.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in InventorySack_AddItem::Hooked_GameInfo_GameInfo_Param.. (triple-dot)");
	}

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
	try {
		if (HandleItem(This, item)) {
			return (void*)1;
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error looting item in Hooked_InventorySack_AddItem_Vec2.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error looting item in Hooked_InventorySack_AddItem_Vec2.. (triple-dot)");
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
	try {
		if (HandleItem(This, item)) {
			return (void*)1;
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error looting item in Hooked_InventorySack_AddItem_Vec2.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error looting item in Hooked_InventorySack_AddItem_Vec2.. (triple-dot)");
	}

	return dll_InventorySack_AddItem_Vec2(This, position, item, SkipPlaySound);
}

void* __fastcall InventorySack_AddItem::Hooked_InventorySack_SetTransferOpen(void* This, bool isOpen) {
	m_isTransferStashOpen = isOpen;
	return dll_InventorySack_SetTransferOpen(This, isOpen);
}

/// <summary>
/// Checks if the provided replica info contains any of the exclusion parameters
/// We are not interested in potions, quest items, components or soulbound items.
/// </summary>
/// <param name="item"></param>
/// <returns></returns>
bool InventorySack_AddItem::IsRelevant(const GAME::ItemReplicaInfo& item) {
	if (!m_isGrimDawnParsed) {
		m_isGrimDawnParsed = m_settingsReader.GetIsGrimDawnParsed();
		if (!m_isGrimDawnParsed) {
			DisplayMessage(L"Item not looted", L"Grim Dawn not parsed");
			return false;
		} else {
			DisplayMessage(L"Item Assistant", L"Item monitoring enabled");
		}
	}
	

	if (item.stackSize > 1) {
		DisplayMessage(L"Stackable item - IA does not loot stackable items", L"Item Assistant");
		return false;
	}
	
	if (item.baseRecord.find("/storyelements/") != std::string::npos) {
		// We'll allow lokarr, but only lokarr out of storyelements items.
		if (item.baseRecord.find("records/storyelements/signs/signh.dbr") != std::string::npos) {} // Lokarr's Gaze
		else if (item.baseRecord.find("records/storyelements/signs/signf.dbr") != std::string::npos) {} // Lokarr's Boots
		else if (item.baseRecord.find("records/storyelements/signs/signs.dbr") != std::string::npos) {} // Lokarr's Mantle
		else if (item.baseRecord.find("records/storyelements/signs/signt.dbr") != std::string::npos) {} // Lokarr's Coat
		else if (item.baseRecord.find("records/storyelements/questassets/q000_torso.dbr") != std::string::npos) {} // Gazer Man
		else if (item.baseRecord.find("records/endlessdungeon/items/q001_torso.dbr") != std::string::npos) {} // Miss Gazer Man
		else {
			DisplayMessage(L"Quest item - IA does not support this specific item", L"Item Assistant");
			return false;
		}
	}

	if (item.baseRecord.find("/materia/") != std::string::npos) {
		DisplayMessage(L"Component ignored - IA does not loot components", L"Item Assistant");
		return false;
	}

	if (item.baseRecord.find("records/items/misc/") != std::string::npos) {
		DisplayMessage(L"Special item ignored - IA does not loot this item", L"Item Assistant");
		return false;
	}
	
	if (item.baseRecord.find("/questitems/") != std::string::npos) {
		DisplayMessage(L"Quest item ignored - IA does not loot quest items", L"Item Assistant");
		return false;
	}

	if (item.baseRecord.find("/crafting/") != std::string::npos) {
		DisplayMessage(L"Component ignored - IA does not loot components", L"Item Assistant");
		return false;
	}

	// Salt bag and lifegivers amulet - Frequently get questions about these
	if (item.baseRecord.find("gearaccessories/necklaces/a00_necklace.dbr") != std::string::npos 
		|| item.baseRecord.find("gearaccessories/rings/d003_ring.dbr") != std::string::npos) {
		DisplayMessage(L"Special item - This item is not supported by IA", L"Item Assistant");
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
	stream << mod.c_str() << ";" << (isHardcore ? 1 : 0)  << ";" << GAME::Serialize(replicaInfo) << "\n";
	stream.flush();
	stream.close();

	LogToFile(LogLevel::INFO, L"Storing to " + fullPath);

	std::ifstream verification;
	verification.open(fullPath);
	if (verification) {
		return true;
	}

	LogToFile(LogLevel::WARNING, L"Error: written CSV file does not exist");


	return false;
}


void InventorySack_AddItem::DisplayMessage(std::wstring text, std::wstring body) {
	const ULONGLONG now = GetTickCount64();
	try {


		// Limit notifications to 1 per 3s, roughly the fade time.
		if (now - m_lastNotificationTickTime > 3000) {
			GAME::Color color;
			color.r = 1;
			color.g = 1;
			color.b = 1;
			color.a = 1;

			// TODO: How can translation support be added?

			GAME::Engine* engine = fnGetEngine();
			if (engine == nullptr) {
				LogToFile(LogLevel::WARNING, L"Attempted to display text in-game, but no engine was set.");
				return;
			}


			LogToFile(LogLevel::INFO, L"Display: " + text + L" - " + body);

			// Checking if the game is loading here says true.. checking if its waiting crashes.. odd..
			fnShowCinematicText(engine, &text, &body, 5, &color, false);
			m_lastNotificationTickTime = now;

		} else {
			LogToFile(LogLevel::INFO, L"Muted: " + text);
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());

		LogToFile(LogLevel::WARNING, L": Encountered an exception while displaying text in-game: " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::WARNING, L"ERROR: Encountered an exception while displaying text in-game");
	}
}

std::wstring InventorySack_AddItem::GetModName(GAME::GameInfo* gameInfo) {
	std::wstring modName;
	if (fnGetGameInfoMode(gameInfo) != 1) { // Skip mod name if we're in Crucible, we don't treat that as a mod.
		fnGetModNameArg(gameInfo, &modName);
		modName.erase(std::remove(modName.begin(), modName.end(), '\r'), modName.end());
		modName.erase(std::remove(modName.begin(), modName.end(), '\n'), modName.end());
	}

	return modName;
}

/// <summary>
/// 
/// </summary>
/// <param name="stashTab">GAME::InventorySack*</param>
/// <returns>If this inventory sack is the one we wish to loot items from</returns>
bool InventorySack_AddItem::IsSackToLootFrom(void* stashTab, GAME::GameEngine* gameEngine) {
	if (gameEngine == nullptr) {
		return false;
	}

	void* realPtr = fnGetPlayerTransfer(gameEngine);
	if (realPtr == nullptr) {
		return false;
	}

	auto sacks = static_cast<std::vector<GAME::InventorySack*>*>(realPtr);

	// Not enough sacks to support running IA
	if (sacks->size() < 2) {
		return false;
	}


	// Determine the correct stash sack..
	size_t toLootFrom;
	if (m_stashTabLootFrom == 0) {
		toLootFrom = sacks->size() - 1;
	}
	else {
		// m_stashTabLootFrom is index from 1, we never want to go <0 nor >= size.
		toLootFrom = max(0,
			min(sacks->size() - 1, m_stashTabLootFrom - 1)
		);
	}

	// Is this the sack we lot items from?
	const auto lastSackPtr = sacks->at(toLootFrom);
	return static_cast<void*>(lastSackPtr) == stashTab;
}

/// <summary>
/// Attempt to classify the item as relevant <-> not relevant for looting, and pass it on for persisting.
/// </summary>
/// <param name="stash"></param>
/// <param name="item"></param>
/// <returns></returns>
bool InventorySack_AddItem::HandleItem(void* stash, GAME::Item* item) {
	if (!m_instalootEnabled || !m_isActive || stash == nullptr || item == nullptr)
		return false;

	if (!IsSackToLootFrom(stash, fnGetGameEngine())) {
		return false;
	}

	GAME::ItemReplicaInfo replica;
	fnItemGetItemReplicaInfo(item, replica);
	if (!IsRelevant(replica)) {
		return false;
	}

	GAME::Engine* engine = fnGetEngine();
	if (engine == nullptr) {
		LogToFile(LogLevel::WARNING, L"Engine is null, aborting..");
		return false;
	}
	GAME::GameInfo* gameInfo = fnGetGameInfo(engine);
	if (gameInfo == nullptr) {
		LogToFile(LogLevel::WARNING, L"GameInfo is null, aborting..");
		return false;
	}

	std::wstring modName = GetModName(gameInfo);
	
	if (Persist(replica, fnGetHardcore(gameInfo), modName)) {
		DisplayMessage(L"Item looted", L"By Item Assistant");
		fnPlayDropSound(item);
		return true;
	}

	return false;

}

/// <summary>
/// We look for CSV files that the IA client has written to a specific folder, when wanting to move items back into the game.
/// </summary>
/// <param name="modName"></param>
/// <param name="isHardcore"></param>
/// <returns></returns>
std::wstring GetFolderToLootFrom(std::wstring modName, bool isHardcore) {
	boost::property_tree::wptree loadPtreeRoot;

	std::wstring folder;
	if (modName.empty()) {
		folder = GetIagdFolder() + L"itemqueue\\outgoing\\" + (isHardcore ? L"hc" : L"sc");
	}
	else {
		folder = GetIagdFolder() + L"itemqueue\\outgoing\\" + (isHardcore ? L"hc" : L"sc") + L"\\" + modName;
	}

	if (!boost::filesystem::is_directory(folder)) {
		boost::filesystem::create_directories(folder);
	}

	return folder;
}

/// <summary>
/// When depositing items in-game, instead of deleting the CSV, we just move them into a "deleted" folder. (soft-delete)
/// It is up to the IA client to delete these CSV files in a timely manner.
/// </summary>
/// <param name="modName"></param>
/// <param name="isHardcore"></param>
/// <returns></returns>
std::wstring GetFolderToMoveTo(std::wstring modName, bool isHardcore) {
	boost::property_tree::wptree loadPtreeRoot;

	std::wstring folder;
	if (modName.empty()) {
		folder = GetIagdFolder() + L"itemqueue\\deleted\\" + (isHardcore ? L"hc" : L"sc");
	}
	else {
		folder = GetIagdFolder() + L"itemqueue\\deleted\\" + (isHardcore ? L"hc" : L"sc") + L"\\" + modName;
	}

	if (!boost::filesystem::is_directory(folder)) {
		boost::filesystem::create_directories(folder);
	}

	return folder;
}

/// <summary>
/// Read a .CSV file into a GAME::ItemReplicaInfo object
/// </summary>
/// <param name="filename">A valid CSV file</param>
/// <returns></returns>
GAME::ItemReplicaInfo* InventorySack_AddItem::ReadReplicaInfo(const std::wstring& filename) {
	try {
		std::ifstream file(filename);
		return GAME::Deserialize(GAME::GetNextLineAndSplitIntoTokens(file));
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());

		LogToFile(LogLevel::FATAL, L"ERROR Creating ReplicaItem.." + wide);

	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"ERROR Creating ReplicaItem.. (triple-dot)");
	}

	return nullptr;
}

/// <summary>
/// Returns the inventory sack to deposit items to (or nullptr on failure)
/// </summary>
/// <param name="gameEngine"></param>
/// <returns></returns>
GAME::InventorySack* InventorySack_AddItem::GetSackToDepositTo(GAME::GameEngine* gameEngine) {
	if (gameEngine == nullptr) {
		return nullptr;
	}

	void* realPtr = fnGetPlayerTransfer(gameEngine);
	if (realPtr == nullptr) {
		return nullptr;
	}

	auto sacks = static_cast<std::vector<GAME::InventorySack*>*>(realPtr);

	// Not enough sacks to even support running IA
	if (sacks->size() < 2) {
		return nullptr;
	}


	// Determine the correct stash sack..
	size_t toDepositTo;
	if (m_stashTabDepositTo == 0) {
		toDepositTo = sacks->size() - 2;
	}
	else {
		// m_stashTabLootFrom is index from 1, we never want to go <0 nor >= size.
		toDepositTo = max(0,
			min(sacks->size() - 1, m_stashTabDepositTo - 1)
		);
	}

	return sacks->at(toDepositTo);
}


/// <summary>
/// GameEngine::Update() hook
/// Responsible for moving items from .csv and back into the game.
/// </summary>
/// <param name="This"></param>
/// <param name="notUsed"></param>
/// <param name="s"></param>
/// <param name="f"></param>
/// <param name="b"></param>
/// <param name="f2"></param>
/// <returns></returns>
void* __fastcall InventorySack_AddItem::Hooked_GameEngine_Update(void* This, int v) {
	try {
		// IA not running? Continue
		if (!m_isActive) {
			//LogToFile(L"Debug: NotActive");
			return dll_GameEngine_Update(This, v);
		}

		// If the game is not in a a "ready state", just continue.
		if (IsGameLoading(This) || IsGameWaiting(This, true) || !IsGameEngineOnline(This)) {
			//LogToFile(L"Debug: NotReady");
			m_isTransferStashOpen = false; // Just to be on the safe side
			return dll_GameEngine_Update(This, v);
		}

		// No need to check *constantly* (at least not until we get a thread here)
		if (++m_gameUpdateIterationsRun < 30) {
			//LogToFile(L"Debug: NotIteration");
			return dll_GameEngine_Update(This, v);
		}

		m_gameUpdateIterationsRun = 0;

		auto engine = fnGetEngine();
		if (engine == nullptr) {
			LogToFile(LogLevel::INFO, L"Debug: NoEngine");
			return dll_GameEngine_Update(This, v);
		}

		GAME::GameInfo* gameInfo = fnGetGameInfo(engine);
		if (gameInfo == nullptr) {
			LogToFile(LogLevel::WARNING, L"GameInfo is null, aborting..");
			return false;
		}

		if (m_isTransferStashOpen) {
			void* sackPtr = GetSackToDepositTo((GAME::GameEngine*)This);
			if (sackPtr != nullptr) {
				GAME::Rect itemPosition;
				boost::lock_guard<boost::mutex> guard(m_mutex);


				bool success = false;
				std::wstring targetFolder = GetFolderToMoveTo(GetModName(gameInfo), fnGetHardcore(gameInfo));
				for (auto it = m_depositQueue.begin(); it != m_depositQueue.end(); ++it) {
					std::wstring targetFile = targetFolder + L"\\" + randomFilename();
					LogToFile(LogLevel::INFO, L"Handling file " + *it);

					GAME::ItemReplicaInfo* replica = ReadReplicaInfo(*it);
					if (replica != nullptr) {
						//LogToFile(L"DEBUG Creating item from replica..");
						try {
							auto item = fnCreateItem(replica);
							//LogToFile(L"DEBUG Adding item to inventory sack..");
							if (item == nullptr) {
								LogToFile(LogLevel::FATAL, L"Error creating item, re-depositing back into IA. (Mod item transferred into vanilla?)");
								targetFile = m_storageFolder + randomFilename();
								LogToFile(LogLevel::INFO, L"Moving to " + targetFile);
							}
							else {
								if (dll_InventorySack_FindNextPosition(sackPtr, item, &itemPosition, true)) {
									dll_InventorySack_AddItem_Vec2(sackPtr, (void*)&itemPosition, item, false);
									LogToFile(LogLevel::INFO, L"Item deposited, moving to " + targetFile);
									success = true;
								}
								else {
									targetFile = m_storageFolder + randomFilename();
									LogToFile(LogLevel::INFO, L"Target sack is full, re-depositing to IA as " + targetFile);
								}

							}
						}
						catch (...) {
							LogToFile(LogLevel::FATAL, L"Invalid item, moving to " + targetFile);
						}
						delete replica;

					}
					else {
						LogToFile(LogLevel::INFO, L"Invalid item, moving to " + targetFile);
					}

					if (!MoveFile(it->c_str(), targetFile.c_str())) {
						LogToFile(LogLevel::WARNING, L"Failed moving file: \"" + *it + L"\" to \"" + targetFile + L"\", error code: " + std::to_wstring(GetLastError()));
					}
				}


				if (!m_depositQueue.empty()) {
					if (success) {
						if (m_depositQueue.size() == 1) {
							DisplayMessage(L"An item was deposited", L"By Item Assistant");
						}
						else {
							DisplayMessage(m_depositQueue.size() + L" items were deposited", L"By Item Assistant");
						}

						// Sort the items, as we've deposited them all in position 1,1
						SortInventorySack(sackPtr, 1);
					}
					else {
						DisplayMessage(L"Could not transfer item, moved back to IA.", L"By Item Assistant");

					}
					// fnPlayDropSound(item);


					// We got a mutex so this is safe (and if it wasn't, we'd just loot it next time IA starts)
					m_depositQueue.clear();
				}

			}
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in InventorySack_AddItem::Hooked_GameEngine_Update.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in InventorySack_AddItem::Hooked_GameEngine_Update.. (triple-dot)");
	}

	return dll_GameEngine_Update(This, v);
}

/// <summary>
/// Looks for new files added to the current itemqueue\outgoing\sc-hc\modname folder, to deposit into the game.
/// Files found are added to a std::set locked by a mutex, read by GameEngine::update()
/// </summary>
/// <param name=""></param>
void InventorySack_AddItem::ThreadMain(void*) {
	LogToFile(LogLevel::INFO, L"IA is running, starting deposit listener..");
	try {
		std::set<std::wstring> knownFiles = std::set<std::wstring>();
		while (m_isActive) {
			Sleep(500);

			auto engine = fnGetEngine(true);
			if (engine == nullptr) {
				LogToFile(LogLevel::INFO, L"Debug: NoEngine");
				continue;
			}

			GAME::GameInfo* gameInfo = fnGetGameInfo(engine);
			if (gameInfo == nullptr) {
				LogToFile(LogLevel::INFO, L"GameInfo is null, aborting..");
				continue;
			}

			std::wstring folder = GetFolderToLootFrom(GetModName(gameInfo), fnGetHardcore(gameInfo, true));
			// LogToFile(std::wstring(L"Looking for files in dir: ") + folder);

			for (auto& entry : boost::make_iterator_range(boost::filesystem::directory_iterator(folder), {})) {
				auto filename = std::wstring(entry.path().c_str());
				if (knownFiles.find(filename) == knownFiles.end()) {
					boost::lock_guard<boost::mutex> guard(m_mutex);

					if (boost::algorithm::ends_with(filename, ".csv")) {
						LogToFile(LogLevel::INFO, std::wstring(L"Found file: ") + std::wstring(entry.path().c_str()));
						m_depositQueue.insert(filename);
					}
					else {
						LogToFile(LogLevel::INFO, std::wstring(L"Ignoring file: ") + std::wstring(entry.path().c_str()));
					}
					knownFiles.insert(filename);
				}
			}
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in InventorySack_AddItem::ThreadMain.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in InventorySack_AddItem::ThreadMain.. (triple-dot)");
	}
	LogToFile(LogLevel::INFO, L"Stopping deposit listener..");
}
