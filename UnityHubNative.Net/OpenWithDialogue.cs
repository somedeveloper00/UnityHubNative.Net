using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace UnityHubNative.Net;

sealed class OpenWithDialogue : Window
{
    private readonly UnityProject _unityProject;
    private ListBox _unityVersionListBox;
    private ComboBox _platformOptionsComboBox;
    static readonly string[] s_platformOptions = [
        "Current Platform",
        "Windows",
        "MacOs",
        "Linux",
        "iOS",
        "Android",
        "WebGL",
        "UWP"
    ];

    public OpenWithDialogue(UnityProject unityProject)
    {
        _unityProject = unityProject;
        Title = "Open With Specific Editor";
        Content = CreateContent();

        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        SystemDecorations = SystemDecorations.BorderOnly;
        ExtendClientAreaToDecorationsHint = true;

        if (UnityHubNativeNetApp.Config.transparent)
        {
            TransparencyLevelHint =
            [
                WindowTransparencyLevel.AcrylicBlur,
                WindowTransparencyLevel.Blur,
            ];
#if Windows
            Background = new SolidColorBrush(Colors.Transparent, UnityHubNativeNetApp.Config.blurIntensity);
#endif
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _platformOptionsComboBox.SelectedIndex = 0;
        _unityVersionListBox.SelectedIndex = 0;
        _unityVersionListBox.ContainerFromIndex(0)!.Focus();
    }

    private Control CreateContent() => new Border
    {
        Child = new DockPanel
        {
            Margin = WindowDecorationMargin + new Thickness(5),
            Background = Brushes.Transparent,
        }.AddChildren
        ([
            new TextBlock
            {
                Text = $"Select editor verison to open \"{_unityProject.name}\""
            }.SetDock(Dock.Top),
            _unityVersionListBox = new ListBox
            {
                SelectionMode = SelectionMode.AlwaysSelected | SelectionMode.Single,
            }.AddItems([.. UnityHubUtils.UnityInstallations.Select(u =>
            {
                return new DockPanel
                {
                }.AddChildren
                ([
                    new TextBlock
                    {
                        Margin = new(4),
                        Text = u.version,
                        FontWeight = FontWeight.SemiBold,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    }.SetDock(Dock.Left),
                    new TextBlock
                    {
                        Text = u.path,
                        FontWeight = FontWeight.Thin,
                        FontStyle = FontStyle.Italic,
                        FontSize = 12,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    }.SetDock(Dock.Right)
                ]);
            })]).SetDock(Dock.Top),
            _platformOptionsComboBox = new ComboBox
            {
                WrapSelection = true,
                AutoScrollToSelectedItem = true,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                IsTextSearchEnabled = true,
            }.AddItems(GetPlatformItems()).SetDock(Dock.Top),
            new Button
            {
                Content = "_Open",
                HotKey = new(Key.Enter),
            }.OnClick(OnOpenClicked).SetDock(Dock.Right),
            new Button
            {
                Content = "_Cancel",
                HotKey = new(Key.Escape),
            }.OnClick(OnCancelClicked).SetDock(Dock.Right),
        ])
    };

    private void OnCancelClicked() => Close();

    private void OnOpenClicked()
    {
        var proj = new UnityProject(_unityProject.path, _unityProject.lastModifiedDate, UnityHubUtils.UnityInstallations[_unityVersionListBox.SelectedIndex]);
        proj.OpenProject((UnityPlatform)_platformOptionsComboBox.SelectedIndex);
    }

    private static object[] GetPlatformItems() => s_platformOptions;
}
