using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using FluentAvalonia.Styling;

namespace UnityHubNative.Net;

public sealed class UnityHubNativeNetApp : Application
{
    public static void Run(string[] args)
    {
        AppBuilder.Configure<UnityHubNativeNetApp>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
    }

    public override void Initialize()
    {
        RequestedThemeVariant = ThemeVariant.Default;
        //Styles.Add(new FluentTheme() { DensityStyle = DensityStyle.Compact });
        //Styles.Add(new SimpleTheme());
        Styles.Add(new FluentAvaloniaTheme() { });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow(new MainViewModel());
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewApplication)
            singleViewApplication.MainView = new MainWindow(new MainViewModel());
        base.OnFrameworkInitializationCompleted();
    }
}
