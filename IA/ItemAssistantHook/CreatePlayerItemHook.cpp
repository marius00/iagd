#include "CreatePlayerItemHook.h"
#include <detours.h>
#include "Globals.h"
#include "MessageType.h"

HANDLE CreatePlayerItemHook::m_hEvent;
DataQueue* CreatePlayerItemHook::m_dataQueue;
CreatePlayerItemHook::GameEngine_CreateItem CreatePlayerItemHook::dll_GameEngine_CreateItem;
CreatePlayerItemHook::GameEngine_CreateItemForCharacter CreatePlayerItemHook::dll_GameEngine_CreateItemForCharacter;
CreatePlayerItemHook::GameEngineInboundInterface_CreateItem CreatePlayerItemHook::dll_GameEngineInboundInterface_CreateItem;

CreatePlayerItemHook::CreatePlayerItemHook(DataQueue* dataQueue, HANDLE hEvent) {
	CreatePlayerItemHook::m_dataQueue = dataQueue;
	CreatePlayerItemHook::m_hEvent = hEvent;
}

CreatePlayerItemHook::CreatePlayerItemHook() {
	CreatePlayerItemHook::m_hEvent = NULL;
}

void CreatePlayerItemHook::EnableHook() {
	dll_GameEngine_CreateItem = (GameEngine_CreateItem)GetProcAddress(::GetModuleHandle("Game.dll"), "?CreateItem@GameEngine@GAME@@QAEXABVWorldCoords@2@AAUItemReplicaInfo@2@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_CreateItem, Hooked_GameEngine_CreateItem);
	DetourTransactionCommit();


	dll_GameEngine_CreateItemForCharacter = (GameEngine_CreateItemForCharacter)GetProcAddress(::GetModuleHandle("Game.dll"), "?CreateItemForCharacter@GameEngine@GAME@@QAEXIABVWorldCoords@2@AAUItemReplicaInfo@2@PAV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_CreateItemForCharacter, Hooked_GameEngine_CreateItemForCharacter);
	DetourTransactionCommit();
}

void CreatePlayerItemHook::DisableHook() {
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());

	DetourDetach((PVOID*)&dll_GameEngine_CreateItem, Hooked_GameEngine_CreateItem);
	DetourDetach((PVOID*)&dll_GameEngine_CreateItemForCharacter, Hooked_GameEngine_CreateItemForCharacter);

	DetourTransactionCommit();
}
void CreatePlayerItemHook::SignalItem(byte src, Vec3f const& WorldCoords, void* ItemReplicaInfo) {
	void* replica_ptr = ItemReplicaInfo;
	const size_t bufflen = sizeof(Vec3f) + sizeof(byte) + 260 + sizeof(DWORD);
	char buffer[bufflen] = { 0 };
	size_t pos = 0;

	// Where this message was sent from
	memcpy(buffer + pos, &src, sizeof(src));
	pos += sizeof(src);

	memcpy(buffer + pos, &WorldCoords, sizeof(Vec3f));
	pos += sizeof(Vec3f);

	DWORD seed = *((DWORD *)replica_ptr + 19);
	memcpy(buffer + pos, &seed, sizeof(DWORD));
	pos += sizeof(DWORD);

	// Base record
	char* item04 = (char*)replica_ptr + 4;
	pos += CopyGDString(item04, buffer + pos, bufflen - pos);

	DataItemPtr dataEvent(new DataItem(TYPE_WorldSpawnItem, pos, buffer));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);
}

void* __fastcall CreatePlayerItemHook::Hooked_GameEngine_CreateItem(void* This, void* notUsed, Vec3f const & WorldCoords, void* ItemReplicaInfo) {
	SignalItem(1, WorldCoords, ItemReplicaInfo);

	return dll_GameEngine_CreateItem(This, WorldCoords, ItemReplicaInfo);
}
void* __fastcall CreatePlayerItemHook::Hooked_GameEngine_CreateItemForCharacter(void* This, void* notUsed, unsigned int unknown, Vec3f const& WorldCoords, void* ItemReplicaInfo, void* someString) {
	SignalItem(2, WorldCoords, ItemReplicaInfo);
	return dll_GameEngine_CreateItemForCharacter(This, unknown, WorldCoords, ItemReplicaInfo, someString);
}
void* __fastcall CreatePlayerItemHook::Hooked_GameEngineInboundInterface_CreateItem(void* This, void* notUsed, unsigned int unknown, Vec3f const& WorldCoords, void* ItemReplicaInfo, void* someString) {
	SignalItem(3, WorldCoords, ItemReplicaInfo);
	return dll_GameEngineInboundInterface_CreateItem(This, unknown, WorldCoords, ItemReplicaInfo, someString);
}