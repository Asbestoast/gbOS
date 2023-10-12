using System;

namespace SerialDeviceEmulator;
public sealed class ConnectionStatusChangedEventArgs : EventArgs
{
    public ConnectionStatus Status { get; }

    public ConnectionStatusChangedEventArgs(ConnectionStatus status)
    {
        Status = status;
    }
}
