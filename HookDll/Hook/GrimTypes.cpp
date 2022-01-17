#include "GrimTypes.h"

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

GAME::GameEngine* fnGetgGameEngine() {
	return (GAME::GameEngine*)*(DWORD_PTR*)GetProcAddress(GetModuleHandle(L"game.dll"), "?gGameEngine@GAME@@3PEAVGameEngine@1@EA");
}

std::wstring fnGetModName(GAME::GameInfo* gameInfo) {
	pGetModName f = pGetModName(GetProcAddress(GetModuleHandle(L"engine.dll"), "?GetModName@GameInfo@GAME@@QEBAAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@XZ"));
	return std::wstring(f(gameInfo)->c_str());
}

GAME::GameInfo* fnGetGameInfo(GAME::GameEngine* engine) {
	pGetGameInfo f = pGetGameInfo(GetProcAddress(GetModuleHandle(L"engine.dll"), "?GetGameInfo@Engine@GAME@@QEAAPEAVGameInfo@2@XZ"));
	return f(engine);
}

bool fnGetHardcore(GAME::GameInfo* gameInfo) {
	pGetHardcore f = pGetHardcore(GetProcAddress(GetModuleHandle(L"engine.dll"), "?GetHardcore@GameInfo@GAME@@QEBA_NXZ"));
	return f(gameInfo);

}

typedef std::basic_string<char, std::char_traits<char>, std::allocator<char> > const& Fancystring;
