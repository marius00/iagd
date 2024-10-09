copy inno\vcredist_x86.exe IAGrim\bin\release\
copy inno\ndp472-kb4054531-web.exe IAGrim\bin\release\
copy inno\2010sp1_vcredist_x86.exe IAGrim\bin\release\
copy inno\2015rc3_vc_redist.x86.exe IAGrim\bin\release\
copy inno\vc2015_redist.x64.exe IAGrim\bin\release\


copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

Inno\iscc Inno\gdia.iss

echo "Checking for unstaged changes before tagging to git.."
set-commit-tags.cmd

pause