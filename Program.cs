// © 2024 led-mirage. All rights reserved.

namespace PingMonitor;

static class Program
{
    [STAThread]
    static void Main()
    {
        Properties.Settings.Default.Reload();
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }    
}