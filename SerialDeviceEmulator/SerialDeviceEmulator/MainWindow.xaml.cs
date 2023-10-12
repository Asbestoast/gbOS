using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SerialDeviceEmulator.Annotations;

namespace SerialDeviceEmulator;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ConnectionStatus _connectionStatus;
    private bool _autoConnectEnabled;
    private BgbConnection BgbConnection { get; set; } = new();

    public bool AutoConnectEnabled
    {
        get => _autoConnectEnabled;
        set
        {
            if (value == _autoConnectEnabled) return;
            _autoConnectEnabled = value;
            BgbConnection.AutoConnectEnabled = value;
            OnPropertyChanged();
        }
    }

    public ConnectionStatus ConnectionStatus
    {
        get => _connectionStatus;
        set
        {
            if (value == _connectionStatus) return;
            _connectionStatus = value;
            OnPropertyChanged();
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        BgbConnection.ConnectionStatusChanged += BgbConnectionOnConnectionStatusChanged;
    }

    private void BgbConnectionOnConnectionStatusChanged(
        object? sender, ConnectionStatusChangedEventArgs e)
    {
        var status = e.Status;
        Dispatcher.BeginInvoke(() =>
        {
            ConnectionStatus = status;
        });
    }

    public void Connect()
    {
        BgbConnection.Connect();
    }

    public void Disconnect()
    {
        BgbConnection.Disconnect();
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Disconnect();
    }

    private void SendMouseState()
    {
        var targetSize = new System.Drawing.Size(160, 144);
        if (targetSize.Width <= 0 || targetSize.Height <= 0) return;

        var size = MouseTracker.RenderSize;
        if (size.Width <= 0 || size.Height <= 0) return;

        var mousePosition = Mouse.GetPosition(MouseTracker);

        var scaledMousePosition = new System.Drawing.Point(
            (int)Math.Clamp((mousePosition.X / size.Width) * targetSize.Width, 0, targetSize.Width - 1),
            (int)Math.Clamp((mousePosition.Y / size.Height) * targetSize.Height, 0, targetSize.Height - 1));

        var buttonStates = MouseButtonStates.None;
        buttonStates |= Mouse.LeftButton == MouseButtonState.Pressed ? MouseButtonStates.Left : MouseButtonStates.None;
        buttonStates |= Mouse.RightButton == MouseButtonState.Pressed ? MouseButtonStates.Right : MouseButtonStates.None;
        buttonStates |= Mouse.MiddleButton == MouseButtonState.Pressed ? MouseButtonStates.Middle : MouseButtonStates.None;

        BgbConnection.SetMouseState(
            (byte)scaledMousePosition.X,
            (byte)scaledMousePosition.Y,
            buttonStates);
    }

    private void MouseGrid_OnMouseMove(object sender, MouseEventArgs e)
    {
        SendMouseState();
    }

    private void MouseGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        SendMouseState();
        MouseGrid.CaptureMouse();
    }

    private void MouseGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        SendMouseState();
        MouseGrid.ReleaseMouseCapture();
    }

    private void ExitCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ConnectCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Connect();
    }

    private void DisconnectCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Disconnect();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}