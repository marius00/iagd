#pragma once

class Mutex
{
   HANDLE d_mutex;

public:
   Mutex()
   {
#if defined( _WIN32 ) && defined( _MT )
      // This mutex will help the two threads share their toys.
      d_mutex = CreateMutex( NULL, false, NULL );
      //if( d_mutex == NULL )
      //   throw cError( "cMonitor::cMonitor() - Mutex creation failed." );
#endif
   }


   virtual ~Mutex()
   {
#if defined( _WIN32 ) && defined( _MT )
      if( d_mutex != NULL )
      {
         CloseHandle( d_mutex );
         d_mutex = NULL;
      }
#endif
   }


   void MutexOn() const
   {
#if defined( _WIN32 ) && defined( _MT )
      WaitForSingleObject( d_mutex, INFINITE );  // To be safe...
#endif
   }


   void MutexOff() const
   {
#if defined( _WIN32 ) && defined( _MT )
      ReleaseMutex( d_mutex );  // To be safe...
#endif
   }
};
