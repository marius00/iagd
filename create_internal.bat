copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim_beta.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt


Inno\iscc Inno\gdia_beta.iss

pause