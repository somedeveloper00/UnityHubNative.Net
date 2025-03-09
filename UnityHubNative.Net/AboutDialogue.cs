using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using UnityHubNative.Net;

namespace UnityHubNative.Net;

sealed class AboutDialogue : Window
{
    private Button _btn;

    public AboutDialogue()
    {
        Content = CreateContent();
        _btn!.Focus();
        TransparencyLevelHint =
        [
            WindowTransparencyLevel.AcrylicBlur,
            WindowTransparencyLevel.Blur,
        ];
        CanResize = false;
        SizeToContent = SizeToContent.WidthAndHeight;
#if Windows
        Background = Brushes.Transparent;
#endif
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Focusable = true;
    }

    private object? CreateContent()
    {
        return new DockPanel
        {
            Margin = new(5)
        }.AddChildren
        ([
            new TextBlock
            {
                Text = "Unity Hub Native .Net " + Manifest.Version,
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
                Text = "Developed with Avalonia and inspired by UnityHubNative",
                HorizontalAlignment = HorizontalAlignment.Center
            }.SetDock(Dock.Top),
            _btn = new Button
            {
                Content = "Close",
                HotKey = new(Key.Escape),
                HorizontalAlignment = HorizontalAlignment.Center
            }.OnClick(Close)
        ]);
    }
}
