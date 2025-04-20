
@powershell "(Get-Item -path "..\iagrim\bin\release\iagrim.exe").VersionInfo.ProductVersion" > ver.txt
@set /p IA_VERSION=<ver.txt
@del ver.txt


echo %IA_VERSION% > ..\iagrim\bin\release\dllver.txt
echo %IA_VERSION% >> .\hook\resource.h

xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.dll ..\IAGrim\bin\Debug\ItemAssistantHook_x64.dll
xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.dll ..\IAGrim\bin\Release\ItemAssistantHook_x64.dll

xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Debug\ItemAssistantHook_x64.pdb
xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Release\ItemAssistantHook_x64.pdb

xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.dll ..\IAGrim\bin\Debug\ItemAssistantHook_playtest_x64.dll
xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.dll ..\IAGrim\bin\Release\ItemAssistantHook_playtest_x64.dll
xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Release\ItemAssistantHook_playtest_x64.pdb

pause


