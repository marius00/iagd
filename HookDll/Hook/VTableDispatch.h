#pragma once
#include "GrimTypes.h"
#include "Logger.h"
#include <windows.h>
#include <vector>
#include <string>

namespace VTableDispatch
{
	typedef void(__fastcall* pGetUIDisplayText)(
		void*                             /* this            */,
		const GAME::Character*            /* myCharacter     */,
		std::vector<GAME::GameTextLine>*  /* text            */,
		bool                              /* includeSetBonus */
		);

	struct VftableCandidate {
		const char* vftableExport;  // exported symbol name of the vftable itself
		const char* fnExport;       // exported symbol name of GetUIDisplayText for this class
	};

	static const VftableCandidate k_candidates[] = {
		{
			"??_7ItemEquipment@GAME@@6BObject@1@@",
			"?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z"
		},
		{
			"??_7ItemRelic@GAME@@6BObject@1@@",
			"?GetUIDisplayText@ItemRelic@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z"
		},
		{
			"??_7ItemArtifact@GAME@@6BObject@1@@",
			"?GetUIDisplayText@ItemArtifact@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z"
		},
		{
			"??_7Item@GAME@@6BObject@1@@",
			"?GetUIDisplayText@Item@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z"
		},
	};

	static int  s_slot = -1;
	static bool s_disabled = false;

	static void Init()
	{
		if (s_slot >= 0 || s_disabled)
			return;

		HMODULE hGame = ::GetModuleHandleW(L"game.dll");
		if (!hGame)
		{
			LogToFile(LogLevel::FATAL, "VTableDispatch::Init - game.dll not loaded, disabling.");
			s_disabled = true;
			return;
		}

		for (const auto& candidate : k_candidates)
		{
			// Get the address of the vftable itself
			void* vftableAddr = ::GetProcAddress(hGame, candidate.vftableExport);
			if (!vftableAddr)
			{
				LogToFile(LogLevel::INFO,
					std::string("VTableDispatch::Init - vftable export not found: ") +
					candidate.vftableExport);
				continue;
			}

			// Get the address of GetUIDisplayText for this class
			void* fnAddr = ::GetProcAddress(hGame, candidate.fnExport);
			if (!fnAddr)
			{
				LogToFile(LogLevel::INFO,
					std::string("VTableDispatch::Init - fn export not found: ") +
					candidate.fnExport);
				continue;
			}

			// Scan the vftable for the fn address -- slot = index where it appears
			uintptr_t target = reinterpret_cast<uintptr_t>(fnAddr);
			auto** vfptr = reinterpret_cast<void**>(vftableAddr);

			for (int i = 0; i < 256; i++)
			{
				if (::IsBadReadPtr(&vfptr[i], sizeof(void*)))
					break;

				if (reinterpret_cast<uintptr_t>(vfptr[i]) == target)
				{
					s_slot = i;
					LogToFile(LogLevel::INFO,
						"VTableDispatch::Init - resolved slot " + std::to_string(i) +
						" via " + candidate.vftableExport);
					return;
				}
			}

			LogToFile(LogLevel::INFO,
				std::string("VTableDispatch::Init - fn not found in vftable: ") +
				candidate.vftableExport);
		}

		LogToFile(LogLevel::FATAL,
			"VTableDispatch::Init - could not resolve slot. Disabling.");
		s_disabled = true;
	}

	static bool IsAvailable() { return s_slot >= 0 && !s_disabled; }

	static void Call(
		void* item,
		const GAME::Character* character,
		std::vector<GAME::GameTextLine>* lines,
		bool                             includeSetBonus)
	{
		if (!item || s_slot < 0 || s_disabled)
			return;

		void** vfptr = *reinterpret_cast<void***>(item);
		auto fn = reinterpret_cast<pGetUIDisplayText>(vfptr[s_slot]);
		fn(item, character, lines, includeSetBonus);
	}

} // namespace VTableDispatch
