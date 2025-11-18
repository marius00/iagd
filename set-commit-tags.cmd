@powershell "(Get-Item -path "iagrim\bin\release\net10.0-windows\iagrim.exe").VersionInfo.FileVersion" > ver.txt
set /p IA_VERSION=<ver.txt
del ver.txt

git tag %IA_VERSION%
git push origin %IA_VERSION%

type nul > Installer\dummy-%IA_VERSION%
scp Installer\dummy-%IA_VERSION% storage@grimdawn.evilsoft.net:/home/storage/grimdawn.evilsoft.net/