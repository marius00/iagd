#pragma once
#include <string>
#include "GrimTypes.h"

/// The mod name and hardcore flag of the world that is currently loaded.
///
namespace GameContext {
	/// Reads the values out of the game, unless they are already known for this GameInfo.
	///
	/// GAME THREAD ONLY -- this is the one place allowed to call into Engine.dll for them.
	/// Returns false when there is nothing to read, leaving the outputs untouched.
	bool Resolve(GAME::GameInfo* gameInfo, std::wstring& modName, bool& isHardcore);

	/// Takes a copy of whatever the game thread last resolved. Safe from any thread.
	///
	/// Returns false while no world is loaded, which is the caller's cue to skip this round rather
	/// than guess: the folder to read from is derived from these, and guessing means reading someone
	/// else's queue.
	bool TryGet(std::wstring& modName, bool& isHardcore);

	/// Forgets the current world. Safe from any thread.
	void Invalidate();
}
