@echo off
REM Build script for Example_RBEReport ESAPI plugin
REM This script builds the project using MSBuild

setlocal enabledelayedexpansion

REM Set project directory
set "PROJECT_DIR=%~dp0"
set "PROJECT_FILE=%PROJECT_DIR%Example_RBEReport.csproj"

REM Default to Debug configuration if not specified
if "%1"=="" (
    set "CONFIG=Debug"
) else (
    set "CONFIG=%1"
)

REM Default to x64 platform
if "%2"=="" (
    set "PLATFORM=x64"
) else (
    set "PLATFORM=%2"
)

echo ========================================
echo Building Example_RBEReport
echo Configuration: %CONFIG%
echo Platform: %PLATFORM%
echo ========================================
echo.

REM Try to find MSBuild
set "MSBUILD_PATH="

REM Check for Visual Studio 2019/2022 (common locations)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
) else if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
)

if "%MSBUILD_PATH%"=="" (
    echo ERROR: MSBuild not found!
    echo Please install Visual Studio or .NET Framework SDK
    echo Or specify MSBuild path manually in this script
    exit /b 1
)

echo Using MSBuild: %MSBUILD_PATH%
echo.

REM Build the project
"%MSBUILD_PATH%" "%PROJECT_FILE%" /p:Configuration=%CONFIG% /p:Platform=%PLATFORM% /t:Build /v:minimal

if %ERRORLEVEL% EQU 0 (
    REM Calculate absolute output path - go up two directories from project, then into plugins
    cd /d "%PROJECT_DIR%"
    cd /d "..\..\plugins" 2>nul
    if !ERRORLEVEL! EQU 0 (
        set "OUTPUT_PATH=!CD!\RBEReport.esapi.dll"
        cd /d "%PROJECT_DIR%"
    ) else (
        REM Fallback to relative path if directory doesn't exist
        set "OUTPUT_PATH=%PROJECT_DIR%..\..\plugins\RBEReport.esapi.dll"
    )
    echo.
    echo ========================================
    echo Build succeeded!
    echo Output: !OUTPUT_PATH!
    echo ========================================
    exit /b 0
) else (
    echo.
    echo ========================================
    echo Build failed!
    echo ========================================
    exit /b 1
)

