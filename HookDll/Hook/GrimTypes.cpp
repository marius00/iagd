#include "GrimTypes.h"

namespace GAME {
	std::wstring itemReplicaToString(GAME::ItemReplicaInfo replica) {
		std::wstringstream stream;
		stream << replica.baseName.c_str() << ";";
		stream << replica.prefixName.c_str() << ";";
		stream << replica.suffixName.c_str() << ";";
		stream << replica.seed << ";";
		stream << replica.modifierName.c_str() << ";";
		stream << replica.relicName.c_str() << ";";
		stream << replica.relicBonus.c_str() << ";";
		stream << replica.relicSeed << ";";
		stream << replica.enchantmentName.c_str() << ";";
		stream << replica.enchantmentSeed << ";";
		stream << replica.transmuteName.c_str() << ";";

		return stream.str();
	}
	//typedef void *(__fastcall * pCreateItem)(GAME::ItemReplicaInfo& info);

}