using System;

namespace SerialDeviceEmulator;
[Flags]
public enum MouseButtonStates : byte
{
    None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Middle = 1 << 2,
}
