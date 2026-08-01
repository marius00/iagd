@rem Read ProductVersion (AssemblyInformationalVersion), not FileVersion: FileVersion is a numeric win32
@rem resource and can't carry the zero-padded revision, so it would produce a dummy- filename that doesn't
@rem match the git tag cut by set-commit-tags.cmd. Strip the +commitsha suffix.
@powershell -NoProfile -Command "(Get-Item -path 'iagrim\bin\release\net10.0-windows\win-x64\iagrim.exe').VersionInfo.ProductVersion.Split('+')[0]" > ver.txt
@set /p IA_VERSION=<ver.txt
@type nul > Installer\dummy-%IA_VERSION%
@scp Installer\dummy-%IA_VERSION% storage@grimdawn.evilsoft.net:/home/storage/grimdawn.evilsoft.net/
@del ver.txt
