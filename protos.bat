set SOLUTION_PATH=%1
set PROJECT_PATH=%2
robocopy "%SOLUTION_PATH%/submodules/protos/ciot" "%PROJECT_PATH%/ciot" /E
if %ERRORLEVEL% LSS 9 exit /B 0