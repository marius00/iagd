
@powershell "(Get-Item -path "Hook\x64\Release\ItemAssistantHook_x64.dll").VersionInfo.ProductVersion" > ver.txt
@set /p IA_VERSION=<ver.txt
@del ver.txt


echo %IA_VERSION% > ..\iagrim\bin\release\net9.0-windows7.0\dllver.txt
echo %IA_VERSION% > ..\IAGrim\bin\Debug\net9.0-windows7.0\dllver.txt

xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.dll ..\IAGrim\bin\Debug\net9.0-windows7.0\ItemAssistantHook_x64.dll
xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.dll ..\IAGrim\bin\Release\net9.0-windows7.0\ItemAssistantHook_x64.dll

xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Debug\net9.0-windows7.0\ItemAssistantHook_x64.pdb
xcopy /Y Hook\x64\Release\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Release\net9.0-windows7.0\ItemAssistantHook_x64.pdb

xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.dll ..\IAGrim\bin\Debug\net9.0-windows7.0\ItemAssistantHook_playtest_x64.dll
xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.dll ..\IAGrim\bin\Release\net9.0-windows7.0\ItemAssistantHook_playtest_x64.dll
xcopy /Y Hook\x64\Release-playtest\ItemAssistantHook_x64.pdb ..\IAGrim\bin\Release\net9.0-windows7.0\ItemAssistantHook_playtest_x64.pdb

pause


