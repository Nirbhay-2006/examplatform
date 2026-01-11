@echo off
echo Starting Exam Platform Backend...
cd /d "e:\sem6\ExamPlatform\Backend\ExamPlatformApi"
dotnet run --project ExamPlatformApi.csproj --urls "http://localhost:5172"
pause