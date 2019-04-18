#pragma once
#pragma once
#include <windows.h>
#include "DataQueue.h"
#include "BaseMethodHook.h"

class DataQueue;

/************************************************************************
/************************************************************************/
class ReadPlayerTransfer : public BaseMethodHook {
public:
	ReadPlayerTransfer();
	ReadPlayerTransfer(DataQueue* dataQueue, HANDLE hEvent);
	void EnableHook();
	void DisableHook();

private:
	//bool GAME::ReadPlayerTransfer::DirectRead(class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > const &,void * &,unsigned int &,bool,bool)
	typedef int(__thiscall *OriginalMethodPtr)(void* This, void* checkedReader);
	static HANDLE m_hEvent;
	static OriginalMethodPtr originalMethod;
	static DataQueue* m_dataQueue;


#if defined(_AMD64_)
	static int __fastcall HookedMethod(void* This, void* checkedReader);
#else
	static int __fastcall HookedMethod(void* This, void* notUsed, void* checkedReader);
#endif

};
