# PowerShell script to start both Backend and Frontend reliably with logging
param()

# Resolve script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

# Create logs folder
$logsDir = Join-Path $scriptDir 'logs'
if (-not (Test-Path $logsDir)) { New-Item -ItemType Directory -Path $logsDir | Out-Null }

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXAM PLATFORM - START ALL SERVICES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check MongoDB but do not abort automatically; warn and continue
Write-Host "[1/5] Checking MongoDB..." -ForegroundColor Yellow
try {
    $mongoService = Get-Service -Name "MongoDB" -ErrorAction SilentlyContinue
    if ($mongoService -and $mongoService.Status -eq 'Running') {
        Write-Host "✓ MongoDB is running" -ForegroundColor Green
    } else {
        Write-Host "⚠ MongoDB service not running or not installed. Backend may fail to start if DB is unavailable." -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠ Could not determine MongoDB service status. Proceeding..." -ForegroundColor Yellow
}
Write-Host ""

# Kill existing backend process to avoid locked file errors
Write-Host "[2/5] Ensuring no previous backend process is running..." -ForegroundColor Yellow
try {
    $existing = Get-Process -Name 'ExamPlatformApi' -ErrorAction SilentlyContinue
    if ($existing) {
        foreach ($p in $existing) {
            Write-Host "Found running ExamPlatformApi process (PID: $($p.Id)). Stopping..." -ForegroundColor Yellow
            Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
        }
        Start-Sleep -Seconds 1
        Write-Host "Stopped previous backend process." -ForegroundColor Green
    } else {
        Write-Host "No running backend process found." -ForegroundColor Green
    }
} catch {
    Write-Host "Warning: failed to stop existing process (permission issue)." -ForegroundColor Yellow
}
Write-Host ""

# Start Backend in a new PowerShell window with logging
Write-Host "[3/5] Starting Backend API (logs\backend.log)..." -ForegroundColor Yellow
$backendPath = Join-Path $scriptDir 'Backend\ExamPlatformApi'
$backendLog = Join-Path $logsDir 'backend.log'
$backendCommand = "Set-Location -LiteralPath '$backendPath'; dotnet run --project ExamPlatformApi.csproj --urls 'http://localhost:5172;https://localhost:7163' 2>&1 | Tee-Object -FilePath '$backendLog'"
Start-Process -FilePath powershell -ArgumentList @('-NoExit','-Command',$backendCommand) -WindowStyle Normal | Out-Null
Write-Host "Backend process started in new window." -ForegroundColor Green
Write-Host ""

# Start Frontend in a new PowerShell window with logging
Write-Host "[4/5] Starting Frontend (logs\frontend.log)..." -ForegroundColor Yellow
$frontendPath = Join-Path $scriptDir 'Frontend'
$frontendLog = Join-Path $logsDir 'frontend.log'
$frontendCommand = "Set-Location -LiteralPath '$frontendPath'; npm start 2>&1 | Tee-Object -FilePath '$frontendLog'"
Start-Process -FilePath powershell -ArgumentList @('-NoExit','-Command',$frontendCommand) -WindowStyle Normal | Out-Null
Write-Host "Frontend process started in new window." -ForegroundColor Green
Write-Host ""

# Poll backend health endpoint until ready (or timeout)
Write-Host "[5/5] Waiting for backend to be ready (polling /api/health)..." -ForegroundColor Yellow
$maxRetries = 30
$retry = 0
$healthy = $false
while ($retry -lt $maxRetries) {
    try {
        $r = Invoke-WebRequest -Uri 'http://localhost:5172/api/health' -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
        if ($r.StatusCode -eq 200) {
            $healthy = $true
            break
        }
    } catch {
        # ignore and retry
    }
    Start-Sleep -Seconds 2
    $retry++
}

if (-not $healthy) {
    Write-Host "Backend did not become healthy within timeout. Check logs\backend.log for details." -ForegroundColor Red
    Write-Host "Tail the backend log with: Get-Content -Path '$backendLog' -Wait" -ForegroundColor Yellow
} else {
    Write-Host "Backend is healthy." -ForegroundColor Green
    Write-Host "Opening browser windows..." -ForegroundColor Yellow
    Start-Process 'http://localhost:4200'
    Start-Sleep -Seconds 1
    Start-Process 'http://localhost:5172/swagger'
}

Write-Host ""
Write-Host "Start script completed. Logs are available in the 'logs' folder." -ForegroundColor Cyan
Write-Host "Backend log: $backendLog" -ForegroundColor White
Write-Host "Frontend log: $frontendLog" -ForegroundColor White
Write-Host ""
Write-Host "Keep the backend and frontend windows open to see runtime logs. Close them to stop services." -ForegroundColor Green

# Do not block the calling shell; end script
exit 0

