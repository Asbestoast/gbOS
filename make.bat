@call setup.bat
@if not %errorLevel% == 0 exit /b %errorLevel%

@set objFiles=

@call :convertAssets
@if not %errorLevel% == 0 exit /b %errorLevel%

@call :assembleFiles %sourceFiles%
@if not %errorLevel% == 0 exit /b %errorLevel%

@if not exist "%binDirectory%" mkdir "%binDirectory%"
@if not %errorLevel% == 0 exit /b %errorLevel%

"%rgblinkPath%" ^
    %linkOptions% ^
    --pad %padValue% ^
    --sym "%binDirectory%\%projectName%.sym" ^
    --map "%binDirectory%\%projectName%.map" ^
    --output "%binDirectory%\%projectName%.gb" ^
    %objFiles%
@if not %errorLevel% == 0 exit /b %errorLevel%

"%rgbfixPath%" ^
    --pad-value %padValue% ^
    --title "%gameTitle%" ^
    --new-licensee "%newLicensee%" ^
    --game-id "%gameId%" ^
    --fix-spec lhg "%binDirectory%\%projectName%.gb"
@if not %errorLevel% == 0 exit /b %errorLevel%

@echo.
@echo --------------------------------
@echo Build completed successfully.
@exit /b 0

:convertAssets
"%gbConvertPath%" "." "%convertDirectory%"
@if not %errorLevel% == 0 exit /b %errorLevel%

@if not exist "%convertDirectory%\obj" mkdir "%convertDirectory%\obj"
@if not %errorLevel% == 0 exit /b %errorLevel%

rgbasm %compileOptions% --output "%convertDirectory%\obj\assets.o" "%convertDirectory%\src\assets.z80"
@if not %errorLevel% == 0 exit /b %errorLevel%

@set objFiles=%objFiles% "%convertDirectory%\obj\assets.o"

@exit /b 0

:assembleFile
@if not exist "%~dp2" mkdir "%~dp2"
@if not %errorLevel% == 0 exit /b %errorLevel%

"%rgbasmPath%" %compileOptions% --output "%~2" "%~1"
@if not %errorLevel% == 0 exit /b %errorLevel%

@set objFiles=%objFiles% "%~2"

@exit /b 0

:assembleFiles
@if "%~1"=="" exit /b 0
@call :assembleFile "%~1" "%objDirectory%\%~1.o"
@if not %errorLevel% == 0 exit /b %errorLevel%
@shift
@goto :assembleFiles
