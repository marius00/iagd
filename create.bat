copy inno\vcredist_x86.exe IAGrim\bin\release\
copy inno\NDP461-KB3102438-Web.exe IAGrim\bin\release\
copy inno\2010sp1_vcredist_x86.exe IAGrim\bin\release\
copy inno\2015rc3_vc_redist.x86.exe IAGrim\bin\release\


copy /y IAGrim\bin\release\IAGrim.exe Installer\IAGrim.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

Inno\iscc Inno\gdia.iss