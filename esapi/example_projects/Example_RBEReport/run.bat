@echo off
REM Run script for Example_RBEReport ESAPI plugin
REM This script builds the project and provides information about running it

setlocal enabledelayedexpansion

set "PROJECT_DIR=%~dp0"
set "DLL_NAME=RBEReport.esapi.dll"

REM Resolve absolute path for DLL
cd /d "%PROJECT_DIR%"
cd /d "..\..\plugins" 2>nul
if !ERRORLEVEL! EQU 0 (
    set "DLL_PATH=!CD!\!DLL_NAME!"
    cd /d "%PROJECT_DIR%"
) else (
    set "DLL_PATH=%PROJECT_DIR%..\..\plugins\%DLL_NAME%"
)

echo ========================================
echo Example_RBEReport ESAPI Plugin
echo ========================================
echo.

REM First, build the project
echo Building project...
call "%PROJECT_DIR%build.bat" Debug

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Build failed. Cannot proceed.
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build completed successfully!
echo ========================================
echo.

REM Check if the output DLL exists
if not exist "!DLL_PATH!" (
    echo WARNING: Output DLL not found at expected location:
    echo !DLL_PATH!
    echo.
    echo Please check the build output above for errors.
    pause
    exit /b 1
)

echo Output DLL: !DLL_PATH!
echo.

REM Check if Eclipse is installed
set "ECLIPSE_PATH="
if exist "C:\Program Files (x86)\Varian\RTM\16.1\Eclipse\Eclipse.exe" (
    set "ECLIPSE_PATH=C:\Program Files (x86)\Varian\RTM\16.1\Eclipse\Eclipse.exe"
) else if exist "C:\Program Files (x86)\Varian\RTM\15.6\Eclipse\Eclipse.exe" (
    set "ECLIPSE_PATH=C:\Program Files (x86)\Varian\RTM\15.6\Eclipse\Eclipse.exe"
) else if exist "C:\Program Files (x86)\Varian\RTM\15.5\Eclipse\Eclipse.exe" (
    set "ECLIPSE_PATH=C:\Program Files (x86)\Varian\RTM\15.5\Eclipse\Eclipse.exe"
)

echo ========================================
echo Plugin Installation Instructions:
echo ========================================
echo.
echo The plugin DLL has been built and should be located at:
echo !DLL_PATH!
echo.
echo To use this plugin in Eclipse:
echo 1. Ensure the DLL is in the plugins directory
echo 2. Launch Eclipse (Varian Treatment Planning System)
echo 3. The plugin should be available in the Scripts menu
echo.

if not "%ECLIPSE_PATH%"=="" (
    echo Eclipse found at: %ECLIPSE_PATH%
    echo.
    set /p LAUNCH="Do you want to launch Eclipse now? (Y/N): "
    if /i "%LAUNCH%"=="Y" (
        echo.
        echo Launching Eclipse...
        start "" "%ECLIPSE_PATH%"
    )
) else (
    echo Eclipse executable not found in common locations.
    echo Please launch Eclipse manually to use the plugin.
)

echo.
pause

