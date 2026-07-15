@powershell "(Get-Item -path "iagrim\bin\release\net10.0-windows\iagrim.exe").VersionInfo.FileVersion" > ver.txt
set /p IA_VERSION=<ver.txt
del ver.txt

git tag %IA_VERSION%
git push origin %IA_VERSION%
