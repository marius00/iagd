copy /y IAGrim\bin\Release\net9.0-windows7.0\\IAGrim.exe Installer\IAGrim_beta.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

"Inno\iscc" Inno\gdia_beta.iss

Inno\iscc Inno\gdia_beta.iss

pause