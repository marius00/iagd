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
void LogToFile(const wchar_t* message);
void LogToFile(std::wstring message);

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

std::map<std::wstring, std::set<std::wstring>> InventorySack_AddItem::m_depositQueue;

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

	
	if (m_isGrimDawnParsed)
		DisplayMessage(L"Item Assistant", L"Item monitoring enabled");
}


InventorySack_AddItem::InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent) {
	InventorySack_AddItem::m_dataQueue = dataQueue;
	InventorySack_AddItem::m_hEvent = hEvent;
	privateStashHook = GetPrivateStash(dataQueue, hEvent);
	m_storageFolder = GetIagdFolder() + L"itemqueue\\ingoing\\";
	LogToFile(L"Storing instaloot items into " + m_storageFolder);

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
		// (HANDLE)_beginthread(ThreadMain, NULL, 0);
	}
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
		else {
			DisplayMessage(L"Quest item - IA does not support this specific item", L"Item Assistant");
			return false;
		}
	}

	if (item.baseRecord.find("/materia/") != std::string::npos) {
		DisplayMessage(L"Component ignored - IA does not loot components", L"Item Assistant");
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
		|| item.baseRecord.find("questassets/q000_torso.dbr") != std::string::npos) {
		DisplayMessage(L"Special item - This item is not supported by IA", L"Item Assistant");
		return false;
	}

	// Transmute - Should be impossible, but never know..
	if (!item.enchantmentRecord.empty()) {
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

	LogToFile(L"Storing to " + fullPath);

	std::ifstream verification;
	verification.open(fullPath);
	if (verification) {
		return true;
	}

	LogToFile(L"Error: written CSV file does not exist");


	return false;
}


void InventorySack_AddItem::DisplayMessage(std::wstring text, std::wstring body) {
	const ULONGLONG now = GetTickCount64();

	// Limit notifications to 1 per 3s, roughly the fade time.
	if (now - m_lastNotificationTickTime > 3000) {
		GAME::Color color;
		color.r = 1;
		color.g = 1;
		color.b = 1;
		color.a = 1;

		// TODO: How can translation support be added?

		GAME::Engine* engine = fnGetEngine();
		fnShowCinematicText(engine, &text, &body, 5, &color);
		m_lastNotificationTickTime = now;

		LogToFile(L"Display: " + text);
	} else {
		LogToFile(L"Muted: " + text);
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
	void* realPtr = fnGetPlayerTransfer(gameEngine);
	if (realPtr == nullptr) {
		return false;
	}

	auto sacks = static_cast<std::vector<GAME::InventorySack*>*>(realPtr);

	// Not enough sacks to even support running IA
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
	if (!m_instalootEnabled || !m_isActive)
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
		LogToFile(L"Engine is null, aborting..");
		return false;
	}
	GAME::GameInfo* gameInfo = fnGetGameInfo(engine);
	if (gameInfo == nullptr) {
		LogToFile(L"GameInfo is null, aborting..");
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
std::wstring GetFolderToLootFrom(bool isHardcore) {
	boost::property_tree::wptree loadPtreeRoot;

	std::wstring folder;
	folder = GetIagdFolder() + L"itemqueue\\outgoing\\" + (isHardcore ? L"hc" : L"sc");

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

		LogToFile(L"ERROR Creating ReplicaItem.." + wide);

	}
	catch (...) {
		LogToFile(L"ERROR Creating ReplicaItem.. (triple-dot)");
	}

	return nullptr;
}

/// <summary>
/// Returns the inventory sack to deposit items to (or nullptr on failure)
/// </summary>
/// <param name="gameEngine"></param>
/// <returns></returns>
GAME::InventorySack* InventorySack_AddItem::GetSackToDepositTo(GAME::GameEngine* gameEngine) {
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
	// IA not running? Continue
	if (!m_isActive) {
		LogToFile(L"Debug: NotActive");
		return dll_GameEngine_Update(This, v);
	}

	// If the game is not in a a "ready state", just continue.
	if (IsGameLoading(This) || IsGameWaiting(This, true) || !IsGameEngineOnline(This)) {
		LogToFile(L"Debug: NotReady");
		m_isTransferStashOpen = false; // Just to be on the safe side
		return dll_GameEngine_Update(This, v);
	}

	// No need to check *constantly* (at least not until we get a thread here)
	if (++m_gameUpdateIterationsRun < 30) {
		//LogToFile(L"Debug: NotIteration");
		return dll_GameEngine_Update(This, v);
	}

	m_gameUpdateIterationsRun = 0;
	LogToFile(L"Debug: Proceeding #1");

	auto engine = fnGetEngine();
	if (engine == nullptr) {
		LogToFile(L"Debug: NoEngine");
		return dll_GameEngine_Update(This, v);
	}

	GAME::GameInfo* gameInfo = fnGetGameInfo(engine);
	if (gameInfo == nullptr) {
		LogToFile(L"GameInfo is null, aborting..");
		return false;
	}

	LogToFile(L"Debug: Proceeding #2");

	if (m_isTransferStashOpen) {
		void* sackPtr = GetSackToDepositTo((GAME::GameEngine*)This);
		if (sackPtr != nullptr) {
			GAME::Rect itemPosition;
			boost::lock_guard<boost::mutex> guard(m_mutex);


			bool success = false;
			auto modName = GetModName(gameInfo);
			std::wstring targetFolder = GetFolderToMoveTo(modName, fnGetHardcore(gameInfo));

			if (m_depositQueue.find(modName) == m_depositQueue.end()) {
				return dll_GameEngine_Update(This, v);
			}
			
			// Find the queue for the current mod
			auto currentDepositQueue = m_depositQueue.find(modName)->second;
			for (auto it = currentDepositQueue.begin(); it != currentDepositQueue.end(); ++it) {
				std::wstring targetFile = targetFolder + L"\\" + randomFilename();
				LogToFile(L"Handling file " + *it);

				GAME::ItemReplicaInfo* replica = ReadReplicaInfo(*it);
				if (replica != nullptr) {
					//LogToFile(L"DEBUG Creating item from replica..");
					try {
						auto item = fnCreateItem(replica);
						//LogToFile(L"DEBUG Adding item to inventory sack..");
						if (item == nullptr) {
							LogToFile(L"Error creating item, re-depositing back into IA. (Mod item transferred into vanilla?)");
							targetFile = m_storageFolder + randomFilename();
							LogToFile(L"Moving to " + targetFile);
						}
						else {
							if (dll_InventorySack_FindNextPosition(sackPtr, item, &itemPosition, true)) {
								dll_InventorySack_AddItem_Vec2(sackPtr, (void*)&itemPosition, item, false);
								LogToFile(L"Item deposited, moving to " + targetFile);
								success = true;
							}
							else {
								targetFile = m_storageFolder + randomFilename();
								LogToFile(L"Target sack is full, re-depositing to IA as " + targetFile);
							}

						}
					}
					catch (...) {
						LogToFile(L"Invalid item, moving to " + targetFile);
					}
					delete replica;

				}
				else {
					LogToFile(L"Invalid item, moving to " + targetFile);
				}

				if (!MoveFile(it->c_str(), targetFile.c_str())) {
					LogToFile(L"Failed moving file: \"" + *it + L"\" to \"" + targetFile + L"\", error code: " + std::to_wstring(GetLastError()));
				}
			}


			if (!currentDepositQueue.empty()) {
				if (success) {
					if (currentDepositQueue.size() == 1) {
						DisplayMessage(L"An item was deposited", L"By Item Assistant");
					}
					else {
						DisplayMessage(currentDepositQueue.size() + L" items were deposited", L"By Item Assistant");
					}

					// Sort the items, as we've deposited them all in position 1,1
					SortInventorySack(sackPtr, 1);
				}
				else {
					DisplayMessage(L"Could not transfer item, moved back to IA.", L"By Item Assistant");

				}
				// fnPlayDropSound(item);


				// We got a mutex so this is safe (and if it wasn't, we'd just loot it next time IA starts)
				currentDepositQueue.clear();
			}

		}
	}

	return dll_GameEngine_Update(This, v);
}


/// <summary>
/// Looks for new files added to the current itemqueue\outgoing\sc-hc\modname folder, to deposit into the game.
/// Files found are added to a std::set locked by a mutex, read by GameEngine::update()
/// </summary>
/// <param name=""></param>
void InventorySack_AddItem::ThreadMain(void*) {
	LogToFile(L"IA is running, starting deposit listener..");
	std::set<std::wstring> knownFiles = std::set<std::wstring>();
	while (m_isActive) {
		Sleep(1);

		// Check both "sc" and "hc"
		for (int isHardcore = 0; isHardcore <= 1; isHardcore++) {
			std::wstring folder = GetFolderToLootFrom(isHardcore != 0);
			 LogToFile(std::wstring(L"Looking for files in dir: ") + folder);

			for (auto& queuedItemFile : boost::make_iterator_range(boost::filesystem::directory_iterator(folder), {})) {
				bool isDirectory = boost::filesystem::is_directory(queuedItemFile.path());
				auto filename = std::wstring(queuedItemFile.path().c_str());

				// If this is a directory, then it's a mod. (\sc\modname\itemfile.csv)
				if (isDirectory) {
					for (auto& queuedModItemFile : boost::make_iterator_range(boost::filesystem::directory_iterator(queuedItemFile.path()), {})) {
						// Nested directory? No idea, user is messing around? Not expected to be here.
						bool isDirectory = boost::filesystem::is_directory(queuedModItemFile.path());
						boost::filesystem::path p(queuedItemFile.path());
						

						auto filename = std::wstring(queuedModItemFile.path().c_str());
						if (!isDirectory) {
							if (knownFiles.find(filename) == knownFiles.end()) {
								boost::lock_guard<boost::mutex> guard(m_mutex);

								if (boost::algorithm::ends_with(filename, ".csv")) {
									LogToFile(std::wstring(L"Found file (" + p.filename().wstring() + L"): ") + std::wstring(filename));

									if (m_depositQueue.find(p.filename().wstring()) == m_depositQueue.end()) {
										m_depositQueue[p.filename().wstring()] = std::set<std::wstring>();
									}

									m_depositQueue.find(p.filename().wstring())->second.insert(filename);
								}
								else {
									LogToFile(std::wstring(L"Ignoring file (" + p.filename().wstring() + L"): ") + std::wstring(filename));
								}
								knownFiles.insert(filename);
							}
						}
						else {
							LogToFile(std::wstring(L"Ignoring nested folder: ") + std::wstring(filename));
						}
					}
				}
				else {
					if (knownFiles.find(filename) == knownFiles.end()) {
						boost::lock_guard<boost::mutex> guard(m_mutex);

						if (boost::algorithm::ends_with(filename, ".csv")) {
							LogToFile(std::wstring(L"Found file (vanilla): ") + std::wstring(filename));

							if (m_depositQueue.find(L"") == m_depositQueue.end()) {
								m_depositQueue[L""] = std::set<std::wstring>();
							}

							m_depositQueue.find(L"")->second.insert(filename);
						}
						else {
							LogToFile(std::wstring(L"Ignoring file (vanilla): ") + std::wstring(filename));
						}
						knownFiles.insert(filename);
					}
				}
			}
		}
	}
	LogToFile(L"Stopping deposit listener..");
}
