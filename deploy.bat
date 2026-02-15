@echo off
REM Copies everything under build\ to a user-provided destination folder.
REM Usage: deploy.bat <destination_folder>
REM Example: deploy.bat D:\Deploy\ESAPI

setlocal

set "ROOT=%~dp0"
set "BUILD=%ROOT%build"
set "DEST=%~1"

if "%DEST%"=="" (
    echo Usage: %~nx0 ^<destination_folder^>
    echo Example: %~nx0 D:\Deploy\ESAPI
    exit /b 1
)

if not exist "%BUILD%" (
    echo ERROR: Build folder not found: %BUILD%
    exit /b 1
)

echo Copying build output to: %DEST%
echo/

mkdir "%DEST%" 2>nul
robocopy "%BUILD%" "%DEST%" /E /IS /IT /NFL /NDL /NJH /NJS

REM Robocopy: 0-7 = success, 8+ = failure
if errorlevel 8 (
    echo/
    echo Robocopy reported errors.
    exit /b 1
)

echo/
echo Deploy complete: %DEST%
exit /b 0
