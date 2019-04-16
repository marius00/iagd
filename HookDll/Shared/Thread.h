#pragma once

#define WINDOWS_LEAN_AND_MEAN
#include <windows.h>


class Thread
{
protected:
  HANDLE  d_threadHandle;
  DWORD   d_threadID;
  bool    d_bIsRunning;

public:
  Thread();
  virtual ~Thread();

  void Begin();
  void End();
  bool IsRunning() { return d_bIsRunning; }

  virtual DWORD ThreadProc();
};
