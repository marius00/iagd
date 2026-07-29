@rem FileVersion is a numeric win32 resource and can't carry the zero-padded revision,
@rem so read ProductVersion (AssemblyInformationalVersion) and strip the +commitsha suffix.
@powershell -NoProfile -Command "(Get-Item -path 'iagrim\bin\release\net10.0-windows\win-x64\iagrim.exe').VersionInfo.ProductVersion.Split('+')[0]" > ver.txt
set /p IA_VERSION=<ver.txt
del ver.txt

git tag %IA_VERSION%
git push origin %IA_VERSION%
