#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

/************************************************************************
/************************************************************************/
class CanBePlacedInTransferStashHook : public BaseMethodHook {
public:
	CanBePlacedInTransferStashHook();
	CanBePlacedInTransferStashHook(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	struct Vec3f {
		float x, y, z, u;
	};
	typedef bool (__thiscall *Item_CanPlaceTransfer)(void* This);

	static HANDLE m_hEvent;
	static DataQueue* m_dataQueue;

	static Item_CanPlaceTransfer dll_Item_CanPlaceTransfer;
	static Item_CanPlaceTransfer dll_QuestItem_CanPlaceTransfer;
	
	static bool __fastcall Hooked_Item_CanPlaceTransfer(void* This, void* notUsed);
};