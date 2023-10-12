call setup.bat
@if not %errorLevel% == 0 (exit /b %errorLevel%)

if exist "%buildDirectory%" rmdir /s /q "%buildDirectory%"
@if not %errorLevel% == 0 (exit /b %errorLevel%)
