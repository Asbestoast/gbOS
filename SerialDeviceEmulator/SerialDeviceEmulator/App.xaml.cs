using System.Windows;

namespace SerialDeviceEmulator
{
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow();

            foreach (var arg in e.Args)
            {
                const string commandPrefix = "-";
                if (arg.StartsWith(commandPrefix))
                {
                    var commandName = arg[commandPrefix.Length..];
                    if (commandName == "autoConnect")
                    {
                        window.AutoConnectEnabled = true;
                        window.Connect();
                    }
                    else if (commandName == "connect")
                    {
                        window.Connect();
                    }
                }
            }

            window.Show();
        }
    }
}
