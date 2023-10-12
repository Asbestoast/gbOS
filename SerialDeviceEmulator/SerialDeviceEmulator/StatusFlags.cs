using System;

namespace SerialDeviceEmulator;
[Flags]
public enum StatusFlags
{
    None = 0,
    Running = 1 << 0,
    Paused = 1 << 1,
    SupportReconnect = 1 << 2,
}
