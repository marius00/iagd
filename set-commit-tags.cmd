powershell "(Get-Item -path "iagrim\bin\release\iagrim.exe").VersionInfo.ProductVersion" > ver.txt
set /p IA_VERSION=<ver.txt
del ver.txt

git tag %IA_VERSION%
git push origin %IA_VERSION%