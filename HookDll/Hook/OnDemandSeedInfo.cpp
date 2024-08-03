#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "OnDemandSeedInfo.h"
#include "Exports.h"
#include <boost/property_tree/ptree.hpp>                                        
#include <boost/property_tree/json_parser.hpp>       
#include <boost/filesystem.hpp>
#include <boost/range/iterator_range.hpp>
#include <boost/algorithm/string/predicate.hpp>
#include <iostream>
#include <boost/algorithm/string.hpp> 

#include <codecvt> // wstring_convert

#include "Logger.h"
std::wstring GetIagdFolder();

OnDemandSeedInfo::pItemEquipmentGetUIDisplayText OnDemandSeedInfo::fnItemEquipmentGetUIDisplayText;
OnDemandSeedInfo* OnDemandSeedInfo::g_self;
OnDemandSeedInfo::OnDemandSeedInfo() {}
OnDemandSeedInfo::OnDemandSeedInfo(DataQueue* dataQueue, HANDLE hEvent) {
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
	m_thread = NULL;
	m_sleepMilliseconds = 0;

	auto handle = GetProcAddressOrLogToFile(L"game.dll", "?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z");
	if (handle == NULL) LogToFile(LogLevel::FATAL, L"Error hooking IsGameLoading@GameEngine");
	fnItemEquipmentGetUIDisplayText = pItemEquipmentGetUIDisplayText(handle);
}



void OnDemandSeedInfo::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		"?Update@GameEngine@GAME@@QEAAXH@Z",
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_GAMEENGINE_UPDATE
	);
}
void OnDemandSeedInfo::DisableHook() {
	Stop();
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

/*
* Continuously listen for new events on the pipe
*/
void OnDemandSeedInfo::ThreadMain(void*) {

	LogToFile(LogLevel::INFO, L"Seed info thread started, sleeping for 6s..");
	try {

		// When spamming too much right as the game first loads, the game tends to crash
		// This is suboptimal, but it is what it is..
		g_self->m_sleepMilliseconds = 6000;

		LogToFile(LogLevel::INFO, L"Seed info thread ready, starting loop");
		while (g_self->m_isActive) {

			// The "m_sleepMilliseconds" is actually a counter read in the Update() method on a different thread. Letting us back off from doing too much in the update thread.
			while (g_self->m_sleepMilliseconds > 0) {
				//LogToFile(LogLevel::INFO, L"Sleeping for 100ms.. " + std::to_wstring(g_self->m_sleepMilliseconds) + L"ms remaining");
				Sleep(100);
				g_self->m_sleepMilliseconds -= 100;

				if (!g_self->m_isActive) {
					LogToFile(LogLevel::INFO, L"No longer running, cancelling sleep");
					return;
				}
			}

			g_self->Process();
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in OndemandSeedInfo::ThreadMain.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in OndemandSeedInfo::ThreadMain.. (triple-dot)");
	}
}

/*
* Stop the running thread
*/
void OnDemandSeedInfo::Stop() {
	m_isActive = false;
	if (m_thread != NULL) {
		CloseHandle(m_thread);
		m_thread = NULL;
	}
}


/*
* Create the named pipe and start a thread to listen for events
*/
void OnDemandSeedInfo::Start() {
	g_self = this;

	LogToFile(LogLevel::INFO, L"Queuing thread start for seed info..");
	m_isActive = true;
	m_thread = (HANDLE)_beginthread(ThreadMain, NULL, 0);
}



ParsedSeedRequest* OnDemandSeedInfo::DeserializeReplicaCsv(std::vector<std::string> tokens) {
	if (tokens.size() != 12) {
		LogToFile(LogLevel::WARNING, L"Error parsing CSV file, expected 12 tokens, got " + std::to_wstring(tokens.size()));
		return nullptr;
	}

	GAME::ItemReplicaInfo item;
	ParsedSeedRequest* result = new ParsedSeedRequest();

	
	int idx = 0;
	int type = stoul(tokens.at(idx++));
	if (type == 1) {
		result->playerItemId = (unsigned int)stoul(tokens.at(idx++));
	}
	else if (type == 2) {
		result->buddyItemId = tokens.at(idx++);
	}
	else {
		return nullptr;
	}

	item.seed = (unsigned int)stoul(tokens.at(idx++));
	item.relicSeed = (unsigned int)stoul(tokens.at(idx++));
	item.enchantmentSeed = (unsigned int)stoul(tokens.at(idx++));

	item.baseRecord = tokens.at(idx++);
	item.prefixRecord = tokens.at(idx++);
	item.suffixRecord = tokens.at(idx++);

	item.modifierRecord = tokens.at(idx++);
	item.materiaRecord = tokens.at(idx++);
	item.enchantmentRecord = tokens.at(idx++);
	item.transmuteRecord = tokens.at(idx++);

	std::string s;
	for (auto it = tokens.begin(); it != tokens.end(); ++it) {
		s = s + *it + ";";
	}
	//LogToFile(LogLevel::INFO, "READ: " + s);
	
	boost::algorithm::trim(item.baseRecord);
	boost::algorithm::trim(item.prefixRecord);
	boost::algorithm::trim(item.suffixRecord);
	boost::algorithm::trim(item.modifierRecord);
	boost::algorithm::trim(item.materiaRecord);
	boost::algorithm::trim(item.enchantmentRecord);
	boost::algorithm::trim(item.transmuteRecord);

	result->itemReplicaInfo = item;

	return result;
}

/// <summary>
/// Read a .CSV file into a GAME::ItemReplicaInfo object
/// </summary>
/// <param name="filename">A valid CSV file</param>
/// <returns></returns>
ParsedSeedRequest* OnDemandSeedInfo::ReadReplicaInfo(const std::wstring& filename) {
	try {
		std::ifstream file(filename);
		return DeserializeReplicaCsv(GAME::GetNextLineAndSplitIntoTokens(file));
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
/// We look for CSV files that the IA client has written to a specific folder, when wanting to move items back into the game.
/// </summary>
/// <param name="modName"></param>
/// <param name="isHardcore"></param>
/// <returns></returns>
std::wstring GetFolderToReadFrom(std::wstring modName, bool isHardcore) {
	boost::property_tree::wptree loadPtreeRoot;

	std::wstring folder;
	if (modName.empty()) {
		folder = GetIagdFolder() + L"replica\\from_ia\\" + (isHardcore ? L"hc" : L"sc");
	}
	else {
		folder = GetIagdFolder() + L"replica\\from_ia\\" + (isHardcore ? L"hc" : L"sc") + L"\\" + modName;
	}

	if (!boost::filesystem::is_directory(folder)) {
		boost::filesystem::create_directories(folder);
	}

	return folder;
}

std::wstring OnDemandSeedInfo::GetModName(GAME::GameInfo* gameInfo) {
	std::wstring modName;
	if (fnGetGameInfoMode(gameInfo) != 1) { // Skip mod name if we're in Crucible, we don't treat that as a mod.
		fnGetModNameArg(gameInfo, &modName);
		modName.erase(std::remove(modName.begin(), modName.end(), '\r'), modName.end());
		modName.erase(std::remove(modName.begin(), modName.end(), '\n'), modName.end());
	}

	return modName;
}

/*
* Process a single request on the named pipe
*/
void OnDemandSeedInfo::Process() {
	try {
		boost::filesystem::create_directories(GetIagdFolder() + L"replica\\to_ia\\");

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

			std::wstring folder = GetFolderToReadFrom(GetModName(gameInfo), fnGetHardcore(gameInfo, true));

			for (auto& entry : boost::make_iterator_range(boost::filesystem::directory_iterator(folder), {})) {
				auto filename = std::wstring(entry.path().c_str());
				if (knownFiles.find(filename) == knownFiles.end()) {

					if (boost::algorithm::ends_with(filename, ".csv")) {
						LogToFile(LogLevel::INFO, std::wstring(L"Queued file: ") + std::wstring(entry.path().c_str()));

						ParsedSeedRequest* obj = ReadReplicaInfo(entry.path().c_str());
						if (obj == nullptr) {
							LogToFile(LogLevel::WARNING, std::wstring(L"Ignoring file, got nullptr when deserializing: ") + std::wstring(entry.path().c_str()));
						}
						else {
							ParsedSeedRequestPtr abc(obj);
							m_itemQueue.push(abc);
						}
					}
					else {
						LogToFile(LogLevel::INFO, std::wstring(L"Ignoring file: ") + std::wstring(entry.path().c_str()));
					}
					knownFiles.insert(filename); // TODO: Still needed?

					DeleteFile(filename.c_str());

					if (m_itemQueue.size() > 20) {
						Sleep(3500);
					}
				}
			}
		}
	}
	catch (std::exception& ex) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(ex.what());
		LogToFile(LogLevel::FATAL, L"Error parsing in OnDemandSeedInfo::ThreadMain.. " + wide);
	}
	catch (...) {
		LogToFile(LogLevel::FATAL, L"Error parsing in OnDemandSeedInfo::ThreadMain.. (triple-dot)");
	}
	LogToFile(LogLevel::INFO, L"Stopping deposit listener..");
}

std::string toString(std::wstring s) {
	using convert_type = std::codecvt_utf8<wchar_t>;
	std::wstring_convert<convert_type, wchar_t> converter;
	return converter.to_bytes(s);
}


std::string toJson(ParsedSeedRequest obj, std::vector<GAME::GameTextLine>& gameTextLines) {
	boost::property_tree::ptree root;
	root.put("playerItemId", obj.playerItemId);
	root.put("buddyItemId", obj.buddyItemId.c_str());



	boost::property_tree::ptree stats;
	for (auto& it : gameTextLines) {
		boost::property_tree::ptree stat;

		stat.put<std::string>("text", toString(it.text));
		stat.put("type", it.textClass);
		stats.push_back(std::make_pair("", stat));
	}

	root.add_child("stats", stats);

	std::stringstream json;
	boost::property_tree::write_json(json, root);
	return json.str();
}

// TODO: Either rename, or make this method do less. Probably the latter
void OnDemandSeedInfo::GetItemInfo(ParsedSeedRequest obj) {
	// Check for access to Game.dll
	if (GetModuleHandleA("Game.dll")) {
		GAME::ItemReplicaInfo replica = obj.itemReplicaInfo;
		GAME::Item* newItem = fnCreateItem(&replica);
		if (newItem) {
			std::vector<GAME::GameTextLine> gameTextLines = {};

			auto gameEngine = fnGetGameEngine();
			if (gameEngine == nullptr) {
				LogToFile(LogLevel::INFO, "No game engine, skipping item stat generation");
				DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOGAME, 0, nullptr));
				m_dataQueue->push(item);
				SetEvent(m_hEvent);
				return;
			}

			GAME::Character* character = (GAME::Character*)fnGetMainPlayer(gameEngine);
			if (character == nullptr) {
				LogToFile(LogLevel::INFO, "No character found, skipping item stat generation");
				DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOGAME, 0, nullptr));
				m_dataQueue->push(item);
				SetEvent(m_hEvent);
				return;
			}

			// TODO: We should fetch this earlier, ensure we don't get the hooked method. -- We seem to be getting 4 replies. 4th one is the message below. 
			// First is probably in Item:: then ItemEquipment:: (both have hooks), 
			// Sender is responsible for ensuring that this is NOT as set item, not a potion/scroll/other and not a relic. Eg must be equipment which is not part of a set.
			fnItemEquipmentGetUIDisplayText((GAME::ItemEquipment*)newItem, character, &gameTextLines, true);
			fnDestroyObjectEx(fnGetObjectManager(), (GAME::Object*)newItem, nullptr, 0);


			LogToFile(LogLevel::INFO, L"Generating json..");
			
			std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
			std::wstring buddyItemId = converter.from_bytes(obj.buddyItemId);
			std::wstring fullPath = GetIagdFolder() + L"replica\\to_ia\\" + std::to_wstring(obj.playerItemId) + buddyItemId + L".json";
			std::wofstream stream;
			stream.open(fullPath);
			stream << toJson(obj, gameTextLines).c_str();
			stream.flush();
			stream.close();
			LogToFile(LogLevel::INFO, L"Wrote items stats to " + fullPath);
		}
		else {
			std::string str = obj.itemReplicaInfo.baseRecord;
			DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOITEM, str.size(), (char*)str.c_str()));
			m_dataQueue->push(item);
			SetEvent(m_hEvent);
		}
	}
	else {
		DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOGAME, 0, nullptr));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
}



void* __fastcall OnDemandSeedInfo::HookedMethod(void* This, int v) {
	void* r = g_self->originalMethod(This, v);

	// Only start processing items if the game is running.
	// Attempting to create items with a set bonus from the main menu may crash the game.
	// Items with skills may also end up with missing info if created from the main menu.
	/*GAME::GameEngine* gameEngine = fnGetGameEngine();
	bool isGameLoading = IsGameLoading(gameEngine);
	bool isGameWaiting = IsGameWaiting(gameEngine, true);
	bool isGameEngineOnline = IsGameEngineOnline(gameEngine);
	*/

	
	if (g_self->m_sleepMilliseconds <= 0) {
		try {
			bool isGameLoading = IsGameLoading(This);
			///bool isGameWaiting = IsGameWaiting(This, true);
			bool isGameEngineOnline = IsGameEngineOnline(This);

			if (!isGameLoading /* && !isGameWaiting*/ && isGameEngineOnline) {

				// Process the queue
				int num = 0;

				while (!g_self->m_itemQueue.empty() && num++ < 5) {
					LogToFile(LogLevel::INFO, L"Processing..");
					ParsedSeedRequestPtr ptr = g_self->m_itemQueue.pop();
					if (ptr == nullptr) {
						return r;
					}
					ParsedSeedRequest obj = *ptr.get();

					LogToFile(LogLevel::INFO, L"Fetching items stats for " + GAME::Serialize(obj.itemReplicaInfo));
					g_self->GetItemInfo(obj);
				}
			}
			else {
				if (isGameLoading) {
					LogToFile(LogLevel::INFO, "Game is loading, real stat generation not available.");
				}
				/*if (isGameWaiting) {
					LogToFile(LogLevel::INFO, "Game is waiting, real stat generation not available.");
				}*/
				if (!isGameEngineOnline) {
					LogToFile(LogLevel::INFO, "///Game engine is not online, real stat generation not available.");
				}
				g_self->m_sleepMilliseconds = 12000;
			}
		}
		catch (std::exception& ex) {
			std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
			std::wstring wide = converter.from_bytes(ex.what());
			LogToFile(LogLevel::FATAL, L"Error parsing on-demand item stats.. " + wide);
		}
		catch (...) {
			LogToFile(LogLevel::FATAL, L"Error parsing on-demand item stats.. (triple-dot)");
		}

	}

	return r;
}