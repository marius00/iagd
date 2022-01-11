#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include "OnDemandSeedInfo.h"
#include "Exports.h"


OnDemandSeedInfo::pItemEquipmentGetUIDisplayText OnDemandSeedInfo::fnItemEquipmentGetUIDisplayText;
OnDemandSeedInfo* OnDemandSeedInfo::g_self;
OnDemandSeedInfo::OnDemandSeedInfo() {}
OnDemandSeedInfo::OnDemandSeedInfo(DataQueue* dataQueue, HANDLE hEvent) {
	m_dataQueue = dataQueue;
	m_hEvent = hEvent;
	hPipe = NULL;
	m_thread = NULL;
	fnItemEquipmentGetUIDisplayText = pItemEquipmentGetUIDisplayText(GetProcAddress(GetModuleHandle(TEXT("game.dll")), "?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@@Z"));
}

void OnDemandSeedInfo::EnableHook() {
	originalMethod = (OriginalMethodPtr)HookGame(
		"?Update@GameEngine@GAME@@QEAAXH@Z",
		HookedMethod,
		m_dataQueue,
		m_hEvent,
		TYPE_GAMEENGINE_UPDATE
	);
}
void OnDemandSeedInfo::DisableHook() {
	Unhook((PVOID*)&originalMethod, HookedMethod);
}

/*
* Continously listen for new events on the pipe
*/
void OnDemandSeedInfo::ThreadMain(void*) {
	while (g_self->isRunning) {
		g_self->Process();
	}
}

/*
* Stop the running thread
*/
void OnDemandSeedInfo::Stop() {
	isRunning = false;
	if (m_thread != NULL) {
		CloseHandle(m_thread);
		m_thread = NULL;
	}
}

void testytest() {

}

/*
* Create the named pipe and start a thread to listen for events
*/
void OnDemandSeedInfo::Start() {
	g_self = this;
	isConnected = false;

	hPipe = CreateNamedPipeA(
		pipeName,
		PIPE_ACCESS_DUPLEX,
		PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE,
		PIPE_UNLIMITED_INSTANCES,
		8096 * 40,
		8096 * 40,
		0,
		NULL);


	if (hPipe == INVALID_HANDLE_VALUE)
		return;

	isRunning = true;
	m_thread = (HANDLE)_beginthread(ThreadMain, NULL, 0);
}

/*
* Parse data from the PIPE
* As per definition in Item Assistant (sender)
*/
ParsedSeedRequest* OnDemandSeedInfo::Parse(char* databuffer, size_t length) {
	int pos = 0;
	__int32 recordLength;
	__int64 playerItemId;
	char buddyItemId[64] = { 0 }; // Think maxlen is 36
	const int TYPE_PLAYERITEM = 1;
	const int TYPE_BUDDYITEM = 2;


	__int32 requestType;
	memcpy(&requestType, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	if (requestType == TYPE_PLAYERITEM) {
		memcpy(&playerItemId, databuffer + pos, sizeof(__int64));
		pos += sizeof(__int64);
	}
	else if (requestType == TYPE_BUDDYITEM) {
		recordLength = 0;
		memcpy(&recordLength, databuffer + pos, sizeof(__int32));
		pos += sizeof(__int32);

		memcpy(buddyItemId, databuffer + pos, recordLength);
		pos += recordLength;
	}
	else {
		return nullptr;
	}

	__int32 seed;
	memcpy(&seed, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	__int32 relicSeed;
	memcpy(&relicSeed, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	__int32 enchantmentSeed;
	memcpy(&enchantmentSeed, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	// Base record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);


	char baseRecord[256] = { 0 };
	memcpy(baseRecord, databuffer + pos, recordLength);
	pos += recordLength;

	// Prefix record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	char prefixRecord[256] = { 0 };
	memcpy(prefixRecord, databuffer + pos, recordLength);
	pos += recordLength;

	// Suffix record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	char suffixRecord[256] = { 0 };
	memcpy(suffixRecord, databuffer + pos, recordLength);
	pos += recordLength;

	// Modifier record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	char modifierRecord[256] = { 0 };
	memcpy(modifierRecord, databuffer + pos, recordLength);
	pos += recordLength;

	// Materia record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	char materiaRecord[256] = { 0 };
	memcpy(materiaRecord, databuffer + pos, recordLength);
	pos += recordLength;

	// Enchantment record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	char enchantmentRecord[256] = { 0 };
	memcpy(enchantmentRecord, databuffer + pos, recordLength);
	pos += recordLength;

	// Transmute record
	recordLength = 0;
	memcpy(&recordLength, databuffer + pos, sizeof(__int32));
	pos += sizeof(__int32);

	char transmuteRecord[256] = { 0 };
	memcpy(transmuteRecord, databuffer + pos, recordLength);
	pos += recordLength;


	GAME::ItemReplicaInfo replica;
	replica.seed = seed;
	replica.relicSeed = relicSeed;
	replica.enchantmentSeed = enchantmentSeed;
	replica.baseRecord = std::string(baseRecord);
	replica.prefixRecord = std::string(prefixRecord);
	replica.suffixRecord = std::string(suffixRecord);
	replica.modifierRecord = std::string(modifierRecord);
	replica.materiaRecord = std::string(materiaRecord);
	replica.enchantmentRecord = std::string(enchantmentRecord);
	replica.transmuteRecord = std::string(transmuteRecord);
	replica.stackSize = 1;

	ParsedSeedRequest* result = new ParsedSeedRequest();
	result->itemReplicaInfo = replica;
	result->playerItemId = playerItemId;
	result->buddyItemId = std::string(buddyItemId);
	return result;
}

/*
* Process a single request on the named pipe
*/
void OnDemandSeedInfo::Process() {
	DWORD numBytesRead;
	char buffer[8096] = { 0 };

	if (!isConnected) {
		BOOL client = ConnectNamedPipe(hPipe, NULL);
		if (client != 0)
			isConnected = true;

		// Client connected already, but not ready.
		else if (GetLastError() == ERROR_PIPE_CONNECTED || GetLastError() == ERROR_IO_PENDING) {
			isConnected = true;
			return;
		}

		else
			return;
	}

	bool slowDown = false;
	BOOL fSuccess = ReadFile(hPipe, buffer, sizeof(buffer) / sizeof(char), &numBytesRead, nullptr);
	if (fSuccess && numBytesRead > 0) {
		// Parse and queue item seed read
		ParsedSeedRequest* obj = Parse(&buffer[0], numBytesRead);
		ParsedSeedRequestPtr abc(obj);
		if (!m_itemQueue.push(abc, 300)) {
			slowDown = true;
			// Will just discard data if >N
			// TODO: Notify IA that it needs to slow the fk down?
		}

		DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_DEBUG_RECV, sizeof(numBytesRead), (char*)&numBytesRead));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);

		FlushFileBuffers(hPipe);
		DisconnectNamedPipe(hPipe);
		isConnected = false;
	}
	else if (!fSuccess) {
		DisconnectNamedPipe(hPipe);
		isConnected = false;
	}

	// Give the game loop some time to catch up
	if (slowDown) {
		Sleep(2500);
	}
}


// TODO: Either rename, or make this method do less. Probably the latter
void OnDemandSeedInfo::GetItemInfo(ParsedSeedRequest obj) {
	// Check for access to Game.dll
	if (GetModuleHandleA("Game.dll")) {
		GAME::ItemReplicaInfo replica = obj.itemReplicaInfo;
		GAME::Item* newItem = fnCreateItem(&replica);
		if (newItem) {
			std::vector<GAME::GameTextLine> gameTextLines = {};

			// TODO: We should fetch this earlier, ensure we don't get the hooked method. -- We seem to be getting 4 replies. 4th one is the message below. 
			// First is probably in Item:: then ItemEquipment:: (both have hooks), 
			// Sender is responsible for ensuring that this is NOT as set item, not a potion/scroll/other and not a relic. Eg must be equipment which is not part of a set.
			fnItemEquipmentGetUIDisplayText((GAME::ItemEquipment*)newItem, (GAME::Character*)fnGetMainPlayer(fnGetgGameEngine()), &gameTextLines);
			fnDestroyObjectEx(fnGetObjectManager(), (GAME::Object*)newItem, nullptr, 0);

			std::wstringstream stream;

			GAME::ItemReplicaInfo replica;
			stream << obj.playerItemId << "\n"; // Differs from TYPE_ITEMSEEDDATA
			stream << obj.buddyItemId.c_str() << "\n";
			stream << GAME::itemReplicaToString(obj.itemReplicaInfo) << "\n";
			stream << GAME::gameTextLineToString(gameTextLines);

			std::wstring str = stream.str();
			DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID, str.size() * sizeof(wchar_t), (char*)str.c_str()));
			m_dataQueue->push(item);
			SetEvent(m_hEvent);
		}
		else {
			std::string str = obj.itemReplicaInfo.baseRecord;
			DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOITEM, str.size(), (char*)str.c_str()));
			m_dataQueue->push(item);
			SetEvent(m_hEvent);
		}
	}
	else {
		DataItemPtr item(new DataItem(TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOGAME, 0, nullptr));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}
}

typedef bool(__thiscall* IsGameLoadingPtr)(void* This);
typedef bool(__thiscall* IsGameWaitingPtr)(void* This, bool);
IsGameLoadingPtr IsGameLoading = IsGameLoadingPtr(GetProcAddress(GetModuleHandle(L"game.dll"), "?IsGameLoading@GameEngine@GAME@@QEBA_NXZ"));
IsGameLoadingPtr IsGameEngineOnline = IsGameLoadingPtr(GetProcAddress(GetModuleHandle(L"game.dll"), "?IsGameEngineOnline@GameEngine@GAME@@QEBA_NXZ"));
IsGameWaitingPtr IsGameWaiting = IsGameWaitingPtr(GetProcAddress(GetModuleHandle(L"game.dll"), "?IsGameWaiting@GameEngine@GAME@@QEAA_N_N@Z"));


void* __fastcall OnDemandSeedInfo::HookedMethod(void* This, int v) {
	void* r = g_self->originalMethod(This, v);

	// Only start processing items if the game is running.
	// Attempting to create items with a set bonus from the main menu may crash the game.
	// Items with skills may also end up with imssing info if created from the main menu.
	if (!IsGameLoading(This) && !IsGameWaiting(This, true) && IsGameEngineOnline(This)) {
		// Process the queue
		int num = 0;
		while (!g_self->m_itemQueue.empty() && num++ < 15) {
			ParsedSeedRequestPtr ptr = g_self->m_itemQueue.pop();
			ParsedSeedRequest obj = *ptr.get();
			g_self->GetItemInfo(obj);
		}
	}

	return r;
}