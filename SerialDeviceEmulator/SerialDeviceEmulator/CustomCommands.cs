using System.Windows.Input;

namespace SerialDeviceEmulator;
public static class CustomCommands
{
    public static readonly RoutedUICommand Exit = new(
        "E_xit",
        "Exit",
        typeof(CustomCommands),
        new InputGestureCollection
        {
            new KeyGesture(Key.F4, ModifierKeys.Alt),
        });

    public static readonly RoutedUICommand Connect = new(
        "_Connect",
        "Connect",
        typeof(CustomCommands),
        new InputGestureCollection
        {
            new KeyGesture(Key.R, ModifierKeys.Control),
        });

    public static readonly RoutedUICommand Disconnect = new(
        "_Disconnect",
        "Disconnect",
        typeof(CustomCommands),
        new InputGestureCollection
        {
            new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift),
        });
}
