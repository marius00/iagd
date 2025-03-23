#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <random>
#include <stdlib.h>
#include "MessageType.h"
#include "OnDemandSeedInfo.h"
#include "Exports.h"
#include <codecvt> // wstring_convert

#include "Logger.h"
std::wstring GetIagdFolder();

OnDemandSeedInfo::pItemEquipmentGetUIDisplayText OnDemandSeedInfo::fnItemEquipmentGetUIDisplayText;
OnDemandSeedInfo::pItemEquipmentGetUIDisplayText OnDemandSeedInfo::fnItemRelicGetUIDisplayText;
OnDemandSeedInfo* OnDemandSeedInfo::g_self;
OnDemandSeedInfo::OnDemandSeedInfo() {}
OnDemandSeedInfo::OnDemandSeedInfo(DataQueue* dataQueue, HANDLE hEvent) {
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
	m_thread = nullptr;
	m_sleepMilliseconds = 0;
	originalMethod = nullptr;

	auto handle = GetProcAddressOrLogToFile(L"game.dll", "?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z");
	if (handle == nullptr) LogToFile(LogLevel::FATAL, L"Error hooking GetUIDisplayText@ItemEquipment");
	fnItemEquipmentGetUIDisplayText = pItemEquipmentGetUIDisplayText(handle);


	auto handle2 = GetProcAddressOrLogToFile(L"game.dll", GET_ITEMARTIFACT_GETUIDISPLAYTEXT);
	if (handle2 == nullptr) LogToFile(LogLevel::FATAL, L"Error hooking GetUIDisplayText@ItemArtifact");
	fnItemRelicGetUIDisplayText = pItemEquipmentGetUIDisplayText(handle2);
}



void OnDemandSeedInfo::EnableHook() {
	g_self = this;
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
	if (tokens.size() != 12 && tokens.size() != 15) {
		LogToFile(LogLevel::WARNING, L"Error parsing CSV file, expected 12 or 15 tokens, got " + std::to_wstring(tokens.size()));
		return nullptr;
	}

	bool isNewDlc = tokens.size() == 15;

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

	if (isNewDlc) {
		auto rerollsUsed = (unsigned int)stoul(tokens.at(idx++));
	}

	item.baseRecord = tokens.at(idx++);
	item.prefixRecord = tokens.at(idx++);
	item.suffixRecord = tokens.at(idx++);

	item.modifierRecord = tokens.at(idx++);
	item.materiaRecord = tokens.at(idx++);
	item.enchantmentRecord = tokens.at(idx++);
	item.transmuteRecord = tokens.at(idx++);

	if (isNewDlc) {
		auto ascendantAffixNameRecord  = tokens.at(idx++);
		auto ascendantAffix2hNameRecord= tokens.at(idx++);
	}

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
	// TODO: !! Trip ascendant records

	result->itemReplicaInfo = item;


	// Relics have a different function it needs called
	result->isRelic = item.baseRecord.find("/gearrelic/") != std::string::npos;

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
		folder = GetIagdFolder() + L"replica\\from_ia";
	}
	else {
		folder = GetIagdFolder() + L"replica\\from_ia\\" + modName;
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

				DeleteFile(filename.c_str());

				if (m_itemQueue.size() > 20) {
					Sleep(1);
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


boost::property_tree::ptree toJson(ParsedSeedRequest obj, std::vector<GAME::GameTextLine>& gameTextLines) {
	boost::property_tree::ptree root;
	root.put("playerItemId", obj.playerItemId);
	root.put("buddyItemId", obj.buddyItemId.c_str());

	// Completely redundant information, might help others to use this DLL.
	boost::property_tree::ptree replica;
	replica.put("baseRecord", obj.itemReplicaInfo.baseRecord);
	replica.put("prefixRecord", obj.itemReplicaInfo.prefixRecord);
	replica.put("suffixRecord", obj.itemReplicaInfo.suffixRecord);
	replica.put("modifierRecord", obj.itemReplicaInfo.modifierRecord);
	replica.put("transmuteRecord", obj.itemReplicaInfo.transmuteRecord);
	replica.put("seed", obj.itemReplicaInfo.seed);
	replica.put("materiaRecord", obj.itemReplicaInfo.materiaRecord);
	replica.put("relicBonus", obj.itemReplicaInfo.relicBonus);
	replica.put("relicSeed", obj.itemReplicaInfo.relicSeed);
	replica.put("enchantmentRecord", obj.itemReplicaInfo.enchantmentRecord);
	replica.put("enchantmentSeed", obj.itemReplicaInfo.enchantmentSeed);
	root.add_child("replica", replica);
	// TODO: ascendant changes here, not critical, not used by IAGD


	boost::property_tree::ptree stats;
	for (auto& it : gameTextLines) {
		boost::property_tree::ptree stat;

		stat.put<std::string>("text", toString(it.text));
		stat.put("type", it.textClass);
		stats.push_back(std::make_pair("", stat));
	}

	root.add_child("stats", stats);

	return root;
}

// TODO: Either rename, or make this method do less. Probably the latter
boost::property_tree::ptree OnDemandSeedInfo::GetItemInfo(ParsedSeedRequest obj) {
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
				return boost::property_tree::ptree();
			}

			GAME::Character* character = (GAME::Character*)fnGetMainPlayer(gameEngine);
			if (character == nullptr) {
				LogToFile(LogLevel::INFO, "No character found, skipping item stat generation");
				DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOGAME, 0, nullptr));
				m_dataQueue->push(item);
				SetEvent(m_hEvent);
				return boost::property_tree::ptree();
			}

			// TODO: We should fetch this earlier, ensure we don't get the hooked method. -- We seem to be getting 4 replies. 4th one is the message below. 
			// First is probably in Item:: then ItemEquipment:: (both have hooks), 
			// Sender is responsible for ensuring that this is NOT as set item, not a potion/scroll/other and not a relic. Eg must be equipment which is not part of a set.

			if (obj.isRelic) {
				fnItemRelicGetUIDisplayText((GAME::ItemEquipment*)newItem, character, &gameTextLines, true);
			}
			else {
				fnItemEquipmentGetUIDisplayText((GAME::ItemEquipment*)newItem, character, &gameTextLines, true);
			}
			

			fnDestroyObjectEx(fnGetObjectManager(), (GAME::Object*)newItem, nullptr, 0);


			LogToFile(LogLevel::INFO, L"Generating json..");

			return toJson(obj, gameTextLines);
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

	return boost::property_tree::ptree();
}


std::wstring randomFilename32() {
	std::wstring str(L"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");

	std::random_device rd;
	std::mt19937 generator(rd());

	std::shuffle(str.begin(), str.end(), generator);

	return str.substr(0, 32);    // assumes 32 < number of characters in str         
}


void* __fastcall OnDemandSeedInfo::HookedMethod(void* This, int v) {
	if (This == nullptr) {
		LogToFile(LogLevel::WARNING, L"Update@GameEngine called with 'This' being null");
	}
	if (g_self == nullptr) {
		LogToFile(LogLevel::FATAL, L"Update@GameEngine called with 'g_self' being null");
	}
	void* r = g_self->originalMethod(This, v);


	
	if (g_self->m_sleepMilliseconds <= 0) {
		try {
			// Only start processing items if the game is running.
			// Attempting to create items with a set bonus from the main menu may crash the game.
			// Items with skills may also end up with missing info if created from the main menu.
			bool isGameLoading = IsGameLoading(This);
			bool isGameEngineOnline = IsGameEngineOnline(This);

			if (!isGameLoading /* && !isGameWaiting*/ && isGameEngineOnline) {

				// Process the queue
				int num = 0;

				boost::property_tree::ptree result;
				
				while (!g_self->m_itemQueue.empty() && num++ < 100) {
					LogToFile(LogLevel::INFO, L"Processing..");
					ParsedSeedRequestPtr ptr = g_self->m_itemQueue.pop();
					if (ptr == nullptr) {
						return r;
					}
					ParsedSeedRequest obj = *ptr.get();

					LogToFile(LogLevel::INFO, L"Fetching items stats for " + GAME::Serialize(obj.itemReplicaInfo));
					boost::property_tree::ptree json = g_self->GetItemInfo(obj);
					if (!json.empty()) {
						std::string id = std::to_string(obj.playerItemId) + obj.buddyItemId;
						result.push_back(std::make_pair(id, json));
					}
				}

				if (result.size() > 0) {
					// Write json array
					std::wstring fullPath = GetIagdFolder() + L"replica\\to_ia\\" + randomFilename32();


					std::stringstream json;
					boost::property_tree::write_json(json, result);

					std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
					std::wofstream stream;
					stream.open(fullPath);
					stream << json.str().c_str();
					stream.flush();
					stream.close();
					LogToFile(LogLevel::INFO, L"Wrote items stats to " + fullPath);

					// Now that we're done writing we can move it and give it the .json suffix, that way IA isn't trying to read it while we're writing
					MoveFile(fullPath.c_str(), (fullPath + L".json").c_str());
				}
			}
			else {
				if (isGameLoading) {
					LogToFile(LogLevel::INFO, "Game is loading, real stat generation not available.");
				}
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