@powershell "(Get-Item -path "Hook\x64\Release\ItemAssistantHook_x64.dll").VersionInfo.ProductVersion" > ver.txt
@set /p IA_VERSION=<ver.txt
@del ver.txt
echo %IA_VERSION% > ..\iagrim\bin\debug\net10.0-windows\dllver.txt


xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.dll ..\IAGrim\bin\Debug\net10.0-windows\ItemAssistantHook_x64.dll
xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Debug\net10.0-windows\ItemAssistantHook_x64.pdb
xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.dll ..\IAGrim\bin\Debug\net10.0-windows\ItemAssistantHook_playtest_x64.dll

pause