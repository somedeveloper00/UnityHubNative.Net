using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;

namespace UnityHubNative.Net;

sealed class CreateNewProjectDialogue : Window
{
    TextBox _pathTextBox;
    TextBox _nameTextBox;
    ComboBox _versionSelector;
    ListBox _templatesParent;
    Button _createButton;

    public CreateNewProjectDialogue()
    {
        Title = UnityHubNativeNetApp.Config.language.CreateANewUnityProject;
        Content = CreateContent();
        UpdateVersionSelectionViews();
        UpdateTemplateViews();
        UpdateCreateButtonView();

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
            await MessageBoxManager.GetMessageBoxStandard(UnityHubNativeNetApp.Config.language.CantCreateNewProjects, UnityHubNativeNetApp.Config.language.NoUnityInstallationsFound).ShowAsPopupAsync(this);
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
                    Text = UnityHubNativeNetApp.Config.language.ProjectName,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100,
                }.SetDock(Dock.Left),
                _nameTextBox = new TextBox
                {
                    MaxLines = 1,
                    VerticalAlignment = VerticalAlignment.Center,
                    Watermark = UnityHubNativeNetApp.Config.language.NewUnityProject
                }.OnTextChanged(UpdateCreateButtonView),
            ]).SetDock(Dock.Top),
            new DockPanel
            {
                Margin = new(2)
            }.AddChildren
            ([
                new TextBlock
                {
                    Text = UnityHubNativeNetApp.Config.language.Location,
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center,
                }.SetDock(Dock.Left),
                new Button
                {
                    Content = UnityHubNativeNetApp.Config.language.Choose,
                    Margin = new(2)
                }.OnClick(OnChooseLocationClicked).SetDock(Dock.Right),
                _pathTextBox = new TextBox
                {
                    MaxLines = 1,
                    VerticalAlignment = VerticalAlignment.Center,
                    Watermark = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                }.OnTextChanged(UpdateCreateButtonView),
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
                    Text = UnityHubNativeNetApp.Config.language.SelectTemplateForVersion,
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
                    Content = UnityHubNativeNetApp.Config.language.Create,
                    IsEnabled = false,
                    Margin = new(2),
                    VerticalAlignment = VerticalAlignment.Center,
                }.OnClick(OnCreateClicked).SetTooltip(UnityHubNativeNetApp.Config.language.CreateTheUnityProjectWithTheSpecifiedAttributes).SetDock(Dock.Right),
                new Button
                {
                    Content = UnityHubNativeNetApp.Config.language.Cancel,
                    HotKey = new(Key.Escape),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new(2),
                }.OnClick(OnCancelClicked).SetTooltip(UnityHubNativeNetApp.Config.language.CancelTheCreationOfANewUnityProject).SetDock(Dock.Right),
            ]).SetDock(Dock.Bottom),
            _templatesParent = new ListBox
            {
                SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected,
                SelectedIndex = 0,
                IsTextSearchEnabled = true
            },
        ]);
    }

    async void OnCreateClicked()
    {
        if (TryGetSelectedUnityInstallation(out var installation))
        {
            try
            {
                var unityInstallation = UnityHubUtils.UnityInstallations[_versionSelector.SelectedIndex];
                string templatePath = unityInstallation.GetTemplatePaths()[_templatesParent.SelectedIndex];
                string projectPath = Path.Combine(_pathTextBox.Text!, _nameTextBox.Text!);
                string args = $"-createProject \"{projectPath}\" -cloneFromTemplate \"{templatePath}\"";
                var startInfo = new ProcessStartInfo
                {
                    FileName = installation.path,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                _ = Task.Run(() => Process.Start(startInfo));

                UnityHubUtils.UnityProjects.Add(new(projectPath, DateTime.UtcNow, unityInstallation));
                UnityHubUtils.SaveUnityProjects();
                MainWindow.UpdateUnityProjectViews();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            Close();
        }
    }

    void OnCancelClicked() => Close();

    void OnVersionSelectionChanged()
    {
        UpdateTemplateViews();
        UpdateCreateButtonView();
    }

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
            Title = UnityHubNativeNetApp.Config.language.SelectWhereToWriteTheUnityProject
        });
        if (path is null || path.Count == 0 || path[0] is null)
            return;
        var dir = path[0].TryGetLocalPath();
        if (Directory.Exists(dir))
            _pathTextBox.Text = dir;
    }
}
