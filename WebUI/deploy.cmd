del /q ..\IAGrim\bin\Debug\net10.0-windows\Resources\static\css\*
del /q ..\IAGrim\bin\Debug\net10.0-windows\Resources\static\js\*
del /q ..\\IAGrim\bin\Release\net10.0-windows\Resources\static\css\*
del /q ..\\IAGrim\bin\Release\net10.0-windows\Resources\static\js\*
xcopy /e /y build ..\IAGrim\bin\Debug\net10.0-windows\Resources\
xcopy /e /y build ..\IAGrim\bin\Release\net10.0-windows\Resources\


pause