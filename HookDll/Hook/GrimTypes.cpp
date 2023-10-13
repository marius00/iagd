#include "GrimTypes.h"
#include "Logger.h"
#include <boost/lexical_cast.hpp>

namespace GAME {
	std::wstring Serialize(GAME::ItemReplicaInfo replica) {
		std::wstringstream stream;
		stream << replica.baseRecord.c_str() << ";";
		stream << replica.prefixRecord.c_str() << ";";
		stream << replica.suffixRecord.c_str() << ";";
		stream << replica.seed << ";";
		stream << replica.modifierRecord.c_str() << ";";
		stream << replica.materiaRecord.c_str() << ";";
		stream << replica.relicBonus.c_str() << ";";
		stream << replica.relicSeed << ";";
		stream << replica.enchantmentRecord.c_str() << ";";
		stream << replica.enchantmentSeed << ";";
		stream << replica.transmuteRecord.c_str();

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
		if (tokens.size() != 13) {
			LogToFile(L"Error parsing CSV file, expected 13 tokens, got " + std::to_wstring(tokens.size()));
			return nullptr;
		}

		GAME::ItemReplicaInfo* item = new GAME::ItemReplicaInfo();
		int idx = 2; // 0: is the mod name, 1: is "isHardcore"
		item->baseRecord = tokens.at(idx++);
		item->prefixRecord = tokens.at(idx++);
		item->suffixRecord = tokens.at(idx++);
		item->seed = (unsigned int)stoul(tokens.at(idx++));
		item->modifierRecord = tokens.at(idx++);
		item->materiaRecord = tokens.at(idx++);
		item->relicBonus = tokens.at(idx++);
		item->relicSeed = (unsigned int)stoul(tokens.at(idx++));
		item->enchantmentRecord = tokens.at(idx++);
		item->enchantmentSeed = (unsigned int)stoul(tokens.at(idx++));
		item->transmuteRecord = tokens.at(idx++);

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
	return (GAME::GameEngine*)*(DWORD_PTR*)GetProcAddressOrLogToFile(L"game.dll", "?gGameEngine@GAME@@3PEAVGameEngine@1@EA");
}

/// <summary>
/// Fetches the static pointer to GAME::Engine (not a method call)
/// </summary>
/// <returns></returns>
GAME::Engine* fnGetEngine(bool skipLog) {
	return (GAME::Engine*)*(DWORD_PTR*)GetProcAddressOrLogToFile(L"engine.dll", "?gEngine@GAME@@3PEAVEngine@1@EA", skipLog);
}

bool fnGetHardcore(GAME::GameInfo* gameInfo, bool skipLog) {
	pGetHardcore f = pGetHardcore(GetProcAddressOrLogToFile(L"engine.dll", "?GetHardcore@GameInfo@GAME@@QEBA_NXZ", skipLog));
	return f(gameInfo);

}

typedef std::basic_string<char, std::char_traits<char>, std::allocator<char> > const& Fancystring;

void* GetProcAddressOrLogToFile(const wchar_t* dll, char* procAddress, bool skipLog) {
	void* originalMethod = GetProcAddress(::GetModuleHandle(dll), procAddress);
	if (originalMethod == NULL) {
		LogToFile(std::string("Error finding export from DLL: ") + std::string(procAddress));
	}
	else if (!skipLog) {
		LogToFile(std::string("Successfully found DLL export: ") + std::string(procAddress));
	}

	return originalMethod;
}



IsGameLoadingPtr IsGameLoading = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameLoading@GameEngine@GAME@@QEBA_NXZ"));
IsGameLoadingPtr IsGameEngineOnline = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameEngineOnline@GameEngine@GAME@@QEBA_NXZ"));
IsGameWaitingPtr IsGameWaiting = IsGameWaitingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameWaiting@GameEngine@GAME@@QEAA_N_N@Z"));
SortInventorySackPtr SortInventorySack = SortInventorySackPtr(GetProcAddressOrLogToFile(L"game.dll", "?Sort@InventorySack@GAME@@QEAA_NI@Z"));