using System;

namespace SerialDeviceEmulator;
[Flags]
public enum SerialControlFlag : byte
{
    None = 0,
    ExternalClock = 1 << 0,
    FastMode = 1 << 1,
    Start = 1 << 7,
}
