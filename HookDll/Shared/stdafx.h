// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#define STRICT
#define _WTL_USE_CSTRING

#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
#define _WIN32_WINNT 0x0501
#define WINVER 0x0501

#include <Windows.h>
#include <tchar.h>

//#include <atlbase.h>        // Base ATL classes
//#include <atlapp.h>
//#include <atlwin.h>         // ATL windowing classes
//#include <atlframe.h>
//#include <atlctrls.h>
//#include <atldlgs.h>
//#include <atlctrlw.h>
//#include <atlmisc.h>
//#include <atlcrack.h>
//#include <atltypes.h>
//#include <atlcoll.h>

#include <stdio.h>

#include <boost/smart_ptr.hpp>
#include <boost/lexical_cast.hpp>
