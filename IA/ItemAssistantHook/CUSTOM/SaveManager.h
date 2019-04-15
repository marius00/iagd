#pragma once
#include <windows.h>
#include "ItemAssistantHook/DataQueue.h"
#include "ItemAssistantHook/BaseMethodHook.h"

class DataQueue;

/************************************************************************
/************************************************************************/
class SaveManager : public BaseMethodHook {
public:
	SaveManager();
	SaveManager(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	//bool GAME::SaveManager::DirectRead(class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > const &,void * &,unsigned int &,bool,bool)
	typedef int(__thiscall *OriginalMethodPtr)(void* This, void* filename, void const* dstBuf, unsigned int numBytesToRead, bool unknown1, bool unknown2);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


#if defined(_AMD64_)
	static int __fastcall HookedMethod(void* This, void* filename, void const* dstBuf, unsigned int numBytesToRead, bool unknown1, bool unknown2);
#else
	static int __fastcall HookedMethod(void* This, void* notUsed, void* filename, void const* dstBuf, unsigned int numBytesToRead, bool unknown1, bool unknown2);
#endif

};

//SAVE_MANAGER_DIRECTREAD
