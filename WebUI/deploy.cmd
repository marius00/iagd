del /q ..\IAGrim\bin\Debug\Resources\static\css\*
del /q ..\IAGrim\bin\Debug\Resources\static\js\*
del /q ..\IAGrim\bin\Release\Resources\static\css\*
del /q ..\IAGrim\bin\Release\Resources\static\js\*
xcopy /e /y build ..\IAGrim\bin\Debug\Resources\
xcopy /e /y build ..\IAGrim\bin\Release\Resources\