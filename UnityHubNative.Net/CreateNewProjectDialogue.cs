using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace UnityHubNative.Net;

sealed class CreateNewProjectDialogue : Window
{
    TextBox _pathTextBox;
    TextBox _nameTextBox;
    ComboBox _versionSelector;
    ListBox _templatesParent;
    private Button _createButton;

    public CreateNewProjectDialogue()
    {
        Title = "Create a New Unity Project";
        Content = CreateContent();
        UpdateVersionSelectionViews();
        UpdateTemplateViews();
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

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (UnityHubUtils.UnityInstallations.Count == 0)
        {
            await MessageBoxManager.GetMessageBoxStandard("Can't create new projects", "No Unity Installations found").ShowAsPopupAsync(this);
            Close();
            return;
        }
        _nameTextBox.Focus();
    }

    object? CreateContent()
    {
        return new DockPanel
        {
            Margin = WindowDecorationMargin + new Thickness(5)
        }.AddChildren
        ([
            new DockPanel
            {
                Margin = new(2)
            }.AddChildren
            ([
                new TextBlock
                {
                    Text = "Project Name",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100,
                }.SetDock(Dock.Left),
                _nameTextBox = new TextBox
                {
                    MaxLines = 1,
                    VerticalAlignment = VerticalAlignment.Center,
                    Watermark = "New Unity Project"
                },
            ]).SetDock(Dock.Top),
            new DockPanel
            {
                Margin = new(2)
            }.AddChildren
            ([
                new TextBlock
                {
                    Text = "Location",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center,
                }.SetDock(Dock.Left),
                new Button
                {
                    Content = "Choose",
                }.OnClick(OnChooseLocationClicked).SetDock(Dock.Right),
                _pathTextBox = new TextBox
                {
                    MaxLines = 1,
                    VerticalAlignment = VerticalAlignment.Center,
                    Watermark = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                },
            ]).SetDock(Dock.Top),
            new Separator
            {
                Margin = new(5)
            }.SetDock(Dock.Top),
            new DockPanel
            {
                Margin = new(2),
                LastChildFill = false
            }.AddChildren
            ([
                new TextBlock
                {
                    Text = "Select Template For Version",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new(0, 0, 5, 0),
                }.SetDock(Dock.Left),
                _versionSelector = new ComboBox
                {
                    SelectedIndex = 0,
                }.OnSelectionChanged(OnVersionSelectionChanged).SetDock(Dock.Right),
            ]).SetDock(Dock.Top),
            new DockPanel
            {
                Margin = new(2),
                LastChildFill = false,
            }.AddChildren
            ([
                _createButton = new Button
                {
                    Content = "Create",
                    IsEnabled = false,
                    VerticalAlignment = VerticalAlignment.Center,
                }.OnClick(OnCreateClicked).SetTooltip("Create the Unity Project with the specified attributes").SetDock(Dock.Right),
                new Button
                {
                    Content = "Cancel",
                    HotKey = new(Key.Escape),
                    VerticalAlignment = VerticalAlignment.Center,
                }.OnClick(OnCancelClicked).SetTooltip("Cancel the creation of a new Unity Project").SetDock(Dock.Right),
            ]).SetDock(Dock.Bottom),
            _templatesParent = new ListBox
            {
                SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected,
                SelectedIndex = 0,
                IsTextSearchEnabled = true
            },
        ]);
    }

    void OnCreateClicked()
    {
        if (TryGetSelectedUnityInstallation(out var installation))
        {
            try
            {
                Task.Run(() => Process.Start(new ProcessStartInfo
                {
                    FileName = installation.path,
                    Arguments = $"-createProject \"{_pathTextBox}\" -cloneFromTemplate \"{UnityHubUtils.UnityInstallations[_templatesParent.SelectedIndex].path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }));
                Close();
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Cannot Create Project", $"Cannot create project at {_pathTextBox} because an error occurred: {ex.Message}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
            }
        }
    }

    void OnCancelClicked() => Close();

    void OnVersionSelectionChanged() => UpdateTemplateViews();

    void UpdateVersionSelectionViews()
    {
        _versionSelector.Items.Clear();
        for (int i = 0; i < UnityHubUtils.UnityInstallations.Count; i++)
            _versionSelector.Items.Add(UnityHubUtils.UnityInstallations[i].version);
        if (_versionSelector.ItemCount > 0)
            _versionSelector.SelectedIndex = 0;
    }

    void UpdateTemplateViews()
    {
        _templatesParent.Items.Clear();
        if (TryGetSelectedUnityInstallation(out var installation))
            foreach (var path in installation.GetTemplatePaths())
                _templatesParent.Items.Add(Path.GetFileName(path));
        UpdateCreateButtonView();
    }

    void UpdateCreateButtonView() => _createButton.IsEnabled = _templatesParent.Items.Count > 0 && Directory.Exists(_pathTextBox.Text) && _nameTextBox.Text?.Length > 0;

    bool TryGetSelectedUnityInstallation(out UnityInstallation installation)
    {
        if (_versionSelector.SelectedIndex < 0 || _versionSelector.SelectedIndex >= UnityHubUtils.UnityInstallations.Count)
        {
            installation = default;
            return false;
        }
        installation = UnityHubUtils.UnityInstallations[_versionSelector.SelectedIndex];
        return true;
    }

    async void OnChooseLocationClicked(Button button, RoutedEventArgs args)
    {
        var path = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Select where to write the Unity project"
        });
        if (path is null || path.Count == 0 || path[0] is null)
            return;
        var dir = path[0].TryGetLocalPath();
        if (Directory.Exists(dir))
            _pathTextBox.Text = dir;
    }
}
