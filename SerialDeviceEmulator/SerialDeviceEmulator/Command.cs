namespace SerialDeviceEmulator;
public enum Command : byte
{
    /// <summary>
    /// Specify protocol version.
    /// </summary>
    Version = 1,
    /// <summary>
    /// Modify joypad state.
    /// </summary>
    Joypad = 101,
    /// <summary>
    /// Send byte as master.
    /// </summary>
    Sync1 = 104,
    /// <summary>
    /// Send byte as slave.
    /// </summary>
    Sync2 = 105,
    Sync3 = 106,
    Status = 108,
    WantDisconnect = 109,
}