@powershell "(Get-Item -path "iagrim\bin\release\iagrim.exe").VersionInfo.ProductVersion" > ver.txt
@set /p IA_VERSION=<ver.txt
@echo 1 > Installer\dummy-%IA_VERSION%
@scp Installer\dummy-%IA_VERSION% storage@grimdawn.evilsoft.net:/home/storage/grimdawn.evilsoft.net/
@del ver.txt
