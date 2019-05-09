#include <Windows.h>
#include <stdio.h>
#include "fheaders.h"
#include "auxiliary.h"

DWORD wmain(int argc, wchar_t* argv[])
{
	PCWSTR pszLibFile = NULL;
	wchar_t *strProcName;
	DWORD dwProcessId = 0;
	DWORD dwTechnique = 0;
	DWORD v = checkOS();

	if (argc != 5)
	{
		displayHelp();
		return(0);
	}

	if (_wcsicmp(argv[1], TEXT("-t")) == 0)
	{
		strProcName = (wchar_t *)malloc((wcslen(argv[3]) + 1) * sizeof(wchar_t));
		strProcName = argv[3];

		pszLibFile = (wchar_t *)malloc((wcslen(argv[4]) + 1) * sizeof(wchar_t));
		pszLibFile = argv[4];

		dwProcessId = findPidByName(strProcName);
		if (dwProcessId == 0)
		{
			wprintf(TEXT("[-] Error: Could not find PID (%d).\n"), dwProcessId);
			return(1);
		}

                SetSePrivilege();
		
		switch (_wtoi(argv[2]))
		{
		case 1:
			return demoCreateRemoteThreadW(pszLibFile, dwProcessId);
		case 2:
			if (v < 2)
				wprintf(TEXT("[-] NtCreateThread() is only available in Windows Vista and up."));
			else
				return demoNtCreateThreadEx(pszLibFile, dwProcessId);
			break;
		case 3:
			return demoQueueUserAPC(pszLibFile, dwProcessId);
		case 4:
			return demoSetWindowsHookEx(pszLibFile, dwProcessId, strProcName);
		case 5:
			return demoRtlCreateUserThread(pszLibFile, dwProcessId);
		case 6:
#ifdef _WIN64
			return demoSuspendInjectResume64(pszLibFile, dwProcessId);
#else
			return demoSuspendInjectResume(pszLibFile, dwProcessId);
#endif
		case 7:
			return demoReflectiveDllInjection(pszLibFile, dwProcessId);
		default:
			displayHelp();
		}
	}
	else
		displayHelp();

	return(0);
}
