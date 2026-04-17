@set "SCRIPTDIR=%~dp0"

@powershell "(Get-Item -path '%SCRIPTDIR%..\iagrim\bin\release\net10.0-windows\iagrim.exe').VersionInfo.FileVersion" > "%SCRIPTDIR%ver.txt"
@set /p IA_RELEASE_VERSION=<"%SCRIPTDIR%ver.txt"
@del "%SCRIPTDIR%ver.txt"

@powershell "(Get-Item -path '%SCRIPTDIR%..\iagrim\bin\debug\net10.0-windows\iagrim.exe').VersionInfo.FileVersion" > "%SCRIPTDIR%ver.txt"
@set /p IA_DEBUG_VERSION=<"%SCRIPTDIR%ver.txt"
@del "%SCRIPTDIR%ver.txt"

if %IA_DEBUG_VERSION% gtr %IA_RELEASE_VERSION% (SET IA_VERSION=%IA_DEBUG_VERSION%) else (SET IA_VERSION=%IA_RELEASE_VERSION%)

@powershell "(Get-Content '%SCRIPTDIR%Hook\resource.h') | Where-Object { $_ -notmatch '#define IAGD_VER' } | Set-Content '%SCRIPTDIR%Hook\resource.h'"
@echo #define IAGD_VER "%IA_VERSION%" >> "%SCRIPTDIR%Hook\resource.h"
