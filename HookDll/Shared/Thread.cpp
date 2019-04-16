#include "stdafx.h"
#include "Thread.h"


static DWORD WINAPI StaticThreadProc( Thread *pThis )
{
   return pThis->ThreadProc();
}


Thread::Thread()
{
   d_threadID = 0;
   d_threadHandle = NULL;
   d_bIsRunning = false;
}


Thread::~Thread()
{
   End();
}


void Thread::Begin()
{
#if defined( _WIN32 ) && defined( _MT )
   if( d_threadHandle )
      End();  // just to be safe.

   // Start the thread.
   d_threadHandle = CreateThread( NULL, 0, (LPTHREAD_START_ROUTINE)StaticThreadProc, this, 0, (LPDWORD)&d_threadID );
   if( d_threadHandle == NULL )
   {
      // Arrooga! Dive, dive!  And deal with the error, too!
   }
   d_bIsRunning = true;
#endif
}


DWORD Thread::ThreadProc()
{
   return 0;
}


void Thread::End()
{
#if defined( _WIN32 ) && defined( _MT )
   if( d_threadHandle != NULL )
   {
      d_bIsRunning = false;
      WaitForSingleObject( d_threadHandle, INFINITE );
      CloseHandle( d_threadHandle );
      d_threadHandle = NULL;
   }
#endif
}
