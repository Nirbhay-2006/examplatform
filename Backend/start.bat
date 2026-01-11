@echo off
echo ========================================
echo   EXAM PLATFORM - BACKEND STARTUP
echo ========================================
echo.

echo [1/4] Checking MongoDB...
net start MongoDB >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ MongoDB is running
) else (
    echo ✗ MongoDB not running. Starting...
    net start MongoDB
)
echo.

echo [2/4] Navigating to project...
cd ExamPlatformApi
if %errorlevel% neq 0 (
    echo ✗ Failed to navigate to project directory
    pause
    exit /b 1
)
echo ✓ In project directory
echo.

echo [3/4] Restoring packages...
dotnet restore ExamPlatformApi.csproj
if %errorlevel% neq 0 (
    echo [WARNING] Package restore had issues, but continuing...
)
echo ✓ Packages ready
echo.

echo [4/4] Starting API...
echo.
echo ========================================
echo   BACKEND API STARTING
echo ========================================
echo.
echo   Project File: ExamPlatformApi.csproj
echo   API URL:      http://localhost:5172
echo   HTTPS URL:    https://localhost:7163
echo   Swagger UI:  http://localhost:5172/swagger
echo.
echo ========================================
echo   Press Ctrl+C to stop the server
echo ========================================
echo.

dotnet run --project ExamPlatformApi.csproj
