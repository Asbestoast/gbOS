@call make.bat
@if not %errorLevel% == 0 (
    pause
    exit /b %errorLevel%
)
@call run.bat
