#include "GrimTypes.h"
#include "Logger.h"
#include "CrashReporter.h"
#include <atomic>
#include <boost/lexical_cast.hpp>
#include <algorithm>
#include <iostream>
#include <mutex>
#include <set>
#include <sstream>
#include <vector>
#include <Windows.h>
#include <fstream>

namespace GAME {
	// Helper: strip \r, \n from a narrow string (defensive against game struct issues)
	static std::string sanitizeCsvField(const char* s) {
		std::string result(s);
		result.erase(std::remove(result.begin(), result.end(), '\r'), result.end());
		result.erase(std::remove(result.begin(), result.end(), '\n'), result.end());
		return result;
	}

	std::wstring Serialize(GAME::ItemReplicaInfo replica) {
		std::wstringstream stream;
		stream << sanitizeCsvField(replica.baseRecord.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.prefixRecord.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.suffixRecord.c_str()).c_str() << ";";
		stream << replica.seed << ";";
		// Rerolls column (offset 0x17c).
		stream << replica.seedRerolls << ";";
		stream << sanitizeCsvField(replica.modifierRecord.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.materiaRecord.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.relicBonus.c_str()).c_str() << ";";
		stream << replica.relicSeed << ";";
		stream << sanitizeCsvField(replica.enchantmentRecord.c_str()).c_str() << ";";
		stream << replica.enchantmentSeed << ";";
		stream << sanitizeCsvField(replica.transmuteRecord.c_str()).c_str() << ";";

		stream << sanitizeCsvField(replica.ascendant1.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.ascendant2.c_str()).c_str() << ";";

		// Affix rerolls column (offset 0x180).
		stream << replica.affixRerolls;

		return stream.str();
	}

	// https://stackoverflow.com/questions/1120140/how-can-i-read-and-parse-csv-files-in-c
	std::vector<std::string> GetNextLineAndSplitIntoTokens(std::istream& str) {
		std::vector<std::string>   result;
		std::string                line;
		std::getline(str, line);

		std::stringstream          lineStream(line);
		std::string                cell;

		while (std::getline(lineStream, cell, ';')) {
			result.push_back(cell);
		}

		// This checks for a trailing semicolon with no data after it.
		if (!lineStream && cell.empty()) {
			// If there was a trailing semicolon then add an empty element.
			result.push_back("");
		}
		return result;
	}

	GAME::ItemReplicaInfo* Deserialize(std::vector<std::string> tokens) {
		// 13 = legacy (no rerolls, no ascendants)
		// 14 = bugfix compat (rerolls present, ascendants split to next line)
		// 16 = rerolls + ascendants
		// 17 = current format (rerolls + ascendants + affixRerolls)
		if (tokens.size() != 13 && tokens.size() != 14 && tokens.size() != 16 && tokens.size() != 17) {
			LogToFile(LogLevel::WARNING, L"Error parsing CSV file, expected 13, 14, 16, or 17 tokens, got " + std::to_wstring(tokens.size()));
			return nullptr;
		}

		bool isNewDlc = tokens.size() >= 14;

		GAME::ItemReplicaInfo* item = new GAME::ItemReplicaInfo();

		// The game's own ItemReplicaInfo constructor defaults stackSize to 1 (verified
		// in the playtest 1.3 binary). Our zero-initialized struct left it at 0, which
		// creates items whose Item::GetStackSize() returns 0. The 1.3 crafting rework
		// counts owned recipe ingredients by summing GetStackSize() over the inventory,
		// so a stackSize=0 item is never recognized as a crafting ingredient (until an
		// inventor reroll re-creates it with a game-built replica). IA never loots
		// stackable items, so 1 is always the correct count for deposited items.
		item->stackSize = 1;

		int idx = 2; // 0: is the mod name, 1: is "isHardcore"
		item->baseRecord = tokens.at(idx++);
		item->prefixRecord = tokens.at(idx++);
		item->suffixRecord = tokens.at(idx++);
		item->seed = (unsigned int)stoul(tokens.at(idx++));
		if (isNewDlc) {
			// See Serialize(): this column carries the reroll count (offset 0x17c).
			item->seedRerolls = (unsigned int)stoul(tokens.at(idx++));
		}
		item->modifierRecord = tokens.at(idx++);
		item->materiaRecord = tokens.at(idx++);
		item->relicBonus = tokens.at(idx++);
		item->relicSeed = (unsigned int)stoul(tokens.at(idx++));
		item->enchantmentRecord = tokens.at(idx++);
		item->enchantmentSeed = (unsigned int)stoul(tokens.at(idx++));
		item->transmuteRecord = tokens.at(idx++);
		if (tokens.size() >= 16) {
			item->ascendant1 = tokens.at(idx++);
			item->ascendant2 = tokens.at(idx++);
		}
		if (tokens.size() == 17) {
			// See Serialize(): this column carries the affix reroll count (offset 0x180).
			item->affixRerolls = (unsigned int)stoul(tokens.at(idx++));
		}

		return item;
	}

	/// <summary>
	/// Helper method for converting gameTextLine to a CSV string.
	/// </summary>
	/// <param name="gameTextLines"></param>
	/// <returns></returns>
	std::wstring GameTextLineToString(std::vector<GameTextLine>& gameTextLines) {
		std::wstringstream stream;
		GAME::ItemReplicaInfo replica;

		for (auto& it : gameTextLines) {
			stream << it.textClass << ";" << it.text.c_str() << "\n";
		}

		std::wstring str = stream.str();
		return str;
	}



}

/// <summary>
/// Fetches the static pointer to GAME::GameEngine (not a method call)
/// </summary>
/// <returns></returns>
GAME::GameEngine* fnGetGameEngine() {
	// The export is a pointer *to* the engine pointer, so it has to be dereferenced -- but only once we know it was found. This used to dereference the result of the lookup directly, which takes the game
	// down with us whenever game.dll is not loaded, instead of reporting "not ready" the way every other path here does. ProcessAttach treats nullptr as "abort the attach".
	auto slot = (DWORD_PTR*)GetProcAddressOrLogToFile(L"game.dll", "?gGameEngine@GAME@@3PEAVGameEngine@1@EA");
	if (slot == nullptr) {
		LogToFile(LogLevel::WARNING, "game.dll export gGameEngine unavailable, the game is not ready to be hooked.");
		return nullptr;
	}

	auto gameEngine = (GAME::GameEngine*)*slot;
	if (gameEngine == nullptr) {
		LogToFile(LogLevel::WARNING, "Got game engine nullptr, beware if a crash follows this.");
	}
	return gameEngine;
}

/// <summary>
/// Fetches the static pointer to GAME::Engine (not a method call)
/// </summary>
/// <returns></returns>
GAME::Engine* fnGetEngine(bool skipLog) {
	// Same unchecked dereference as fnGetGameEngine above; see the comment there.
	auto slot = (DWORD_PTR*)GetProcAddressOrLogToFile(L"engine.dll", "?gEngine@GAME@@3PEAVEngine@1@EA", skipLog);
	if (slot == nullptr) {
		if (!skipLog) {
			LogToFile(LogLevel::WARNING, "engine.dll export gEngine unavailable, the game is not ready to be hooked.");
		}
		return nullptr;
	}

	auto engine = (GAME::Engine*)*slot;
	if (engine == nullptr) {
		LogToFile(LogLevel::WARNING, "Got engine nullptr, beware if a crash follows this.");
	}
	return engine;
}

bool fnIsWorldAlive(GAME::GameEngine* gameEngine) {
	if (gameEngine == nullptr) {
		return false;
	}

	if (IsGameLoading(gameEngine)) {
		return false;
	}

	if (!IsGameEngineOnline(gameEngine)) {
		return false;
	}

	// Deliberately NOT using IsGameWaiting(gameEngine, false) here. It looks like the
	// stronger check (it null-checks GetMainPlayer internally) but it also demands
	// player state == 2 and player+0x242a == 0, and we don't know what those mean.
	// If either is set while the transfer stash is open it would silently disable
	// instaloot, so we settle for the player null check, which is the part we want.
	return fnGetMainPlayer(gameEngine) != nullptr;
}

namespace {
	std::atomic<ULONGLONG> g_lastAddItemTick{ 0 };
	std::atomic<int> g_lastWorldState{ -1 }; // -1 unknown, 0 dead, 1 alive

	const wchar_t* WorldStateName(int state) {
		switch (state) {
		case 0:  return L"DEAD";
		case 1:  return L"ALIVE";
		default: return L"UNKNOWN";
		}
	}
}

void fnNoteItemAdded() {
	g_lastAddItemTick = GetTickCount64();
}

long long fnMsSinceLastAddItem() {
	const ULONGLONG last = g_lastAddItemTick.load();
	if (last == 0) {
		return -1;
	}

	return static_cast<long long>(GetTickCount64() - last);
}

void fnLogWorldStateTransition(GAME::GameEngine* gameEngine, const wchar_t* site) {
	const int state = fnIsWorldAlive(gameEngine) ? 1 : 0;
	const int previous = g_lastWorldState.exchange(state);
	if (previous == state) {
		return;
	}

	// Timestamps the exact moment the world deconstructed, so a crash report shows how many milliseconds separated teardown from whatever the hook was doing on other threads.
	CrashReporter::Note(state == 1 ? "WORLD -> ALIVE" : "WORLD -> DEAD", (uint64_t)gameEngine);

	std::wstringstream ss;
	ss << L"WORLD STATE " << WorldStateName(previous) << L" -> " << WorldStateName(state)
	   << L" at " << site;

	ss << std::hex << std::showbase;
	ss << L" | gGameEngine=" << reinterpret_cast<DWORD_PTR>(gameEngine);

	// Skill::GetSkillProfile() returns this address for every skill that has no
	// profile of its own, and ~GameEngine destroys it. If a dump faults reading
	// [rax+0x140] / [rax+0x150], compare rax against this value.
	ss << L" | defaultSkillProfile="
	   << (gameEngine != nullptr ? reinterpret_cast<DWORD_PTR>(gameEngine) + 0x1d8 : 0);

	GAME::Engine* engine = fnGetEngine(true);
	ss << L" | gEngine=" << reinterpret_cast<DWORD_PTR>(engine);
	ss << L" | gameInfo="
	   << reinterpret_cast<DWORD_PTR>(engine != nullptr ? fnGetGameInfo(engine) : nullptr);
	ss << L" | mainPlayer="
	   << reinterpret_cast<DWORD_PTR>(gameEngine != nullptr ? fnGetMainPlayer(gameEngine) : nullptr);
	ss << std::dec << std::noshowbase;

	if (gameEngine != nullptr) {
		ss << L" | isGameLoading=" << (IsGameLoading(gameEngine) ? 1 : 0);
		ss << L" | isEngineOnline=" << (IsGameEngineOnline(gameEngine) ? 1 : 0);
	}

	const long long sinceAddItem = fnMsSinceLastAddItem();
	ss << L" | msSinceLastAddItem=";
	if (sinceAddItem < 0) {
		ss << L"never";
	}
	else {
		ss << sinceAddItem;
	}

	LogToFile(LogLevel::WARNING, ss.str());
}

bool fnGetHardcore(GAME::GameInfo* gameInfo, bool skipLog) {
	pGetHardcore f = pGetHardcore(GetProcAddressOrLogToFile(L"engine.dll", "?GetHardcore@GameInfo@GAME@@QEBA_NXZ", skipLog));
	return f(gameInfo);

}

typedef std::basic_string<char, std::char_traits<char>, std::allocator<char> > const& Fancystring;

/// <summary>
/// Some exports (gEngine, gGameEngine) are looked up on every tick, so confirming each one
/// on every lookup drowns the log. Returns true only the first time we see a given export.
/// </summary>
static bool IsFirstLookupOfExport(const char* procAddress) {
	// Called from the game threads as well as IA's polling threads, so the set needs a lock.
	static std::mutex mutex;
	static std::set<std::string> seen;

	std::lock_guard<std::mutex> guard(mutex);
	return seen.insert(procAddress).second;
}

void* GetProcAddressOrLogToFile(const wchar_t* dll, char* procAddress, bool skipLog) {
	void* originalMethod = GetProcAddress(::GetModuleHandle(dll), procAddress);
	if (originalMethod == NULL) {
		LogToFile(LogLevel::FATAL, std::string("Error finding export from DLL: ") + std::string(procAddress));
	}
	else if (!skipLog && IsFirstLookupOfExport(procAddress)) {
		LogToFile(LogLevel::INFO, std::string("Successfully found DLL export: ") + std::string(procAddress));
	}

	return originalMethod;
}



IsGameLoadingPtr IsGameLoading = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameLoading@GameEngine@GAME@@QEBA_NXZ"));
IsGameLoadingPtr IsGameEngineOnline = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameEngineOnline@GameEngine@GAME@@QEBA_NXZ"));
IsGameWaitingPtr IsGameWaiting = IsGameWaitingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameWaiting@GameEngine@GAME@@QEAA_N_N@Z"));
SortInventorySackPtr SortInventorySack = SortInventorySackPtr(GetProcAddressOrLogToFile(L"game.dll", "?Sort@InventorySack@GAME@@QEAA_NI@Z"));