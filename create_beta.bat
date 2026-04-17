copy /y IAGrim\bin\Release\net10.0-windows\IAGrim.exe Installer\IAGrim_beta.exe
copy /y %appdata%\..\local\evilsoft\iagd\tags_ia.template.txt installer\tags_ia.txt

"D:\programs\Inno\iscc" Inno\gdia_beta.iss

set-commit-tags.cmd

echo All good

pause