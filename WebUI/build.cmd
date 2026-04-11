fnm use
SET BUILD_TARGET=GDIA
npm run build && del build\assets\*.png && xcopy /e /y build ..\IAGrim\bin\Debug\net10.0-windows\Resources\ && pause
