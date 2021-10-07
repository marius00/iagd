copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim_beta.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

Inno\iscc Inno\gdia_beta.iss

echo "Checking for unstaged changes before tagging to git.."
git diff --exit-code || exit
set-commit-tags.cmd

powershell -ExecutionPolicy Bypass -file makehash.ps1
echo All good

pause