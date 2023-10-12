using System;

namespace SerialDeviceEmulator;
[Flags]
public enum Button : byte
{
    Right = 0 << 0,
    Left = 1 << 0,
    Up = 2 << 0,
    Down = 3 << 0,
    A = 4 << 0,
    B = 5 << 0,
    Select = 6 << 0,
    Start = 7 << 0,

    Pressed = 1 << 3,
    Released = 0 << 3,
}