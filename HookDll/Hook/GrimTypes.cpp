#include "GrimTypes.h"
#include "Logger.h"
#include <boost/lexical_cast.hpp>
#include <algorithm>

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
#ifdef PLAYTEST
		// Rerolls column (playtest offset 0x17c). 
		stream << replica.seedRerolls << ";";
#else
		stream << 0 << ";";
#endif
		stream << sanitizeCsvField(replica.modifierRecord.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.materiaRecord.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.relicBonus.c_str()).c_str() << ";";
		stream << replica.relicSeed << ";";
		stream << sanitizeCsvField(replica.enchantmentRecord.c_str()).c_str() << ";";
		stream << replica.enchantmentSeed << ";";
		stream << sanitizeCsvField(replica.transmuteRecord.c_str()).c_str() << ";";

#ifdef PLAYTEST
		stream << sanitizeCsvField(replica.ascendant1.c_str()).c_str() << ";";
		stream << sanitizeCsvField(replica.ascendant2.c_str()).c_str() << ";";
#else
		stream << "" << ";";
		stream << "" << ";";
#endif

#ifdef PLAYTEST
		// Affix rerolls column (playtest offset 0x180). See seedRerolls above for
		// the equivalent seed-reroll counter; this is the separate affix-reroll
		// count confirmed by the game devs' upcoming save format.
		stream << replica.affixRerolls;
#else
		stream << 0;
#endif

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
#ifdef PLAYTEST
			// See Serialize(): this column carries the reroll count (offset 0x17c).
			item->seedRerolls = (unsigned int)stoul(tokens.at(idx++));
#else
			auto unused = (unsigned int)stoul(tokens.at(idx++));
#endif
		}
		item->modifierRecord = tokens.at(idx++);
		item->materiaRecord = tokens.at(idx++);
		item->relicBonus = tokens.at(idx++);
		item->relicSeed = (unsigned int)stoul(tokens.at(idx++));
		item->enchantmentRecord = tokens.at(idx++);
		item->enchantmentSeed = (unsigned int)stoul(tokens.at(idx++));
		item->transmuteRecord = tokens.at(idx++);
		if (tokens.size() >= 16) {
#ifdef PLAYTEST
			item->ascendant1 = tokens.at(idx++);
			item->ascendant2 = tokens.at(idx++);
#else
			auto ascendant1 = tokens.at(idx++);
			auto ascendant2 = tokens.at(idx++);
#endif
		}
		if (tokens.size() == 17) {
#ifdef PLAYTEST
			// See Serialize(): this column carries the affix reroll count (offset 0x180).
			item->affixRerolls = (unsigned int)stoul(tokens.at(idx++));
#else
			auto unused = (unsigned int)stoul(tokens.at(idx++));
#endif
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
	auto gameEngine = (GAME::GameEngine*)*(DWORD_PTR*)GetProcAddressOrLogToFile(L"game.dll", "?gGameEngine@GAME@@3PEAVGameEngine@1@EA");
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
	auto engine = (GAME::Engine*)*(DWORD_PTR*)GetProcAddressOrLogToFile(L"engine.dll", "?gEngine@GAME@@3PEAVEngine@1@EA", skipLog);
	if (engine == nullptr) {
		LogToFile(LogLevel::WARNING, "Got engine nullptr, beware if a crash follows this.");
	}
	return engine;
}

bool fnGetHardcore(GAME::GameInfo* gameInfo, bool skipLog) {
	pGetHardcore f = pGetHardcore(GetProcAddressOrLogToFile(L"engine.dll", "?GetHardcore@GameInfo@GAME@@QEBA_NXZ", skipLog));
	return f(gameInfo);

}

typedef std::basic_string<char, std::char_traits<char>, std::allocator<char> > const& Fancystring;

void* GetProcAddressOrLogToFile(const wchar_t* dll, char* procAddress, bool skipLog) {
	void* originalMethod = GetProcAddress(::GetModuleHandle(dll), procAddress);
	if (originalMethod == NULL) {
		LogToFile(LogLevel::FATAL, std::string("Error finding export from DLL: ") + std::string(procAddress));
	}
	else if (!skipLog) {
		LogToFile(LogLevel::INFO, std::string("Successfully found DLL export: ") + std::string(procAddress));
	}

	return originalMethod;
}



IsGameLoadingPtr IsGameLoading = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameLoading@GameEngine@GAME@@QEBA_NXZ"));
IsGameLoadingPtr IsGameEngineOnline = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameEngineOnline@GameEngine@GAME@@QEBA_NXZ"));
IsGameWaitingPtr IsGameWaiting = IsGameWaitingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameWaiting@GameEngine@GAME@@QEAA_N_N@Z"));
SortInventorySackPtr SortInventorySack = SortInventorySackPtr(GetProcAddressOrLogToFile(L"game.dll", "?Sort@InventorySack@GAME@@QEAA_NI@Z"));