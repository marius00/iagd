#include "stdafx.h"
#include "GameContext.h"
#include "Logger.h"

#include <algorithm>
#include <mutex>

namespace {

	struct Cache {
		std::mutex mutex;
		GAME::GameInfo* gameInfo = nullptr; // The GameInfo the values below were read from.
		std::wstring modName;
		bool isHardcore = false;
		bool valid = false;
	};

	/// Function-local rather than namespace-scope, for the same reason g_log() in dllmain.cpp is:
	/// initialisation order across translation units is unspecified, and a std::wstring member makes
	/// this dynamically initialised.
	Cache& cache() {
		static Cache instance;
		return instance;
	}

	/// Unchanged from the two identical copies in InventorySack_AddItem and OnDemandSeedInfo that
	/// this replaces.
	std::wstring ReadModName(GAME::GameInfo* gameInfo) {
		std::wstring modName;
		if (fnGetGameInfoMode(gameInfo) != 1) { // Skip mod name if we're in Crucible, we don't treat that as a mod.
			fnGetModNameArg(gameInfo, &modName);
			modName.erase(std::remove(modName.begin(), modName.end(), '\r'), modName.end());
			modName.erase(std::remove(modName.begin(), modName.end(), '\n'), modName.end());
		}

		return modName;
	}
}

namespace GameContext {

	bool Resolve(GAME::GameInfo* gameInfo, std::wstring& modName, bool& isHardcore) {
		if (gameInfo == nullptr) {
			Invalidate();
			return false;
		}

		Cache& c = cache();

		{
			std::lock_guard<std::mutex> guard(c.mutex);
			if (c.valid && c.gameInfo == gameInfo) {
				modName = c.modName;
				isHardcore = c.isHardcore;
				return true;
			}
		}

		// Outside the lock: these are the Engine.dll calls, and holding the lock across them would
		// put a polling thread's wait on the game's thread rather than on a memcpy.
		const std::wstring name = ReadModName(gameInfo);
		const bool hardcore = fnGetHardcore(gameInfo);

		{
			std::lock_guard<std::mutex> guard(c.mutex);
			c.gameInfo = gameInfo;
			c.modName = name;
			c.isHardcore = hardcore;
			c.valid = true;
		}

		// Once per world load, so it is worth a line: it is the only record of which queue folder
		// the deposit and seed-info threads are going to be looking at.
		LogToFile(LogLevel::INFO, L"Game context resolved, mod=\""
			+ (name.empty() ? std::wstring(L"(none)") : name)
			+ L"\" hardcore=" + (hardcore ? L"yes" : L"no"));

		modName = name;
		isHardcore = hardcore;
		return true;
	}

	bool TryGet(std::wstring& modName, bool& isHardcore) {
		Cache& c = cache();
		std::lock_guard<std::mutex> guard(c.mutex);

		if (!c.valid) {
			return false;
		}

		modName = c.modName;
		isHardcore = c.isHardcore;
		return true;
	}

	void Invalidate() {
		Cache& c = cache();
		std::lock_guard<std::mutex> guard(c.mutex);

		if (!c.valid) {
			return;
		}

		c.gameInfo = nullptr;
		c.modName.clear();
		c.isHardcore = false;
		c.valid = false;
	}
}
