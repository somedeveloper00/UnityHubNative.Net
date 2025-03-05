using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using FluentAvalonia.Styling;

namespace Counter;

public sealed class CounterApp : Application
{
    public static void Run(string[] args)
    {
        AppBuilder.Configure<CounterApp>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
    }

    public override void Initialize()
    {
        RequestedThemeVariant = ThemeVariant.Default;
        //Styles.Add(new FluentTheme() { DensityStyle = DensityStyle.Compact });
        //Styles.Add(new SimpleTheme());
        Styles.Add(new FluentAvaloniaTheme() { UseSystemFontOnWindows = true });

        //Styles.Add(new Style(x => x.Is<Button>())
        //{
        //    Setters =
        //    {
        //        new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(5)),
        //    }
        //});
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
