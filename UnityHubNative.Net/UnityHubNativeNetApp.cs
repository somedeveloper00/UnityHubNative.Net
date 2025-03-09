using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using FluentAvalonia.Styling;

namespace UnityHubNative.Net;

public sealed class UnityHubNativeNetApp : Application
{
    public static UnityHubNativeNetApp Instance { get; private set; }
    public static AppConfig Config;

    public UnityHubNativeNetApp() : base() => Instance = this;

    public static void Run(string[] args)
    {
        Config = LoadConfig();
        AppBuilder.Configure<UnityHubNativeNetApp>()
            .UsePlatformDetect()
            .With(() => Config.transparent ? null : new Win32PlatformOptions
            {
                RenderingMode = [Win32RenderingMode.Software]
            })
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

    public static AppConfig LoadConfig()
    {
        try
        {
            var txt = File.ReadAllLines(Paths.ConfigPath);
            return new AppConfig()
            {
                transparent = txt.Length >= 1 && txt[0] == "true",
                acrylic = txt.Length >= 2 && txt[1] == "true",
                blurIntensity = txt.Length >= 3 && float.TryParse(txt[2], out var acrylicAmount) ? acrylicAmount : 0.2f
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return default;
        }
    }

    public static void SaveConfig(AppConfig config)
    {
        File.WriteAllLines(Paths.ConfigPath,
        [
            config.transparent ? "true" : "false",
            config.acrylic ? "true" : "false",
            config.blurIntensity.ToString(),
        ]);
    }

    public struct AppConfig
    {
        public bool transparent;
        public bool acrylic;
        public float blurIntensity;
    }
}
