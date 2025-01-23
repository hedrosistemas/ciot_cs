robocopy "../submodules/protos/ciot" "ciot" /E /XO
if %ERRORLEVEL% LSS 9 exit /B 0