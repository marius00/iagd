
@powershell "(Get-Item -path "..\iagrim\bin\release\iagrim.exe").VersionInfo.ProductVersion" > ver.txt
@set /p IA_RELEASE_VERSION=<ver.txt
@del ver.txt

@powershell "(Get-Item -path "..\iagrim\bin\debug\iagrim.exe").VersionInfo.ProductVersion" > ver.txt
@set /p IA_DEBUG_VERSION=<ver.txt
@del ver.txt

if %IA_DEBUG_VERSION% gtr %IA_RELEASE_VERSION% (SET IA_VERSION=%IA_DEBUG_VERSION%) else (SET IA_VERSION=%IA_RELEASE_VERSION%)


@echo #define IAGD_VER "%IA_VERSION%" >> .\hook\resource.h
