@echo off
setlocal EnableDelayedExpansion
title Exam Platform - Start Backend and Frontend
color 0A

echo.
echo ========================================
echo   EXAM PLATFORM - STARTING SERVICES
echo ========================================
echo.

REM Get script directory
set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM Ensure logs folder exists
if not exist "%SCRIPT_DIR%logs" mkdir "%SCRIPT_DIR%logs"

REM --- Check .NET SDK ---
echo [1/4] Checking .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK not found.
    echo Install .NET 8 SDK from https://dotnet.microsoft.com/download and try again.
    pause
    exit /b 1
)
for /f "tokens=1" %%v in ('dotnet --version') do set "DOTNETVER=%%v"
echo [OK] .NET SDK: %DOTNETVER%
echo.

REM --- Check Node.js ---
echo [2/4] Checking Node.js...
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Node.js not found.
    echo Install Node.js from https://nodejs.org/ and try again.
    pause
    exit /b 1
)
for /f "tokens=1" %%v in ('node --version') do set "NODEVER=%%v"
echo [OK] Node.js: %NODEVER%
echo.

REM --- MongoDB info (no privileged commands) ---
echo [3/4] MongoDB info...
echo This script does NOT start MongoDB automatically.
echo Ensure MongoDB is running BEFORE you continue:
echo   - If Windows service:   run ^"net start MongoDB^" in an admin PowerShell.
echo   - If using Docker:      from ^"Backend^" folder run ^"docker-compose up -d mongodb^".
echo.

REM --- Start backend and frontend ---
echo [4/4] Starting backend and frontend...

REM Start Backend API (ExamPlatformApi.csproj) in new PowerShell window
if exist "%SCRIPT_DIR%Backend\ExamPlatformApi\ExamPlatformApi.csproj" (
    echo [INFO] Starting Backend API on http://localhost:5172 ...
    start "Exam Platform - Backend" powershell -NoExit -Command "Set-Location -LiteralPath '%SCRIPT_DIR%Backend\ExamPlatformApi'; dotnet run --project ExamPlatformApi.csproj --urls 'http://localhost:5172;https://localhost:7163'"
) else (
    echo [ERROR] Backend project not found at Backend\ExamPlatformApi\ExamPlatformApi.csproj
)
echo.

REM Start Frontend (npm start) in new PowerShell window
if exist "%SCRIPT_DIR%Frontend\package.json" (
    echo [INFO] Starting Frontend on http://localhost:4200 ...
    start "Exam Platform - Frontend" powershell -NoExit -Command "Set-Location -LiteralPath '%SCRIPT_DIR%Frontend'; npm start"
) else (
    echo [ERROR] Frontend project not found at Frontend\package.json
)
echo.

echo Opening browser to frontend and Swagger (if backend becomes ready)...
start "" http://localhost:4200
start "" http://localhost:5172/swagger

echo.
echo ========================================
echo   BACKEND:  http://localhost:5172
echo   FRONTEND: http://localhost:4200
echo   SWAGGER:  http://localhost:5172/swagger
echo ========================================
echo.

echo Services are running in separate PowerShell windows.
echo Close those windows to stop the services.

pause
exit /b 0
