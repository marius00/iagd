#pragma once
#include <windows.h>
#include <vector>
#include "DataQueue.h"
#include "BaseMethodHook.h"
#include <string>
#include <thread>
#include "HookLog.h"
#include "GrimTypes.h"


struct ParsedSeedRequest {
	__int32 playerItemId;
	GAME::ItemReplicaInfo itemReplicaInfo;
};

class OnDemandSeedInfo {
public:
	OnDemandSeedInfo();
	OnDemandSeedInfo(DataQueue* dataQueue, HANDLE hEvent);
	void Start();
	void Stop();

protected:
	// Pipe&thread stuff
	const char* pipeName = "\\\\.\\pipe\\gdiahook";
	HANDLE m_thread;
	HANDLE hPipe;
	bool isConnected;
	volatile bool isRunning;

	// Feedback for IA
	DataQueue* m_dataQueue;
	HANDLE m_hEvent;

	void Process();
	ParsedSeedRequest Parse(char* databuffer, size_t length);
	void GetItemInfo(ParsedSeedRequest obj);

	static void ThreadMain(void*);
	static OnDemandSeedInfo* g_self;

	typedef void(__fastcall* pItemEquipmentGetUIDisplayText)(GAME::ItemEquipment*, GAME::Character* myCharacter, std::vector<GAME::GameTextLine>* text);
	static pItemEquipmentGetUIDisplayText fnItemEquipmentGetUIDisplayText;
};