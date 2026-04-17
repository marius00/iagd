copy /y IAGrim\bin\Release\net10.0-windows\IAGrim.exe Installer\IAGrim.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

"D:\programs\Inno\iscc" Inno\gdia.iss

echo "Checking for unstaged changes before tagging to git.."
set-commit-tags.cmd

pause