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
	volatile bool m_isActive;
	void Process();
	static void ThreadMain(void*);
	BaseDataQueue<ParsedSeedRequestPtr> m_itemQueue;

	// Feedback for IA
	DataQueue* m_dataQueue;
	HANDLE m_hEvent;
	volatile int m_sleepMilliseconds;

	// Game interaction
	boost::property_tree::ptree GetItemInfo(ParsedSeedRequest obj);
	typedef void(__fastcall* pItemEquipmentGetUIDisplayText)(GAME::ItemEquipment*, GAME::Character* myCharacter, std::vector<GAME::GameTextLine>* text, bool includeSetBonusDetails); // If false, we'll get a "click here for more info" text instead.
	static pItemEquipmentGetUIDisplayText fnItemEquipmentGetUIDisplayText;
	static pItemEquipmentGetUIDisplayText fnItemRelicGetUIDisplayText;

	static OnDemandSeedInfo* g_self;

	// Game hook - To run code inside the game in a safe manner
	// void GAME::GameEngine::Update(int)
	typedef void* (__thiscall* OriginalGameUpdateMethodPtr)(void* This, int v);
	typedef void* (__thiscall* OriginalEngineRenderMethodPtr)(void* This);

	OriginalGameUpdateMethodPtr gameUpdateMethod;
	OriginalEngineRenderMethodPtr engineRenderMethod;
	static void* __fastcall HookedEngineRenderMethod(void* This);
	static void* __fastcall HookedGameUpdateMethod(void* This, int v);
	static std::wstring GetModName(GAME::GameInfo* gameInfo);
	ParsedSeedRequest* ReadReplicaInfo(const std::wstring& filename);
	ParsedSeedRequest* DeserializeReplicaCsv(std::vector<std::string> tokens);
	static std::mutex _mutex;
};