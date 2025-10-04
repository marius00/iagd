@powershell "(Get-Item -path "iagrim\bin\release\net9.0-windows7.0\iagrim.exe").VersionInfo.FileVersion" > ver.txt
set /p IA_VERSION=<ver.txt
del ver.txt

git tag %IA_VERSION%
git push origin %IA_VERSION%

type nul > Installer\dummy-%IA_VERSION%
scp Installer\dummy-%IA_VERSION% storage@grimdawn.evilsoft.net:/home/storage/grimdawn.evilsoft.net/