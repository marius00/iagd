copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim_beta.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

set-commit-tags.cmd
Inno\iscc Inno\gdia_beta.iss

echo All good
