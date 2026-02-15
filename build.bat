@echo off
REM Builds all projects under ./projects that have a build.bat.
REM Usage: build.bat [Debug|Release]
REM Optional: pass Debug or Release; default is Debug.

setlocal enabledelayedexpansion

set "ROOT=%~dp0"
set "PROJECTS_DIR=%ROOT%projects"
set "CONFIG=%~1"
if "%CONFIG%"=="" set "CONFIG=Debug"

echo ========================================
echo Building all projects (Configuration: %CONFIG%)
echo ========================================
echo/

set "FAILED=0"
for /D %%D in ("%PROJECTS_DIR%\*") do (
    if exist "%%D\build.bat" (
        set "NAME=%%~nxD"
        echo ----------------------------------------
        echo Building: !NAME!
        echo ----------------------------------------
        pushd "%%D"
        call build.bat %CONFIG%
        if errorlevel 1 set "FAILED=1"
        popd
        echo/
    )
)

echo ========================================
if %FAILED%==1 (
    echo One or more builds failed.
    exit /b 1
)
echo All builds succeeded.
echo ========================================
exit /b 0
