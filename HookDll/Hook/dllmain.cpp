#include "stdafx.h"
#include <windows.h>
#include <stdlib.h>
//#include <boost/thread.hpp>
#include "DataQueue.h"
#include "MessageType.h"
#include "StateRequestNpcAction.h"
#include "StateRequestMoveAction.h"
#include "CloudGetNumFiles.h"
#include "CloudRead.h"
#include "CloudWrite.h"
#include "InventorySack_AddItem.h"
#include "NpcDetectionHook.h"
#include "SaveTransferStash.h"
#include "Exports.h"
#include "CanUseDismantle.h"
#include "ExperimentalSeed.h"

#pragma region Variables
// Switches hook logging on/off
#if 1
#include "HookLog.h"
HookLog g_log;
#define LOG(streamdef) \
{ \
    std::string msg = (((std::ostringstream&)(std::ostringstream().flush() << streamdef)).str()); \
    g_log.out(msg); \
    msg += _T("\n"); \
    OutputDebugString(msg.c_str()); \
}
#else
#define LOG(streamdef) \
    __noop;
#endif


DWORD g_lastThreadTick = 0;
HANDLE g_hEvent;
HANDLE g_thread;

DataQueue g_dataQueue;

HWND g_targetWnd = NULL;

#pragma endregion

#pragma region CORE


/// Thread function that dispatches queued message blocks to the AOIA application.
void WorkerThreadMethod() {
    while ((g_hEvent != NULL) && (WaitForSingleObject(g_hEvent,INFINITE) == WAIT_OBJECT_0)) {
        if (g_hEvent == NULL) {
            break;
        }

        DWORD tick = GetTickCount();
        if (tick < g_lastThreadTick) {
            // Overflow
            g_lastThreadTick = tick;
        }

        if ((tick - g_lastThreadTick > 1000) || (g_targetWnd == NULL)) {
            // We either don't have a valid window target OR it has been more than 1 sec since we last update the target.
            g_targetWnd = FindWindow( "GDIAWindowClass", NULL);
            g_lastThreadTick = GetTickCount();
            LOG("FindWindow returned: " << g_targetWnd);
        }

        while (!g_dataQueue.empty()) {
            DataItemPtr item = g_dataQueue.pop();

            if (g_targetWnd == NULL) {
                // We have data, but no target window, so just delete the message
                continue;
            }

            COPYDATASTRUCT data;
            data.dwData = item->type();
            data.lpData = item->data();
            data.cbData = item->size();

            // To avoid blocking the main thread, we should not have a lock on the queue while we process the message.
			SendMessage( g_targetWnd, WM_COPYDATA, 0, ( LPARAM ) &data );
            LOG("After SendMessage error code is " << GetLastError());
        }
    }
}

unsigned __stdcall WorkerThreadMethodWrap(void* argss) {
	WorkerThreadMethod();
	return 0;
}
void StartWorkerThread() {
	LOG("Starting worker thread..");
	unsigned int pid;
	g_thread = (HANDLE)_beginthreadex(NULL, 0, &WorkerThreadMethodWrap, NULL, 0, &pid);
	

	DataItemPtr item(new DataItem(TYPE_REPORT_WORKER_THREAD_LAUNCHED, 0, NULL));
	g_dataQueue.push(item);
	SetEvent(g_hEvent);
	LOG("Started worker thread..");
}


void EndWorkerThread() {
	LOG("Ending worker thread..");
	if (g_hEvent != NULL) {
		SetEvent(g_hEvent);
		HANDLE h = g_hEvent;
		g_hEvent = NULL;
		CloseHandle(h);

		//WaitForSingleObject(g_thread, INFINITE);
		CloseHandle(g_thread);
	}
}

#pragma endregion

static void ConfigurePlayerPositionHooks(std::vector<BaseMethodHook*>& hooks) {
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_MOVETO));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_IDLE));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_LONG_IDLE));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_MOVE_AND_SKILL));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_MOVE_ACTOR));

	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_MOVE_TO_SKILL));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_PICKUP));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_TALK_TO_NPC));
	hooks.push_back(new StateRequestMoveAction(&g_dataQueue, g_hEvent, REQUEST_MOVE_ACTION_SKILL));

	// For these, the target position could actually be the smuggler.
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_IDLE));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_MOVETO));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_JUMP_TO_SKILL));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_LONG_IDLE));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_MOVE_AND_SKILL));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_MOVE_TO_ACTOR));	
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_MOVE_TO_SKILL));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_PICKUP_ITEM));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_TALK_TO_NPC));
	hooks.push_back(new StateRequestNpcAction(&g_dataQueue, g_hEvent, REQUEST_NPC_ACTION_MOVE_TO_NPC));	
}

// Cloud detection (is cloud enabled?) hooks
static void ConfigureCloudDetectionHooks(std::vector<BaseMethodHook*>& hooks) {
	hooks.push_back(new CloudGetNumFiles(&g_dataQueue, g_hEvent));
	hooks.push_back(new CloudRead(&g_dataQueue, g_hEvent));
	hooks.push_back(new CloudWrite(&g_dataQueue, g_hEvent));
}

static void ConfigureStashDetectionHooks(std::vector<BaseMethodHook*>& hooks) {
	// Stash detection hooks
	hooks.push_back(new NpcDetectionHook(&g_dataQueue, g_hEvent));
	hooks.push_back(new CanUseDismantle(&g_dataQueue, g_hEvent));	
	hooks.push_back(new SaveTransferStash(&g_dataQueue, g_hEvent));
	hooks.push_back(new InventorySack_AddItem(&g_dataQueue, g_hEvent)); // Includes GetPrivateStash internally
}

void DoLog(const wchar_t* staaaaa) {
	LOG(staaaaa);
}
std::vector<BaseMethodHook*> hooks;
int ProcessAttach(HINSTANCE _hModule) {
	LOG("Attatching to process..");
	g_hEvent = CreateEvent(NULL,FALSE,FALSE,"IA_Worker");

	LOG("Preparing hooks..");
	// Player position hooks
	ConfigurePlayerPositionHooks(hooks);
	ConfigureCloudDetectionHooks(hooks);
	ConfigureStashDetectionHooks(hooks);

	hooks.push_back(new ExperimentalSeed(&g_dataQueue, g_hEvent, &g_log));

	std::stringstream msg;
	msg << "Starting hook enabling.. " << hooks.size() << " hooks.";
	LOG(msg);
	for (unsigned int i = 0; i < hooks.size(); i++) {
		LOG("Enabling hook..");
		hooks[i]->EnableHook();
	}
	LOG("Hooking complete..");

	
    StartWorkerThread();
	LOG("Existing initialization..");
    return TRUE;
}


#pragma region Attach_Detatch
int ProcessDetach( HINSTANCE _hModule ) {
	// Signal that we are shutting down
	// This message is not at all guaranteed to get sent.

	OutputDebugString("ProcessDetach");


	for (unsigned int i = 0; i < hooks.size(); i++) {
		hooks[i]->DisableHook();
		delete hooks[i];
	}
	hooks.clear();

    EndWorkerThread();
    return TRUE;
}

BOOL APIENTRY DllMain(HINSTANCE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
        return ProcessAttach( hModule );

	case DLL_PROCESS_DETACH:
        return ProcessDetach( hModule );
	}
    return TRUE;
}
#pragma endregion

