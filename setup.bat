@set buildDirectory=build
@set objDirectory=%buildDirectory%\obj
@set binDirectory=%buildDirectory%\bin
@set convertDirectory=%buildDirectory%\convert
@set projectName=gbOS
@set gameTitle=GBOS
@set gameId=OSGE
@set newLicensee=ZZ
@set padValue=0xFF
@set compileOptions=%compileOptions% -Wall -Wextra -Werror --halt-without-nop --preserve-ld --include "%convertDirectory%\src" --include "src"
@set linkOptions=%linkOptions%
@set sourceFiles=%sourceFiles% src/display.z80 src/header.z80 src/input.z80 src/main.z80 src/memory.z80 src/system.z80 src/vBlank.z80 src/graphics.z80 src/lcdStat.z80 src/gui.z80 src/serial.z80 src/palettes.z80 src/application.z80

@rem @set linkOptions=%linkOptions% --tiny
@rem @set compileOptions=%compileOptions% -D __TINY_MODE

@set compileOptions=%compileOptions% -D RomBank_autoLoad=1

@call setup.user.bat
