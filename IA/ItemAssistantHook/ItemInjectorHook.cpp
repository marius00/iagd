#include "ItemInjectorHook.h"
#include <detours.h>
#include "MessageType.h"
#include <process.h>

void setString(char* buffer, char* str, int len) {
	DWORD isDeref = 16;
	memset(buffer, 0, 24);
	memcpy(buffer, &str, sizeof(char*));
	memcpy(buffer + 16, &len, sizeof(len));
	memcpy(buffer + 20, &isDeref, sizeof(isDeref));
}

HANDLE ItemInjectorHook::m_hEvent;
DataQueue* ItemInjectorHook::m_dataQueue;
HANDLE ItemInjectorHook::hPipe;
bool ItemInjectorHook::isConnected;
DataQueue ItemInjectorHook::m_itemQueue;
HANDLE ItemInjectorHook::m_itemQueueEvent;
HANDLE ItemInjectorHook::m_thread;
void* ItemInjectorHook::_inventorySack3;
ItemInjectorHook::InventorySack_AddItem ItemInjectorHook::dll_InventorySack_AddItem;
void* ItemInjectorHook::gameEngine = NULL;
std::set<void*>* ItemInjectorHook::inventorySackSet;


ItemInjectorHook::GameItem_CreateItem ItemInjectorHook::dll_GameItem_CreateItem;
ItemInjectorHook::GameEngine_Update ItemInjectorHook::dll_GameEngine_Update;

ItemInjectorHook::ItemInjectorHook() {
	hPipe = NULL;
	m_itemQueueEvent = NULL;
}
ItemInjectorHook::ItemInjectorHook(DataQueue* dataQueue, HANDLE hEvent) {
	ItemInjectorHook::m_dataQueue = dataQueue;
	ItemInjectorHook::m_hEvent = hEvent;
	hPipe = NULL;
	m_itemQueueEvent = NULL;
}

void ItemInjectorHook::EnableHook(void* addItemFunc, std::set<void*>* inventorySackSet) {
	this->inventorySackSet = inventorySackSet;
	dll_GameEngine_Update = (GameEngine_Update)GetProcAddress(::GetModuleHandle("Engine.dll"), "?Update@Engine@GAME@@QAEXPBVSphere@2@PBVWorldFrustum@2@_N1@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_Update, Hooked_GameEngine_Update);
	DetourTransactionCommit();


	dll_GameItem_CreateItem = (GameItem_CreateItem)GetProcAddress(::GetModuleHandle("Game.dll"), "?CreateItem@Item@GAME@@SAPAV12@ABUItemReplicaInfo@2@@Z");;
	dll_InventorySack_AddItem = (InventorySack_AddItem)addItemFunc;

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

	m_itemQueueEvent = CreateEventA(NULL, FALSE, FALSE, "IA_ITEM_LISTENER");
	m_thread = (HANDLE)_beginthread(ThreadMain, NULL, 0);
}

void ItemInjectorHook::ThreadMain(void*) {
	while (m_thread != NULL)
		Process();
}

void ItemInjectorHook::AddItem(char* in_buffer, DWORD in_bufflen) {


	char buffer[240] = { 0 };

	float positionX;
	float positionY;
	memcpy(&positionX, in_buffer, 4);
	memcpy(&positionY, in_buffer+4, 4);

	in_buffer = in_buffer + 8; // Hardcoded values below, so skip the position elements
	in_bufflen -= 8;
	
	memcpy(buffer + 76, in_buffer + 0 * 4, 4); // seed
	memcpy(buffer + 152, in_buffer + 1 * 4, 4); // relic seed
	memcpy(buffer + 180, in_buffer + 2 * 4, 4); // unknown
	memcpy(buffer + 184, in_buffer + 3 * 4, 4); // enchantSeed
	memcpy(buffer + 212, in_buffer + 4 * 4, 4); // materiaCombines

	// Split the 64 bit var into 2
	memcpy(buffer + 216, in_buffer + 5 * 4, 4); // OBS
	memcpy(buffer + 220, in_buffer + 6 * 4, 4); // OBS

	memcpy(buffer + 224, in_buffer + 7 * 4, 4); // ?
	memcpy(buffer + 228, in_buffer + 8 * 4, 4); // ?
	memcpy(buffer + 232, in_buffer + 9 * 4, 4); // stacksize
	memcpy(buffer + 236, in_buffer + 10 * 4, 4); // ?

	size_t pos = 11 * 4;
	int recordLength;

	{
		int tmp = strlen(in_buffer + pos + 4);
		DataItemPtr dataEvent(new DataItem(TYPE_Custom_AddItem, 4, (char*)&tmp));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 4, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 28, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 52, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 80, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 104, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 128, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 156, in_buffer + pos, recordLength); pos += 1 + recordLength;

	memcpy(&recordLength, in_buffer + pos, 4); pos += 4;
	setString(buffer + 188, in_buffer + pos, recordLength); pos += 1 + recordLength;

	


	// Can hook the sub_methods of CreateItem and try to see where it crashes
	void* item = dll_GameItem_CreateItem(buffer);
	if (item == 0) {
		DataItemPtr dataEvent(new DataItem(TYPE_Custom_AddItemFailed, in_bufflen, in_buffer));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}


/*

	if (false && gameEngine != NULL) {
		//void GAME::GameEngine::CreateItem(class GAME::WorldCoords const &, struct GAME::ItemReplicaInfo &)

		int zone = 562066224;
		float x = 81;
		float y = 7.73f;
		float z = 46;
		char pos[4 * 4] = { 0 };
		memcpy(pos, &zone, 4);
		memcpy(pos + 4, &x, 4);
		memcpy(pos + 8, &y, 4);
		memcpy(pos + 12, &z, 4);
		typedef void*(__cdecl *GameEngine_CreateItem)(void* engine, void* worldvec, void* replica);
		GameEngine_CreateItem f = (GameEngine_CreateItem)GetProcAddress(::GetModuleHandle("Engine.dll"), "?CreateItem@GameEngine@GAME@@QAEXABVWorldCoords@2@AAUItemReplicaInfo@2@@Z");
		f(gameEngine, pos, buffer);
	}*/

	float v2[] = { positionX , positionY };
	
	if (dll_InventorySack_AddItem(_inventorySack3, v2, item, false /* MUST be false */)) {
		// All good
		DataItemPtr dataEvent(new DataItem(TYPE_Custom_AddItemSucceeded, 0, NULL));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);		
	}
	else {
		// Make an array of size (in_bufflen+4)
		// Send it back, inserting buffer at pos 4
		DataItemPtr dataEvent(new DataItem(TYPE_Custom_AddItemFailed, in_bufflen, in_buffer));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}
}

void* ItemInjectorHook::GetStash3() {
	return _inventorySack3;
}
void ItemInjectorHook::SetStash3(void* stash3) {
	_inventorySack3 = stash3;
	// Signal that we may have an event, in case we got items queued waiting for a stash
	if (_inventorySack3 != NULL)
		SetEvent(m_itemQueueEvent);
}
// Accept clients and read data from pipe
void ItemInjectorHook::Process() {
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

	BOOL fSuccess = ReadFile(hPipe, buffer, sizeof(buffer) / sizeof(char), &numBytesRead, nullptr);

	if (fSuccess && numBytesRead > 0) {
		DataItemPtr item(new DataItem(0, numBytesRead, buffer));
		m_itemQueue.push(item);
		SetEvent(m_itemQueueEvent);

		FlushFileBuffers(hPipe);
		DisconnectNamedPipe(hPipe);
		isConnected = false;
	}
	else if (!fSuccess) {
		DisconnectNamedPipe(hPipe);
		isConnected = false;
	}

}

void ItemInjectorHook::DisableHook() {
	if (hPipe != NULL) {
		CloseHandle(hPipe);
		hPipe = NULL;
	}
	if (m_itemQueueEvent != NULL) {
		CloseHandle(m_itemQueueEvent);
		m_itemQueueEvent = NULL;
	}
	// TODO: Signal thread to close
	if (m_thread != NULL) {
		CloseHandle(m_thread);
		m_thread = NULL;
	}

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&dll_GameEngine_Update, Hooked_GameEngine_Update);
	DetourTransactionCommit();
}

void* __fastcall ItemInjectorHook::Hooked_GameEngine_Update(void* This, void* notUsed, void* s, void* f, bool b, void* f2) {
	// If we don't yet know the transfer sack, attempt to locate it.
	// TODO: Cache the function at least
	if (_inventorySack3 == NULL && gameEngine != NULL) {
		typedef int* (__thiscall *GetTransferSack)(void* ge, int index);
		GetTransferSack func = (GetTransferSack)GetProcAddress(::GetModuleHandle("Game.dll"), "?GetTransferSack@GameEngine@GAME@@QAEPAVInventorySack@2@H@Z");
		for (void* it : *inventorySackSet) {
			if (func(gameEngine, 2) == it) {
				_inventorySack3 = it;
				SetEvent(m_itemQueueEvent);
			}
		}
	}

	// Add any items waiting in the queue
	if (m_itemQueueEvent != NULL) {
		if (_inventorySack3 != NULL && WaitForSingleObject(m_itemQueueEvent, 3) == WAIT_OBJECT_0) {

			while (!m_itemQueue.empty()) {
				DataItemPtr item = m_itemQueue.pop();
				AddItem(item->data(), item->size());
			}
		}
	}
	return dll_GameEngine_Update(This, s, f, b, f2);
}