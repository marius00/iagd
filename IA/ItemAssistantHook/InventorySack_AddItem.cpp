#include "stdafx.h"
#include <set>
#include <stdio.h>
#include <stdlib.h>
#include "MessageType.h"
#include <detours.h>
#include "InventorySack_AddItem.h"
#include "ItemInjectorHook.h"
#include "Globals.h"

#ifdef INSTALOOT_ENABLE
#define INSTALOOT_ENABLED 1
#else
#define INSTALOOT_ENABLED 0
#endif
#define STASH_1 0
#define STASH_2 1
#define STASH_3 2
#define STASH_4 3
#define STASH_5 4
#define STASH_PRIVATE 1000

#define REPLICA_SIZE 44

struct ItemReplica {
	// Completely ignored, never written to
	INT32 ignored0;

	// Base Record
	char* item04;
	char item04_unknown[12];
	int item04_len;
	int item04_deref;

	// Prefix Record
	char* item28;
	char item28_unknown[12];
	int item28_len;
	int item28_deref;

	// Suffix Record
	char* item52;
	char item52_unknown[12];
	int item52_len;
	int item52_deref;

	DWORD seed;

	// Modifier Record
	char* item80;
	char item80_unknown[12];
	int item80_len;
	int item80_deref;

	// Materia Record
	char* item104;
	char item104_unknown[12];
	int item104_len;
	int item104_deref;

	// RelicCompletionBonusRecord
	char* item128;
	char item128_unknown[12];
	int item128_len;
	int item128_deref;


	DWORD relicSeed;

	// transmuteOrEnchantRecord01
	char* item156;
	char item156_unknown[12];
	int item156_len;
	int item156_deref;
	
	DWORD unknown;
	DWORD enchantSeed;

	// transmuteOrEnchantRecord02
	char* item188;
	char item188_unknown[12];
	int item188_len;
	int item188_deref;

	DWORD materiaCombines;
	long long item54;

	DWORD item56;
	DWORD item57;
	DWORD stackCount;
	DWORD item59;
};
ItemReplica CreateReplica() {
	ItemReplica replica = {};
	replica.item04_len = 255;
	replica.item04 = new char[256];
	replica.item28_len = 255;
	replica.item28 = new char[256];
	replica.item52_len = 255;
	replica.item52 = new char[256];
	replica.item80_len = 255;
	replica.item80 = new char[256];
	replica.item104_len = 255;
	replica.item104 = new char[256];
	replica.item128_len = 255;
	replica.item128 = new char[256];
	replica.item156_len = 255;
	replica.item156 = new char[256];
	replica.item188_len = 255;
	replica.item188 = new char[256];

	return replica;
}
void DestroyReplica(ItemReplica replica) {
	delete replica.item04;
	delete replica.item28;
	delete replica.item52;
	delete replica.item80;
	delete replica.item104;
	delete replica.item128;
	delete replica.item156;
	delete replica.item188;
}


#define MCPY(x) { memcpy(buffer + pos, &x, sizeof(x)); pos += sizeof(x); }

int FillItemReplicaBuffer(char* buffer, size_t bufflen, void* replica_ptr) {
	ItemReplica replica;

	DWORD unknown = *((DWORD *)replica_ptr); // Those 4 bytes before the first string
	replica.seed = *((DWORD *)replica_ptr + 19); //76
	replica.relicSeed = *((DWORD *)replica_ptr + 38); //152

	replica.unknown = *((DWORD *)replica_ptr + 45); //180
	replica.enchantSeed = *((DWORD *)replica_ptr + 46); // 184

	replica.materiaCombines = *((DWORD *)replica_ptr + 53); //212
	replica.item54 = *((char*)replica_ptr + 216);

	replica.item56 = *((DWORD *)replica_ptr + 56);
	replica.item57 = *((DWORD *)replica_ptr + 57);
	replica.stackCount = *((DWORD *)replica_ptr + 58);
	replica.item59 = *((DWORD *)replica_ptr + 59); //236

	size_t pos = 0;
	MCPY(unknown);
	MCPY(replica.seed);
	MCPY(replica.relicSeed);
	MCPY(replica.unknown);
	MCPY(replica.enchantSeed);
	MCPY(replica.materiaCombines);
	MCPY(replica.item54);
	MCPY(replica.item56);
	MCPY(replica.item57);
	MCPY(replica.stackCount);
	MCPY(replica.item59);

	// Grab the memory address for comparison
	/*
	int tmp = (int)replica_ptr;
	memcpy(buffer + pos, &tmp, 4);
	pos += 4;*/


	char* item04 = (char*)replica_ptr + 4; // Confirmed, this adds up to (seed - 72)
	char* item28 = (char*)replica_ptr + 28;
	char* item52 = (char*)replica_ptr + 52;
	char* item80 = (char*)replica_ptr + 80;
	char* item104 = (char*)replica_ptr + 104;
	char* item128 = (char*)replica_ptr + 128;
	char* item156 = (char*)replica_ptr + 156;
	char* item188 = (char*)replica_ptr + 188;


	
	size_t len = 0;
	len = CopyGDString(item04, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item28, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item52, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item80, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item104, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item128, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item156, buffer + pos, bufflen - pos); pos += len;
	len = CopyGDString(item188, buffer + pos, bufflen - pos); pos += len;

	return pos;
}

int FillItemReplicaBufferFromReplica(char* buffer, size_t bufflen, ItemReplica replica) {
	size_t pos = 0;
	MCPY(replica.ignored0);
	MCPY(replica.seed);
	MCPY(replica.relicSeed);
	MCPY(replica.unknown);
	MCPY(replica.enchantSeed);
	MCPY(replica.materiaCombines);
	MCPY(replica.item54);
	MCPY(replica.item56);
	MCPY(replica.item57);
	MCPY(replica.stackCount);
	MCPY(replica.item59);
	

	MCPY(replica.item04_len);
	memcpy(buffer + pos, replica.item04, replica.item04_len); pos += replica.item04_len;

	MCPY(replica.item28_len);
	memcpy(buffer + pos, replica.item28, replica.item28_len); pos += replica.item28_len;

	MCPY(replica.item52_len);
	memcpy(buffer + pos, replica.item52, replica.item52_len); pos += replica.item52_len;

	MCPY(replica.item80_len);
	memcpy(buffer + pos, replica.item80, replica.item80_len); pos += replica.item80_len;

	MCPY(replica.item104_len);
	memcpy(buffer + pos, replica.item104, replica.item104_len); pos += replica.item104_len;

	MCPY(replica.item128_len);
	memcpy(buffer + pos, replica.item128, replica.item128_len); pos += replica.item128_len;

	MCPY(replica.item156_len);
	memcpy(buffer + pos, replica.item156, replica.item156_len); pos += replica.item156_len;

	MCPY(replica.item188_len);
	memcpy(buffer + pos, replica.item188, replica.item188_len); pos += replica.item188_len;

	return pos;
}




HANDLE InventorySack_AddItem::m_hEvent;
DataQueue* InventorySack_AddItem::m_dataQueue;
int InventorySack_AddItem::REPLICA_PTR_OFFSET;

InventorySack_AddItem::GameEngine_GetTransferSack InventorySack_AddItem::dll_GameEngine_GetTransferSack;
InventorySack_AddItem::InventorySack_AddItem01 InventorySack_AddItem::dll_InventorySack_AddItem01;
InventorySack_AddItem::InventorySack_AddItem02 InventorySack_AddItem::dll_InventorySack_AddItem02;
InventorySack_AddItem::GameEngine_AddItemToTransfer_01 InventorySack_AddItem::dll_GameEngine_AddItemToTransfer_01;
InventorySack_AddItem::GameEngine_AddItemToTransfer_02 InventorySack_AddItem::dll_GameEngine_AddItemToTransfer_02;
InventorySack_AddItem::GameEngine_SetTransferOpen InventorySack_AddItem::dll_GameEngine_SetTransferOpen;
InventorySack_AddItem::GameInfo_GameInfo_Param InventorySack_AddItem::dll_GameInfo_GameInfo_Param;
InventorySack_AddItem::GameInfo_GameInfo InventorySack_AddItem::dll_GameInfo_GameInfo;
InventorySack_AddItem::GameInfo_SetHardcore InventorySack_AddItem::dll_GameInfo_SetHardcore;
InventorySack_AddItem::GameInfo_GetHardcore InventorySack_AddItem::dll_GameInfo_GetHardcore;
InventorySack_AddItem::GameEngine_GetGameInfo InventorySack_AddItem::dll_GameEngine_GetGameInfo;
InventorySack_AddItem::InventorySack_Sort InventorySack_AddItem::dll_InventorySack_Sort;
GetPrivateStash InventorySack_AddItem::privateStashHook;

InventorySack_AddItem::Item_GetItemReplicaInfo InventorySack_AddItem::dll_GetItemReplicaInfo;
InventorySack_AddItem::InventorySack_InventorySack InventorySack_AddItem::dll_InventorySack_InventorySack;
InventorySack_AddItem::InventorySack_InventorySackParam InventorySack_AddItem::dll_InventorySack_InventorySackParam;
InventorySack_AddItem::InventorySack_Deconstruct InventorySack_AddItem::dll_InventorySack_Deconstruct;
std::set<void*> InventorySack_AddItem::inventorySacks;

InventorySack_AddItem::GameInfo_SetModName InventorySack_AddItem::dll_GameInfo_SetModName;
InventorySack_AddItem::GameInfo_SetModNameWideString InventorySack_AddItem::dll_GameInfo_SetModNameWideString;
InventorySack_AddItem::GameInfo_GetModName InventorySack_AddItem::dll_GameInfo_GetModName;

char InventorySack_AddItem::m_modName[256];
bool InventorySack_AddItem::m_hasModName = false;
int InventorySack_AddItem::m_stashToLootFrom = STASH_5;

int InventorySack_AddItem::m_isHardcore;
ItemInjectorHook InventorySack_AddItem::itemInjector;
void* InventorySack_AddItem::m_gameEngine = NULL;


bool InventorySack_AddItem::CanTransferItems() {
	bool initialized = m_isHardcore != -1;
	return initialized && FindWindow("GDIAWindowClass", NULL) != NULL;
}




// Check if this sack is the Nth transfer sack
// Zero indexed, so 3 == stash 4
bool InventorySack_AddItem::IsTransferStash(void* stash, int idx) {
	if (m_gameEngine != NULL) {
		// class GAME::InventorySack * GAME::GameEngine::GetTransferSack(int)
		//?GetTransferSack@GameEngine@GAME@@QAEPAVInventorySack@2@H@Z
		if (dll_GameEngine_GetTransferSack != NULL) {
			return dll_GameEngine_GetTransferSack(m_gameEngine, idx) == stash;
		}
	}

	return false;
}

void InventorySack_AddItem::EnableHook() {
	dll_GameEngine_GetTransferSack = (GameEngine_GetTransferSack)GetProcAddress(::GetModuleHandle("Game.dll"), "?GetTransferSack@GameEngine@GAME@@QAEPAVInventorySack@2@H@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_GetTransferSack, Hooked_GameEngine_GetTransferSack);
	DetourTransactionCommit();


	dll_InventorySack_AddItem01 = (InventorySack_AddItem01)GetProcAddress(::GetModuleHandle("Game.dll"), "?AddItem@InventorySack@GAME@@QAE_NPAVItem@2@_N1@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_AddItem01, Hooked_InventorySack_AddItem_01);
	DetourTransactionCommit();


	dll_InventorySack_AddItem02 = (InventorySack_AddItem02)GetProcAddress(::GetModuleHandle("Game.dll"), "?AddItem@InventorySack@GAME@@QAE_NABVVec2@2@PAVItem@2@_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_AddItem02, Hooked_InventorySack_AddItem_02);
	DetourTransactionCommit();


	dll_GameEngine_AddItemToTransfer_01 = (GameEngine_AddItemToTransfer_01)GetProcAddress(::GetModuleHandle("Game.dll"), "?AddItemToTransfer@GameEngine@GAME@@QAE_NIABVVec2@2@I_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_AddItemToTransfer_01, Hooked_GameEngine_AddItemToTransfer_01);
	DetourTransactionCommit();


	dll_GameEngine_AddItemToTransfer_02 = (GameEngine_AddItemToTransfer_02)GetProcAddress(::GetModuleHandle("Game.dll"), "?AddItemToTransfer@GameEngine@GAME@@QAE_NII_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameEngine_AddItemToTransfer_02, Hooked_GameEngine_AddItemToTransfer_02);
	DetourTransactionCommit();


	dll_GameEngine_SetTransferOpen = (GameEngine_SetTransferOpen)GetProcAddress(::GetModuleHandle("Game.dll"), "?SetTransferOpen@GameEngine@GAME@@QAEX_N@Z");
	if (dll_GameEngine_SetTransferOpen != NULL) {
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach((PVOID*)&dll_GameEngine_SetTransferOpen, Hooked_GameEngine_SetTransferOpen);
		DetourTransactionCommit();
	}
	else {
		DataItemPtr item(new DataItem(TYPE_ERROR_HOOKING_TRANSFER_STASH, 0, 0));
		m_dataQueue->push(item);
		SetEvent(m_hEvent);
	}


	// GameInfo::
	dll_GameInfo_GameInfo_Param = (GameInfo_GameInfo_Param)GetProcAddress(::GetModuleHandle("Engine.dll"), "??0GameInfo@GAME@@QAE@ABV01@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourTransactionCommit();

	dll_GameInfo_GameInfo = (GameInfo_GameInfo)GetProcAddress(::GetModuleHandle("Engine.dll"), "??0GameInfo@GAME@@QAE@XZ");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_GameInfo, Hooked_GameInfo_GameInfo);
	DetourTransactionCommit();

	dll_GameInfo_SetHardcore = (GameInfo_SetHardcore)GetProcAddress(::GetModuleHandle("Engine.dll"), "?SetHardcore@GameInfo@GAME@@QAEX_N@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_SetHardcore, Hooked_GameInfo_SetHardcore);
	DetourTransactionCommit();

	// No hook, just caching the call
	dll_GetItemReplicaInfo = (Item_GetItemReplicaInfo)GetProcAddress(::GetModuleHandle("Game.dll"), "?GetItemReplicaInfo@Item@GAME@@UBEXAAUItemReplicaInfo@2@@Z");

	privateStashHook.EnableHook();
	
#if MOD_FUNCTIONALITY_ENABLED
	dll_GameInfo_SetModName = (GameInfo_SetModName)GetProcAddress(::GetModuleHandle("Engine.dll"), "?SetModName@GameInfo@GAME@@QAEXABV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_SetModName, Hooked_GameInfo_SetModName);
	DetourTransactionCommit();

	dll_GameInfo_SetModNameWideString = (GameInfo_SetModNameWideString)GetProcAddress(::GetModuleHandle("Engine.dll"), "?SetModName@GameInfo@GAME@@QAEXABV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_SetModNameWideString, Hooked_GameInfo_SetModName_WideStr);
	DetourTransactionCommit();
#endif

	// Not hooking it
	dll_GameInfo_GetModName = (GameInfo_GetModName)GetProcAddress(::GetModuleHandle("Engine.dll"), "?GetModName@GameInfo@GAME@@QBEABV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@XZ");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_GameInfo_GetModName, Hooked_GameInfo_GetModName);
	DetourTransactionCommit();


	dll_InventorySack_InventorySackParam = (InventorySack_InventorySackParam)GetProcAddress(::GetModuleHandle("Game.dll"), "??0InventorySack@GAME@@QAE@ABV01@@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_InventorySackParam, Hooked_InventorySack_InventorySackParam);
	DetourTransactionCommit();

	dll_InventorySack_InventorySack = (InventorySack_InventorySack)GetProcAddress(::GetModuleHandle("Game.dll"), "??0InventorySack@GAME@@QAE@XZ");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_InventorySack, Hooked_InventorySack_InventorySack);
	DetourTransactionCommit();

	dll_InventorySack_Deconstruct = (InventorySack_Deconstruct)GetProcAddress(::GetModuleHandle("Game.dll"), "??1InventorySack@GAME@@UAE@XZ");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_Deconstruct, Hooked_InventorySack_Deconstruct);
	DetourTransactionCommit();

	dll_InventorySack_Sort = (InventorySack_Sort)GetProcAddress(::GetModuleHandle("Game.dll"), "?Sort@InventorySack@GAME@@QAE_NI@Z");
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&dll_InventorySack_Sort, Hooked_InventorySack_Sort);
	DetourTransactionCommit();



	// bool GAME::GameInfo::GetHardcore(void)
	dll_GameInfo_GetHardcore = (GameInfo_GetHardcore)GetProcAddress(::GetModuleHandle("Engine.dll"), "?GetHardcore@GameInfo@GAME@@QBE_NXZ");

	// class GAME::GameInfo * GAME::Engine::GetGameInfo(void)
	dll_GameEngine_GetGameInfo = (GameEngine_GetGameInfo)GetProcAddress(::GetModuleHandle("Engine.dll"), "?GetGameInfo@Engine@GAME@@QAEPAVGameInfo@2@XZ");

	itemInjector.EnableHook(dll_InventorySack_AddItem02, &inventorySacks);
}

InventorySack_AddItem::InventorySack_AddItem(DataQueue* dataQueue, HANDLE hEvent, DWORD stashToLootFrom) {
	REPLICA_PTR_OFFSET = FindOffset();
	InventorySack_AddItem::m_dataQueue = dataQueue;
	InventorySack_AddItem::m_hEvent = hEvent;
	itemInjector = ItemInjectorHook(dataQueue, hEvent);
	privateStashHook = GetPrivateStash(dataQueue, hEvent);

	m_isHardcore = -1;
	m_stashToLootFrom = stashToLootFrom - 1; // Stashes are zero-based indexing
}

InventorySack_AddItem::InventorySack_AddItem() {
	REPLICA_PTR_OFFSET = FindOffset();
	InventorySack_AddItem::m_hEvent = NULL;
	itemInjector = ItemInjectorHook();
	m_isHardcore = -1;
}

void InventorySack_AddItem::DisableHook() {
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());


	DetourDetach((PVOID*)&dll_GameEngine_GetTransferSack, Hooked_GameEngine_GetTransferSack);
	DetourDetach((PVOID*)&dll_InventorySack_AddItem01, Hooked_InventorySack_AddItem_01);
	DetourDetach((PVOID*)&dll_InventorySack_AddItem02, Hooked_InventorySack_AddItem_02);

	DetourDetach((PVOID*)&dll_GameEngine_AddItemToTransfer_01, Hooked_GameEngine_AddItemToTransfer_01);
	DetourDetach((PVOID*)&dll_GameEngine_AddItemToTransfer_02, Hooked_GameEngine_AddItemToTransfer_02);
	DetourDetach((PVOID*)&dll_GameEngine_SetTransferOpen, Hooked_GameEngine_SetTransferOpen);

	DetourDetach((PVOID*)&dll_GameInfo_GameInfo_Param, Hooked_GameInfo_GameInfo_Param);
	DetourDetach((PVOID*)&dll_GameInfo_GameInfo, Hooked_GameInfo_GameInfo);
	DetourDetach((PVOID*)&dll_GameInfo_SetHardcore, Hooked_GameInfo_SetHardcore);

#if MOD_FUNCTIONALITY_ENABLED
	/*DetourDetach((PVOID*)&dll_GameInfo_SetModName_01, Hooked_GameInfo_SetModName_01);
	DetourDetach((PVOID*)&dll_GameInfo_SetModName_02, Hooked_GameInfo_SetModName_02);*/
#endif

	DetourDetach((PVOID*)&dll_InventorySack_InventorySack, Hooked_InventorySack_InventorySack);
	DetourDetach((PVOID*)&dll_InventorySack_InventorySackParam, Hooked_InventorySack_InventorySackParam);
	DetourDetach((PVOID*)&dll_InventorySack_Deconstruct, Hooked_InventorySack_Deconstruct);
	DetourDetach((PVOID*)&dll_InventorySack_Sort, Hooked_InventorySack_Sort);
	
	DetourTransactionCommit();

	itemInjector.DisableHook();
	privateStashHook.DisableHook();
}

void __fastcall InventorySack_AddItem::Hooked_GameEngine_SetTransferOpen(void* This, void* notUsed, bool isOpen) {
	dll_GameEngine_SetTransferOpen(This, isOpen);
	m_gameEngine = This;
	itemInjector.gameEngine = This;



	char b[1];
	b[0] = (isOpen ? 1 : 0);
	DataItemPtr item(new DataItem(TYPE_OPEN_CLOSE_TRANSFER_STASH, 1, (char*)b));
	m_dataQueue->push(item);
	SetEvent(m_hEvent);

}


// UNUSED
void* __fastcall InventorySack_AddItem::Hooked_GameEngine_AddItemToTransfer_01(void* This, void* notUsed, unsigned int a, void* Vec2, unsigned int index, bool c) {
	char buffer[4] = { 0 };
	memcpy(buffer, &index, sizeof(index));

	DataItemPtr dataEvent(new DataItem(TYPE_GameEngine_AddItemToTransfer_01, sizeof(buffer), (char*)buffer));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);

	return dll_GameEngine_AddItemToTransfer_01(This, a, Vec2, index, c);
}

// UNUSED
void* __fastcall InventorySack_AddItem::Hooked_GameEngine_AddItemToTransfer_02(void* This, void* notUsed, unsigned int a, unsigned int index, bool c) {
	char buffer[4] = { 0 };
	memcpy(buffer, &index, sizeof(index));

	DataItemPtr dataEvent(new DataItem(TYPE_GameEngine_AddItemToTransfer_02, sizeof(buffer), (char*)buffer));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);

	return dll_GameEngine_AddItemToTransfer_02(This, a, index, c);
}

void InventorySack_AddItem::GetModNameAndTriggerMessage(void* This) {
#if MOD_FUNCTIONALITY_ENABLED
	if (INSTALOOT_ENABLED && !m_hasModName && m_gameEngine != nullptr) {
		const int stash = GetStashIndex(This);
		if (stash == STASH_1) {
			void* gameInfo = dll_GameEngine_GetGameInfo(m_gameEngine);

			DataItemPtr dataEvent = GetModName(gameInfo);
			m_dataQueue->push(dataEvent);
			SetEvent(m_hEvent);
		}
	}
#endif
}

/*
 Called primarily when dropping an item into a closed stash (eg drop item from tab 3 to tab 4)
*/
void* __fastcall InventorySack_AddItem::Hooked_InventorySack_AddItem_01(void* This, void* notUsed, void* item, bool findPosition, bool SkipPlaySound) {
	// In case IA started after GD, try to fetch the mod name now
	GetModNameAndTriggerMessage(This);

	if (INSTALOOT_ENABLED) {
		const int stash = GetStashIndex(This);
		if (stash == STASH_3) {
			itemInjector.SetStash3(This);
		}



		// Purpose: Simply notify IA that an item exists (used to map positions, and track unlooted items)
		if (stash != -1 && stash != m_stashToLootFrom) {
			void* replica_ptr = (void*)((int)item + REPLICA_PTR_OFFSET); // Replica start
			const size_t bufflen = sizeof(Vec2f) + sizeof(DWORD) + 256 + sizeof(int);
			char buffer[bufflen] = { 0 };
			size_t pos = 0;

			// Copy the stash indicator
			memcpy(buffer + pos, &stash, sizeof(stash));
			pos += sizeof(stash);

			// Where this message was sent from
			byte src = 1;
			memcpy(buffer + pos, &src, sizeof(src));
			pos += sizeof(src);

			Vec2f xz = {};
			memcpy(buffer + pos, &xz, sizeof(Vec2f));
			pos += sizeof(Vec2f);

			DWORD seed = *((DWORD *)replica_ptr + 19);
			memcpy(buffer + pos, &seed, sizeof(DWORD));
			pos += sizeof(DWORD);

			// Base record
			char* item04 = (char*)replica_ptr + 4; // Confirmed, this adds up to (seed - 72)
			pos += CopyGDString(item04, buffer + pos, bufflen - pos);

			DataItemPtr dataEvent(new DataItem(TYPE_Stash_Item_BasicInfo, pos, buffer));
			m_dataQueue->push(dataEvent);
			SetEvent(m_hEvent);
		}


		// [Stash 4] Loot the item
		if (stash == m_stashToLootFrom && CanTransferItems()) {
			const size_t replicaSize = REPLICA_SIZE + 8 * 200 + 4 /* These 4 are temporary */;
			const size_t extrasSize = sizeof(bool) * 2;
			const size_t miscSize = sizeof(bool) + 200; // The Hardcore&Mod settings
			const size_t bufflen = extrasSize + replicaSize + miscSize;
			char buffer[bufflen];
			memset(buffer, 0, sizeof(buffer));
			size_t pos = 0;

			memcpy(buffer + pos, &findPosition, sizeof(bool) * 1);
			pos += sizeof(bool);

			memcpy(buffer + pos, &SkipPlaySound, sizeof(bool) * 1);
			pos += sizeof(bool);


			// Include game mode and mod name in the transfer
			// This ensures that items arrive in the right location.
			buffer[pos++] = m_isHardcore;
			int modNameLength = 0;
			memcpy(buffer + pos, &modNameLength, 4);
			pos += 4;


			void* replica_ptr = (void*)((int)item + REPLICA_PTR_OFFSET); // Replica start
			pos += FillItemReplicaBuffer(buffer + pos, bufflen - pos, replica_ptr);

			/*
					ItemReplica replica = CreateReplica();
					dll_GetItemReplicaInfo(item, (void*)&replica);
					pos += FillItemReplicaBufferFromReplica(buffer + pos, bufflen - pos, replica);
					DestroyReplica(replica);*/

			m_dataQueue->push(DataItemPtr(new DataItem(50100, 0, NULL))); SetEvent(m_hEvent);

			{
				DataItemPtr dataEvent(new DataItem(TYPE_InventorySack_AddItem, pos, (char*)buffer));
				m_dataQueue->push(dataEvent);
				SetEvent(m_hEvent);
			}


			// Discard the item, IA will recreate it.
			return (void*)1;
		}
	}
	void* v = dll_InventorySack_AddItem01(This, item, findPosition, SkipPlaySound);
	return v;
	
}

int InventorySack_AddItem::GetStashIndex(void* stash) {
	if (stash == privateStashHook.GetPrivateStashInventorySack())
		return STASH_PRIVATE;
	else if (IsTransferStash(stash, STASH_1))
		return STASH_1;
	else if (IsTransferStash(stash, STASH_2))
		return STASH_2;
	else if (IsTransferStash(stash, STASH_3))
		return STASH_3;
	else if (IsTransferStash(stash, STASH_4))
		return STASH_4;
	else if (IsTransferStash(stash, STASH_5))
		return STASH_5;

	return -1;
}


// bool GAME::InventorySack::AddItem(class GAME::Vec2 const &,class GAME::Item *,bool)
//class std::basic_string<unsigned short,struct std::char_traits<unsigned short>,class std::allocator<unsigned short> > GAME::Item::GetGameDescription(bool,bool)
void* __fastcall InventorySack_AddItem::Hooked_InventorySack_AddItem_02(void* This, void* notUsed, Vec2f const& xz, void* item, bool a) {
	m_dataQueue->push(DataItemPtr(new DataItem(50001, 0, NULL))); SetEvent(m_hEvent);
	void* replica_ptr = (void*)((int)item + REPLICA_PTR_OFFSET); // Replica start


	// In case IA started after GD, try to fetch the mod name now
	GetModNameAndTriggerMessage(This);

	
	// THIS IS NOT RELATED TO INSTALOOT, but it uses the same crashy-functionality, so keep it off when not needed
	// [Stash 3]
	// Purpose: Notify IA of changes to transfer stash #3

	if (INSTALOOT_ENABLED) {
		const int stash = GetStashIndex(This);
		if (stash == STASH_3) {
			itemInjector.SetStash3(This);
		}

		// Purpose: Simply notify IA that an item exists (used to map positions, and track unlooted items)
		if (stash != -1 && stash != m_stashToLootFrom) {
			const size_t bufflen = sizeof(Vec2f) + sizeof(DWORD) + 256 + sizeof(int);
			char buffer[bufflen] = { 0 };
			size_t pos = 0;

			// Copy the stash indicator
			memcpy(buffer + pos, &stash, sizeof(stash));
			pos += sizeof(stash);

			// Where this message was sent from
			byte src = 2;
			memcpy(buffer + pos, &src, sizeof(src));
			pos += sizeof(src);

			memcpy(buffer + pos, &xz, sizeof(Vec2f));
			pos += sizeof(Vec2f);

			DWORD seed = *((DWORD *)replica_ptr + 19);
			memcpy(buffer + pos, &seed, sizeof(DWORD));
			pos += sizeof(DWORD);

			char* item04 = (char*)replica_ptr + 4; // Confirmed, this adds up to (seed - 72)
			pos += CopyGDString(item04, buffer + pos, bufflen - pos);

			DataItemPtr dataEvent(new DataItem(TYPE_Stash_Item_BasicInfo, pos, buffer));
			m_dataQueue->push(dataEvent);
			SetEvent(m_hEvent);
		}


		// Item dropped in Stash 4
		// Purpose: Loot item
		if (stash == m_stashToLootFrom && CanTransferItems()) {

			const size_t replicaSize = REPLICA_SIZE + 8 * 255;
			const size_t extrasSize = sizeof(Vec2f) + sizeof(bool);
			const size_t miscSize = sizeof(bool) + 200; // The Hardcore&Mod settings
			const size_t bufflen = extrasSize + replicaSize + miscSize;

			char buffer[bufflen] = { 0 };
			size_t pos = 0;

			// Copy operations
			memcpy(buffer + pos, &xz, sizeof(Vec2f));
			pos += sizeof(Vec2f);

			memcpy(buffer + pos, &a, sizeof(bool) * 1);
			pos += sizeof(bool);


			// Include game mode and mod name in the transfer
			// This ensures that items arrive in the right location.
			buffer[pos++] = m_isHardcore;
			int modNameLength = 0; //*((int*)m_modName);
			//memcpy(buffer + pos, &m_modName, 4 + modNameLength);
			memcpy(buffer + pos, &modNameLength, 4);
			pos += 4 + modNameLength;


			// Fill with juicy replica info
	/*
			ItemReplica replica = CreateReplica();
			dll_GetItemReplicaInfo(item, (void*)&replica);
			pos += FillItemReplicaBufferFromReplica(buffer + pos, bufflen - pos, replica);
			DestroyReplica(replica);*/
			pos += FillItemReplicaBuffer(buffer + pos, bufflen - pos, replica_ptr);


			DataItemPtr dataEvent(new DataItem(TYPE_InventorySack_AddItem_Vec2, pos, (char*)buffer));
			m_dataQueue->push(dataEvent);
			SetEvent(m_hEvent);

			// Discard the item, IA will recreate it.
			return (void*)1;
		}
	}

	// Just continue with the regular functionality =)
	void* v = dll_InventorySack_AddItem02(This, xz, item, a);
	return v;
	
	
}

DataItemPtr InventorySack_AddItem::GetModName(void* GameInfoInstance) {
	char* modName = (char*)dll_GameInfo_GetModName(GameInfoInstance);

	const size_t bufsize = 200;
	char buffer[bufsize] = { 0 };

	int bytes = CopyGDString(modName, buffer, bufsize);

	// Store the mod name locally
	memcpy_s(modName, 255, buffer, bufsize);
	m_hasModName = true; // TODO:

	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_SetModName, min(bufsize, bytes), (char*)&buffer));
	return dataEvent;
}
#if MOD_FUNCTIONALITY_ENABLED

//void GAME::GameInfo::SetModName(class std::basic_string<char, struct std::char_traits<char>, class std::allocator<char> > const &)
/*void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetModName_01(void* This, void* notUsed, void* stdString) {
	{	// DEBUG
		DataItemPtr dataEvent(new DataItem(1050, 0, 0));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}

	char buffer[256] = { 0 };
	int bufsize = sizeof(buffer) / sizeof(char);
	int bytes = CopyGDString((char*)stdString, buffer, bufsize);

	// Store the mod name locally
	memcpy(m_modName, buffer, bytes);
	m_hasModName = true;

	// Notify IA
	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_SetModName, min(bufsize, bytes), (char*)&buffer));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);


	void* result = dll_GameInfo_SetModName_01(This, stdString);
	return result;
}*/

// Appears to be unused, only one reference to it in IDA, and never seen it called.
//void GAME::GameInfo::SetModName(class std::basic_string<unsigned short, struct std::char_traits<unsigned short>, class std::allocator<unsigned short> > const &)
/*void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetModName_02(void* This, void* notUsed, void* stdString) {
	{	// DEBUG
		DataItemPtr dataEvent(new DataItem(1010, 0, 0));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}
	void* result = dll_GameInfo_SetModName_02(This, stdString);

	DataItemPtr dataEvent = GetModName(This);
	if (dataEvent != NULL) {
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}

	return result;
}
*/
#endif

/*
* Called when the game starts and when the game mode changes
* Reading the contents of the string will crash the game during startup / loading of char selection (safe after startup/load)
*/
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetModName(void* This, void* notUsed, void* stdString) {
	void* result = dll_GameInfo_SetModName(This, stdString);
	
	if (stdString != NULL) {
		char buffer[281] = { 0 };
		int written = 4;//CopyGDString((char*)stdString, buffer, 280);

		DataItemPtr dataEvent(new DataItem(1010, written, buffer));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);

		//memcpy_s(&m_modName, 256, buffer, 280);
		//m_hasModName = true;
	}
	return result;
}

/*
 * This method seems to be called while joining a crucible game (other times as well?)
 */
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetModName_WideStr(void* This, void* notUsed, void* stdStringWide) {
	{	// DEBUG
		DataItemPtr dataEvent(new DataItem(1011, 0, 0));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}
	void* result = dll_GameInfo_SetModNameWideString(This, stdStringWide);

	return result;
}



// Since were creating from an existing object we'll need to call Get() on isHardcore and ModLabel
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_GameInfo_Param(void* This, void* notUsed, void* info) {
	void* result = dll_GameInfo_GameInfo_Param(This, info);

	{
		bool isHardcore = dll_GameInfo_GetHardcore(This);
		DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore_via_init, sizeof(isHardcore), (char*)&isHardcore));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
		m_isHardcore = isHardcore ? 1 : 0;
	}

	return result;
}
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_GameInfo(void* This, void* notUsed) {
	void* result = dll_GameInfo_GameInfo(This);

	{
		DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore_via_init_2, 0, 0));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}

	return result;
}


//void GAME::GameInfo::SetHardcore(bool)
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_SetHardcore(void* This, void* notUsed, bool isHardcore) {
	DataItemPtr dataEvent(new DataItem(TYPE_GameInfo_IsHardcore, sizeof(isHardcore), (char*)&isHardcore));
	m_dataQueue->push(dataEvent);
	SetEvent(m_hEvent);
	m_isHardcore = isHardcore ? 1 : 0;

	return dll_GameInfo_SetHardcore(This, isHardcore);
}


// Keep our stash list updated
void* __fastcall InventorySack_AddItem::Hooked_InventorySack_InventorySackParam(void* This, void* Other) {
	void* result = dll_InventorySack_InventorySackParam(This, Other);
	inventorySacks.insert(This);

	m_dataQueue->push(DataItemPtr(new DataItem(50002, 0, NULL))); 
	SetEvent(m_hEvent);

	return result;
}

void* __fastcall InventorySack_AddItem::Hooked_InventorySack_InventorySack(void* This) {
	void* result = dll_InventorySack_InventorySack(This);
	inventorySacks.insert(This);

	m_dataQueue->push(DataItemPtr(new DataItem(50003, 0, NULL))); 
	SetEvent(m_hEvent);

	return result;
}

void* __fastcall InventorySack_AddItem::Hooked_InventorySack_Deconstruct(void* This) {
	if (This == itemInjector.GetStash3())
		itemInjector.SetStash3(NULL);
	inventorySacks.erase(inventorySacks.find(This));

	m_dataQueue->push(DataItemPtr(new DataItem(50004, 0, NULL))); SetEvent(m_hEvent);

	return dll_InventorySack_Deconstruct(This);
}

// When stash 3 is sorted, IA no longer knows where items are placed
bool __fastcall InventorySack_AddItem::Hooked_InventorySack_Sort(void* This, void* notUsed, unsigned int unknown) {
	if (IsTransferStash(This, 2)) { // TODO: This is now dynamic....
		DataItemPtr dataEvent(new DataItem(TYPE_InventorySack_Sort, 0, 0));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}

	m_dataQueue->push(DataItemPtr(new DataItem(50005, 0, NULL))); SetEvent(m_hEvent);

	return dll_InventorySack_Sort(This, unknown);
}

int* __fastcall InventorySack_AddItem::Hooked_GameEngine_GetTransferSack(void* This, void* discarded, int idx) {
	if (idx == STASH_PRIVATE || idx == STASH_1 || idx == STASH_2 || idx == STASH_3 || idx == STASH_4 || idx == STASH_5) {
		DataItemPtr dataEvent(new DataItem(TYPE_RequestRestrictedSack, sizeof(idx), (char*)&idx));
		m_dataQueue->push(dataEvent);
		SetEvent(m_hEvent);
	}
	int* result = dll_GameEngine_GetTransferSack(This, idx);
	return result;
}

// TODO: Make this a third dll (or replace the vanilla one) which only activates once the mod is known, otherwise ignores (eg first attempt always goes file, rest goes insta)
void* __fastcall InventorySack_AddItem::Hooked_GameInfo_GetModName(void* This, void* discarded) {
	void* result = dll_GameInfo_GetModName(This);
	return result;
}