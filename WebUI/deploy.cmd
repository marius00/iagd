del /q ..\IAGrim\bin\Debug\net9.0-windows7.0\Resources\static\css\*
del /q ..\IAGrim\bin\Debug\net9.0-windows7.0\Resources\static\js\*
del /q ..\\IAGrim\bin\Release\net9.0-windows7.0\Resources\static\css\*
del /q ..\\IAGrim\bin\Release\net9.0-windows7.0\Resources\static\js\*
xcopy /e /y build ..\IAGrim\bin\Debug\net9.0-windows7.0\Resources\
xcopy /e /y build ..\IAGrim\bin\Release\net9.0-windows7.0\Resources\


pause