#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class CreatePlayerItemHook : public BaseMethodHook {
public:
	CreatePlayerItemHook();
	CreatePlayerItemHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x, y, z, u;
	};
	typedef void*(__thiscall *GameEngine_CreateItem)(void* This, Vec3f const & WorldCoords, void* ItemReplicaInfo);
	typedef void*(__thiscall *GameEngine_CreateItemForCharacter)(void* This, unsigned int unknown, Vec3f const& WorldCoords, void* ItemReplicaInfo, void* someString);
	typedef void*(__thiscall *GameEngineInboundInterface_CreateItem)(void* This, unsigned int unknown, Vec3f const& WorldCoords, void* ItemReplicaInfo, void* someString);

	static HANDLE m_hEvent;
	static DataQueue* m_dataQueue;
	
	static GameEngineInboundInterface_CreateItem dll_GameEngineInboundInterface_CreateItem;
	static GameEngine_CreateItem dll_GameEngine_CreateItem;
	static GameEngine_CreateItemForCharacter dll_GameEngine_CreateItemForCharacter;
	static void SignalItem(byte src, Vec3f const& WorldCoords, void* ItemReplicaInfo);

	static void* __fastcall Hooked_GameEngine_CreateItem(void* This, void* notUsed, Vec3f const & WorldCoords, void* ItemReplicaInfo);
	static void* __fastcall Hooked_GameEngine_CreateItemForCharacter(void* This, void* notUsed, unsigned int unknown, Vec3f const& WorldCoords, void* ItemReplicaInfo, void* someString);
	static void* __fastcall Hooked_GameEngineInboundInterface_CreateItem(void* This, void* notUsed, unsigned int unknown, Vec3f const& WorldCoords, void* ItemReplicaInfo, void* someString);

};