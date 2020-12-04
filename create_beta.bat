@echo off
certutil -hashfile IAGrim\bin\Release\libcef.dll MD5 > current-cefsharp-hash.txt
fc installer\expected-cefsharp.hash current-cefsharp-hash.txt > nul
if errorlevel 1 goto error




copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim_beta.exe
copy /y IAGrim\resources\help.html Installer\help.html
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

Inno\iscc Inno\gdia_beta.iss

echo All good
goto end

:error
echo Did someone forgot this cannot be used after updating libcef?
pause


:end
del /q current-cefsharp-hash.txt