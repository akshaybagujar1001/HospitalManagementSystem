@echo off
echo ========================================
echo   Hospital Management System - Backend
echo ========================================
echo.

cd /d "%~dp0HMS.API"

echo [1/3] Restoring NuGet packages...
call dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo [2/3] Building project...
call dotnet build
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo [3/3] Starting backend server...
echo.
echo Backend will be available at:
echo   - HTTP:  http://localhost:5001
echo   - HTTPS: https://localhost:7001
echo   - Swagger: http://localhost:5001
echo.
echo Press Ctrl+C to stop the server
echo.

call dotnet run

pause

