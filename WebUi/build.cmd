set CI=true
npm run test && npm run build
xcopy /e /y build ..\IAGrim\bin\Debug\Resources\

pause