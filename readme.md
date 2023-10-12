# gbOS

gbOS is a mock OS for the Game Boy and Game Boy Color. It features a desktop environment user interface and (mock) serial mouse support.

[gbOS.webm](https://github.com/Asbestoast/gbOS/assets/141359640/f28f3dc1-f070-494b-a9aa-dfc7370e5e18)

<b>Warning:</b> This is only a proof of concept and does not offer any real functionality. Also, the source code is very much thrown together and not meant to serve as a realistic example of how an OS, or its GUI subsystem, should function (that would be far more complicated).

It's just for fun.

## Controls

- <b>D-Pad</b> - Cursor movement
- <b>A</b> - Left mouse button
- <b>B</b> - Right mouse button
- <b>Start</b> - Opens the on-screen keyboard (non-functional)
- <b>Select</b> - Redraws the desktop

## Serial Device Emulator

Serial Device Emulator (SDE) emulates a mouse-like peripheral connected over the link port. It is compatible with the emulator BGB.

<b>To use SDE, follow these steps:</b>
- Open BGB and load the ROM for gbOS
- Right click inside the BGB window and select `Link > Listen` from the menu
- Open SDE and select `Link > Connect` from the menu

The mouse in gbOS should now be controllable by moving or clicking the mouse within the SDE window.

<b>Warning:</b> If either BGB or SDE lose their connection, then they may become unable to reconnect to each other. If this happens, then restart BGB and retry the steps listed above.

## Build Instructions (Windows)

### Building GBConvert (required)

- Open `GBConvert\GBConvert.sln` in Visual Studio
- Set the startup project to `GBConvert`
- Select `Build > Build Solution` from the menu

### Building Serial Device Emulator (optional)

- Open `SerialDeviceEmulator\SerialDeviceEmulator.sln` in Visual Studio
- Set the startup project to `SerialDeviceEmulator`
- Select `Build > Build Solution` from the menu

### Building gbOS (required)

- Ensure that all of gbOS's build dependencies are present. The following are required:
    - RGBDS
    - GBConvert
- Make a copy of `setup.user.model.bat` and name it `setup.user.bat`
- Edit `setup.user.bat` and fill in the appropriate fields
- Run either `make and pause.bat`, `make.bat`, or `make and run.bat`

## Credits

<b>Asbestoast</b> - Programming, Graphics