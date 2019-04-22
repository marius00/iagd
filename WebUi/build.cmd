set CI=true
echo npm run test && npm run build
npm run build
xcopy /e /y build ..\IAGrim\bin\Debug\Resources\

pause