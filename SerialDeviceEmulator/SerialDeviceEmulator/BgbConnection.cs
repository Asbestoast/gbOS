using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SerialDeviceEmulator;
public class BgbConnection
{
    private const bool TimeoutEnabled = false;

    public bool AutoConnectEnabled
    {
        get => _autoConnectEnabled;
        set => _autoConnectEnabled = value;
    }

    private MouseState _nextMouseState;
    private Thread? NetworkThread { get; set; }
    private CancellationTokenSource? Cts { get; set; }

    public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;

    public ConnectionStatus Status
    {
        get => _status;
        set
        {
            if (value == _status) return;
            _status = value;
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(_status));
        }
    }

    private enum MessageType : byte
    {
        None = 0,
        MouseX = 1,
        MouseY = 2,
        MouseButtons = 3,
    }

    private readonly object MouseStateLock = new();
    private volatile ConnectionStatus _status;
    private volatile bool _autoConnectEnabled;

    private MouseState NextMouseState
    {
        get
        {
            lock (MouseStateLock)
            {
                return _nextMouseState;
            }
        }
        set
        {
            lock (MouseStateLock)
            {
                _nextMouseState = value;
            }
        }
    }

    private struct MouseState
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public MouseButtonStates ButtonState { get; set; }
    }

    public void SetMouseState(byte x, byte y, MouseButtonStates buttonState)
    {
        NextMouseState = new MouseState
        {
            X = x,
            Y = y,
            ButtonState = buttonState,
        };
    }

    public void Connect()
    {
        Disconnect();
        Cts = new CancellationTokenSource();
        NetworkThread = new Thread(ProcessThread);
        NetworkThread.Start();
    }

    private void ProcessThread()
    {
        try
        {
            if (Cts == null) throw new NullReferenceException("Cts not initialized.");
            while (!Cts.IsCancellationRequested)
            {
                var mouseState = new MouseState();

                var receiveBuffer = new byte[Packet.SizeOf];
                var bytesBuffered = 0;

                Status = ConnectionStatus.Connecting;

                try
                {
                    using var client = new TcpClient
                    {
                        NoDelay = true,
                    };

                    var connectResult = client.BeginConnect("127.0.0.1", 8765, _ => {}, null);
                    connectResult.AsyncWaitHandle.WaitOne();
                    client.EndConnect(connectResult);

                    Status = ConnectionStatus.Linked;

                    using var stream = client.GetStream();

                    if (TimeoutEnabled)
                    {
                        stream.ReadTimeout = 500;
                        stream.WriteTimeout = 500;
                    }

                    stream.WriteAsync(Packet.CreateHandshakePacket(), Cts.Token);

                    while (!Cts.IsCancellationRequested)
                    {
                        try
                        {
                            var task = stream.ReadAsync(receiveBuffer, bytesBuffered, receiveBuffer.Length - bytesBuffered, Cts.Token);
                            task.Wait();
                            var readBytes = task.Result;
                            bytesBuffered += readBytes;
                            if (bytesBuffered < Packet.SizeOf) continue;

                            using var packetStream = new MemoryStream(receiveBuffer);
                            var packet = new Packet(packetStream);
                            bytesBuffered = 0;

                            if (packet.Command == Command.Version)
                            {
                                stream.WriteAsync(Packet.CreateStatusPacket(StatusFlags.Running), Cts.Token);
                            }
                            else if (packet.Command == Command.Status)
                            {
                            }
                            else if (packet.Command == Command.Sync1)
                            {
                                var messageType = (MessageType)packet.B2;
                                byte response;
                                if (messageType == MessageType.MouseX)
                                {
                                    mouseState = NextMouseState;
                                    response = mouseState.X;
                                }
                                else if (messageType == MessageType.MouseY)
                                    response = mouseState.Y;
                                else if (messageType == MessageType.MouseButtons)
                                    response = (byte)mouseState.ButtonState;
                                else
                                    response = 0;

                                stream.WriteAsync(Packet.CreateSync2Packet(response), Cts.Token);
                            }
                            else if (packet.Command == Command.Sync2)
                            {
                                Debug.WriteLine($"Recieved packet {packet}");
                            }
                            else if (packet.Command == Command.Sync3)
                            {
                                if (packet.B2 == 0)
                                {
                                    stream.WriteAsync(new Packet
                                    {
                                        Command = Command.Sync3,
                                        I1 = packet.I1,
                                    }, Cts.Token);
                                }
                                else
                                {
                                    Debug.WriteLine($"Recieved Sync3 packet (B2=1) {packet}");
                                }
                            }
                            else if (packet.Command == Command.WantDisconnect)
                            {
                            }
                            else if (packet.Command == Command.Joypad)
                            {
                            }
                            else
                            {
                                Debug.WriteLine($"Recieved unknown packet {packet}");
                            }
                        }
                        catch (IOException)
                        {
                            if (!TimeoutEnabled) throw;
                        }
                    }
                }
                catch (ThreadInterruptedException)
                {
                    throw;
                }
                catch (Exception)
                {
                    if (AutoConnectEnabled)
                    {
                        Status = ConnectionStatus.Error;
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        catch (ThreadInterruptedException)
        {
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Status = ConnectionStatus.Error;
            return;
        }

        Status = ConnectionStatus.NotLinked;
    }

    public void Disconnect()
    {
        if (NetworkThread == null) return;
        Cts?.Cancel();
        NetworkThread.Interrupt();
        NetworkThread.Join();
        NetworkThread = null;
        if (Cts != null)
        {
            Cts.Dispose();
            Cts = null;
        }
        Status = ConnectionStatus.NotLinked;
    }
}