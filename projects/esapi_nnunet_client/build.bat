@echo off
REM Build script for esapi_nnunet_client ESAPI plugin
REM This script builds the project using MSBuild

setlocal enabledelayedexpansion

REM Set project directory
set "PROJECT_DIR=%~dp0"
set "PROJECT_FILE=%PROJECT_DIR%esapi_nnunet_client.csproj"

REM Default to Debug configuration if not specified
REM Normalize configuration name (case-insensitive)
if "%1"=="" (
    set "CONFIG=Debug"
) else (
    set "CONFIG=%1"
    REM Normalize to proper case
    if /i "%CONFIG%"=="debug" set "CONFIG=Debug"
    if /i "%CONFIG%"=="release" set "CONFIG=Release"
)

REM Default to x64 platform
if "%2"=="" (
    set "PLATFORM=x64"
) else (
    set "PLATFORM=%2"
)

echo ========================================
echo Building esapi_nnunet_client
echo Configuration: %CONFIG%
echo Platform: %PLATFORM%
echo ========================================
echo.

REM Try to find MSBuild (prioritize newer versions that can handle modern project formats)
set "MSBUILD_PATH="

REM Check for Visual Studio 2026 (version 19) - prioritize this first
if exist "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
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
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
)

if "%MSBUILD_PATH%"=="" (
    REM Last resort: .NET Framework MSBuild (may have issues with newer project formats)
    if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (
        set "MSBUILD_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
    ) else if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" (
        set "MSBUILD_PATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    )
    if "%MSBUILD_PATH%"=="" (
        echo ERROR: MSBuild not found!
        echo Please install Visual Studio Build Tools 2017 or newer.
        exit /b 1
    ) else (
        echo ERROR: Only old .NET Framework 4.0 MSBuild found. This version does not support modern C# features.
        echo Please install Visual Studio Build Tools 2017 or newer.
        exit /b 1
    )
)
REM When a VS MSBuild was found above, keep it (do not overwrite with Framework 4.0 MSBuild)


echo Using MSBuild: %MSBUILD_PATH%
echo.

echo.
echo Building main project (esapi_nnunet_client)...
echo.

REM Clean obj folder to remove stale NuGet package references
if exist "%PROJECT_DIR%obj" (
    echo Cleaning obj folder to remove stale NuGet references...
    rmdir /s /q "%PROJECT_DIR%obj" 2>nul
)

REM Build the main project
REM Skip NuGet package restore since we're using local DLLs
"%MSBUILD_PATH%" "%PROJECT_FILE%" /p:Configuration=%CONFIG% /p:Platform=%PLATFORM% /p:RestorePackages=false /p:SkipRestorePackages=true /p:DisableImplicitNuGetFallbackFolder=true /p:ResolveNuGetPackages=false /t:Build /v:minimal

if errorlevel 1 goto :build_failed
echo.
echo ========================================
echo Build succeeded!
REM Output path - Debug uses network path, Release uses local bin\release
if /i "%CONFIG%"=="Debug" (
    echo Output: Network path (as configured in project)
    echo Local copy may be in: %PROJECT_DIR%bin\debug\esapi_nnunet_client.exe
) else (
    echo Output: %PROJECT_DIR%bin\release\esapi_nnunet_client.exe
)
echo ========================================
exit /b 0

:build_failed
echo.
echo ========================================
echo Build failed!
echo ========================================
exit /b 1

