@echo off
setlocal

REM Build script for test_esapi_standalone without requiring Visual Studio.

REM --- Configuration ---
set CONFIG=Debug
set PLATFORM=x64
set PROJECT=esapi_autocontour.csproj

REM --- Locate MSBuild (prefer 64-bit .NET Framework 4.x) ---
set MSBUILD_EXE=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
if not exist "%MSBUILD_EXE%" (
  set MSBUILD_EXE=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
)

if not exist "%MSBUILD_EXE%" (
  echo MSBuild not found in expected .NET Framework locations.
  echo Please adjust MSBUILD_EXE in this script to point to a valid MSBuild.exe.
  exit /b 1
)

REM --- Kill any running instances to prevent file lock ---
set EXE_NAME=autocontour.esapi.dll
tasklist /FI "IMAGENAME eq %EXE_NAME%" 2>NUL | find /I /N "%EXE_NAME%">NUL
if "%ERRORLEVEL%"=="0" (
  echo Closing running instance of %EXE_NAME%...
  taskkill /F /IM %EXE_NAME% >NUL 2>&1
  timeout /t 1 /nobreak >NUL
)

echo Using MSBuild: %MSBUILD_EXE%
echo Building %PROJECT% (Configuration=%CONFIG%, Platform=%PLATFORM%)...

"%MSBUILD_EXE%" "%PROJECT%" /p:Configuration=%CONFIG% /p:Platform=%PLATFORM%
set BUILD_RC=%ERRORLEVEL%

if %BUILD_RC% neq 0 (
  echo Build failed with exit code %BUILD_RC%.
  exit /b %BUILD_RC%
)

echo Build succeeded.
echo Output should be under bin\%CONFIG:~0,1%%CONFIG:~1%\ (e.g., bin\debug\ or bin\release\).

exit /b 0


