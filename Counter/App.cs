using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using Classic.Avalonia.Theme;
using Material.Styles.Themes;

namespace Counter;

internal class App : Application
{
    public override void Initialize()
    {
        RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Default;
        Styles.Add(new FluentTheme());
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
