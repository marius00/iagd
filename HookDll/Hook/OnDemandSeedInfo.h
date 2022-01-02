#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <string>
#include <thread>
#include "GrimTypes.h"


struct ParsedSeedRequest {
	__int64 playerItemId;
	GAME::ItemReplicaInfo itemReplicaInfo;
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
	const char* pipeName = "\\\\.\\pipe\\gdiahook";
	HANDLE m_thread;
	HANDLE hPipe;
	bool isConnected;
	volatile bool isRunning;
	void Process();
	static void ThreadMain(void*);
	BaseDataQueue<ParsedSeedRequestPtr> m_itemQueue;

	// Feedback for IA
	DataQueue* m_dataQueue;
	HANDLE m_hEvent;

	// Game interaction
	ParsedSeedRequest* Parse(char* databuffer, size_t length);
	void GetItemInfo(ParsedSeedRequest obj);
	typedef void(__fastcall* pItemEquipmentGetUIDisplayText)(GAME::ItemEquipment*, GAME::Character* myCharacter, std::vector<GAME::GameTextLine>* text);
	static pItemEquipmentGetUIDisplayText fnItemEquipmentGetUIDisplayText;

	static OnDemandSeedInfo* g_self;

	// Game hook - To run code inside the game in a safe manner
	// void GAME::GameEngine::Update(int)
	typedef void* (__thiscall* OriginalMethodPtr)(void* This, int v);
	OriginalMethodPtr originalMethod;
	static void* __fastcall HookedMethod(void* This, int v);
};