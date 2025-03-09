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
        TransparencyLevelHint =
        [
            WindowTransparencyLevel.AcrylicBlur,
            WindowTransparencyLevel.Blur,
        ];
        Content = CreateContent();
        _platformOptionsComboBox!.SelectedIndex = 0;
        _unityVersionListBox!.Focus();
        CanResize = false;
        SizeToContent = SizeToContent.WidthAndHeight;
#if Windows
        Background = Brushes.Transparent;
#endif
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    private Control CreateContent() => new Border
    {
        Child = new DockPanel
        {
            Margin = new(5),
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
