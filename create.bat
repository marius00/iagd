copy inno\vcredist_x86.exe IAGrim\bin\Release\net10.0-windows\
copy inno\2010sp1_vcredist_x86.exe IAGrim\bin\Release\net10.0-windows\


copy /y IAGrim\bin\Release\net10.0-windows\IAGrim.exe Installer\IAGrim.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

Inno\iscc Inno\gdia.iss

echo "Checking for unstaged changes before tagging to git.."
set-commit-tags.cmd

pause