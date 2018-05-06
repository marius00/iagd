copy inno\vcredist_x64.exe IAGrim\bin\release\
copy inno\vcredist_x86.exe IAGrim\bin\release\
copy inno\NDP461-KB3102438-Web.exe IAGrim\bin\release\
copy inno\2010sp1_vcredist_x86.exe IAGrim\bin\release\




Inno\iscc Inno\gdia_beta.iss
copy IAGrim\bin\release\IAGrim.exe Installer\IAGrim_beta.exe