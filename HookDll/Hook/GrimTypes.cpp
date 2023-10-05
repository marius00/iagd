#include "GrimTypes.h"
#include "Logger.h"

namespace GAME {
	std::wstring itemReplicaToString(GAME::ItemReplicaInfo replica) {
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

	/// <summary>
	/// Helper method for converting gameTextLine to a CSV string.
	/// </summary>
	/// <param name="gameTextLines"></param>
	/// <returns></returns>
	std::wstring gameTextLineToString(std::vector<GameTextLine>& gameTextLines) {
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
GAME::Engine* fnGetEngine() {
	return (GAME::Engine*)*(DWORD_PTR*)GetProcAddressOrLogToFile(L"engine.dll", "?gEngine@GAME@@3PEAVEngine@1@EA");
}

bool fnGetHardcore(GAME::GameInfo* gameInfo) {
	pGetHardcore f = pGetHardcore(GetProcAddressOrLogToFile(L"engine.dll", "?GetHardcore@GameInfo@GAME@@QEBA_NXZ"));
	return f(gameInfo);

}

typedef std::basic_string<char, std::char_traits<char>, std::allocator<char> > const& Fancystring;

void* GetProcAddressOrLogToFile(const wchar_t* dll, char* procAddress) {
	void* originalMethod = GetProcAddress(::GetModuleHandle(dll), procAddress);
	if (originalMethod == NULL) {
		LogToFile(std::string("Error finding export from DLL: ") + std::string(procAddress));
	}
	else {
		LogToFile(std::string("Successfully found DLL export: ") + std::string(procAddress));
	}

	return originalMethod;
}



IsGameLoadingPtr IsGameLoading = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameLoading@GameEngine@GAME@@QEBA_NXZ"));
IsGameLoadingPtr IsGameEngineOnline = IsGameLoadingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameEngineOnline@GameEngine@GAME@@QEBA_NXZ"));
IsGameWaitingPtr IsGameWaiting = IsGameWaitingPtr(GetProcAddressOrLogToFile(L"game.dll", "?IsGameWaiting@GameEngine@GAME@@QEAA_N_N@Z"));