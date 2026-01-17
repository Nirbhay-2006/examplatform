@echo off
echo Starting Exam Platform...

echo Starting Backend Service (Database + API)...
start "ExamPlatform Backend" cmd /k "cd Backend && start.bat"

echo Starting Frontend Service (Angular)...
start "ExamPlatform Frontend" cmd /k "cd Frontend && npm start"

echo Exam Platform Services Initiated.
