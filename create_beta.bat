copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim_beta.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

"C:\Program Files (x86)\Inno Setup 6\iscc" Inno\gdia_beta.iss

echo "Checking for unstaged changes before tagging to git.."
git diff --exit-code || exit
set-commit-tags.cmd

echo All good

pause