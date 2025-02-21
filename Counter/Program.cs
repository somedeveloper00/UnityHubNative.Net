using System.Diagnostics;
using Avalonia;

namespace Counter;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }
}
