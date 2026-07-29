fnm use
SET BUILD_TARGET=GDIA

REM The output folders live under win-x64 since IAGrim.csproj gained a RuntimeIdentifier.
SET DEBUG_RES=..\IAGrim\bin\Debug\net10.0-windows\win-x64\Resources
SET RELEASE_RES=..\IAGrim\bin\Release\net10.0-windows\win-x64\Resources

REM "call" is required. npm is npm.cmd, and invoking a batch file from a batch file
REM without call hands over control permanently - every line below this one is
REM silently discarded, which is why Release never received the bundle.
call npm run build
if errorlevel 1 exit /b 1

del /q build\assets\*.png

REM Filenames are content-hashed, so a plain copy would pile up every old bundle
REM alongside the current one. Drop the previous copy first.
if exist "%DEBUG_RES%\assets" rmdir /s /q "%DEBUG_RES%\assets"
if exist "%RELEASE_RES%\assets" rmdir /s /q "%RELEASE_RES%\assets"

xcopy /e /y build "%DEBUG_RES%\"
xcopy /e /y build "%RELEASE_RES%\"

pause
