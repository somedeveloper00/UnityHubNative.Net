using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace UnityHubNative.Net;

sealed class AboutDialogue : Window
{
    private Button _btn;

    public AboutDialogue()
    {
        Content = CreateContent();

        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        SystemDecorations = SystemDecorations.BorderOnly;
        ExtendClientAreaToDecorationsHint = true;

        if (UnityHubNativeNetApp.Config.transparent)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Blur];
#if Windows
            Background = new SolidColorBrush(Colors.Transparent, UnityHubNativeNetApp.Config.blurIntensity);
#endif
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _btn.Focus();
    }

    private object? CreateContent()
    {
        return new DockPanel
        {
            Margin = WindowDecorationMargin + new Thickness(5)
        }.AddChildren
        ([
            new TextBlock
            {
                Text = UnityHubNativeNetApp.Config.language.TitleBar + " " + Manifest.Version,
                FontSize = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
            }.SetDock(Dock.Top),
            new TextBlock
            {
                Text = "© Saeed Barari 2025\n© Ravbug 2025",
                HorizontalAlignment = HorizontalAlignment.Center
            }.SetDock(Dock.Top),
            new TextBlock
            {
                Text = UnityHubNativeNetApp.Config.language.About_1,
                HorizontalAlignment = HorizontalAlignment.Center
            }.SetDock(Dock.Top),
            _btn = new Button
            {
                Content = UnityHubNativeNetApp.Config.language.Close,
                HotKey = new(Key.Escape),
                HorizontalAlignment = HorizontalAlignment.Center
            }.OnClick(Close)
        ]);
    }
}
