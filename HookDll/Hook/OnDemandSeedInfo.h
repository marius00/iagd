#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <string>
#include <thread>
#include <boost/property_tree/ptree.hpp>             
#include <mutex>

#include "GrimTypes.h"


struct ParsedSeedRequest {
	__int64 playerItemId;
	GAME::ItemReplicaInfo itemReplicaInfo;
	std::string buddyItemId;
	bool isRelic;
};
typedef boost::shared_ptr<ParsedSeedRequest> ParsedSeedRequestPtr;


class OnDemandSeedInfo : public BaseMethodHook {
public:
	OnDemandSeedInfo();
	OnDemandSeedInfo(DataQueue* dataQueue, HANDLE hEvent);
	void Start();
	void Stop();

	void EnableHook() override;
	void DisableHook() override;

protected:
	// Pipe&thread stuff
	HANDLE m_thread;
	HANDLE m_threadStoppedEvent;
	std::atomic<bool> m_isActive;
	void Process();
	static unsigned __stdcall ThreadMain(void*);
	BaseDataQueue<ParsedSeedRequestPtr> m_itemQueue;

	// Feedback for IA
	DataQueue* m_dataQueue;
	HANDLE m_hEvent;
	std::atomic<int> m_sleepMilliseconds;

	// Game interaction
	boost::property_tree::ptree GetItemInfo(ParsedSeedRequest obj);
	typedef void(__fastcall* pItemEquipmentGetUIDisplayText)(GAME::ItemEquipment*, GAME::Character* myCharacter, std::vector<GAME::GameTextLine>* text, bool includeSetBonusDetails); // If false, we'll get a "click here for more info" text instead.
	static pItemEquipmentGetUIDisplayText fnItemEquipmentGetUIDisplayText;
	static pItemEquipmentGetUIDisplayText fnItemRelicGetUIDisplayText;

	static OnDemandSeedInfo* g_self;

	// Game hook - To run code inside the game in a safe manner
	// GameEngine::SetDifficultyRamp gained a trailing bool in the 2026-07-26 FOA patch:
	//   before: ?SetDifficultyRamp@GameEngine@GAME@@QEAAXH@Z    -> void(int)
	//   after:  ?SetDifficultyRamp@GameEngine@GAME@@QEAAXH_N@Z  -> void(int, bool)
	// The extra argument arrives in R8, so it has to be declared and forwarded or the
	// original method reads whatever happens to be in that register.
	typedef void* (__thiscall* OriginalEngineRenderMethodPtr)(void* This, int v, bool b);
	typedef void* (__thiscall* Engine_Render)(void* This);

	OriginalEngineRenderMethodPtr gameSetDifficultyRampMethod;
	Engine_Render dll_Engine_Render;
	static void* __fastcall HookedGameSetDifficultyRampMethod(void* This, int v, bool b);
	static void* __fastcall Hooked_Engine_Render(void* This);
	static std::wstring GetModName(GAME::GameInfo* gameInfo);
	ParsedSeedRequest* ReadReplicaInfo(const std::wstring& filename);
	ParsedSeedRequest* DeserializeReplicaCsv(std::vector<std::string> tokens);
	static std::mutex _mutex;
};