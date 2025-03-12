using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;

namespace UnityHubNative.Net;

internal sealed class UnityProjectView : Panel
{
    public UnityProject unityProject;
    private MenuItem _moveUpMenuItem;
    private MenuItem _moveDownMenuItem;
    readonly ComboBox _unityVersionComboBox;
    readonly TextBlock _titleTextBlock;
    readonly TextBlock _pathTextBlock;

    public UnityProjectView(UnityProject unityProject) : this() => Update(unityProject);

    public UnityProjectView() : base()
    {
        unityProject = null;
        ClipToBounds = false;
        this.AddChildren
        ([
            new Border
            {
                Background = Brushes.Transparent,
                ContextFlyout = new MenuFlyout
                {
                }.AddItems
                ([
                    new MenuItem
                    {
                        Header = "Open"
                    }.OnClick(OpenProject),
                    new MenuItem
                    {
                        Header = "Open With",
                    }.OnClick(MainWindow.OnOpenWithClicked),
                    new MenuItem
                    {
                        Header = "Remove From List",
                    }.OnClick(MainWindow.OnRemoveProjectFromListClicked),
                    _moveUpMenuItem = new MenuItem
                    {
                        Header = "Move Up In List",
                    }.OnClick(OnMoveUpClicked),
                    _moveDownMenuItem = new MenuItem
                    {
                        Header = "Move Down In List",
                    }.OnClick(OnMoveDownClicked),
                ]),
                ClipToBounds = false,
                Child = new DockPanel
                {
                    Margin = new(0)
                }.AddChildren
                ([
                    _unityVersionComboBox = new ComboBox
                    {
                         ItemsSource = GetInstallationsEnum(),
                         Margin = new(5, 0, 0, 0),
                         MinWidth = 120,
                         VerticalAlignment = VerticalAlignment.Center,
                         WrapSelection = true
                    }.OnSelectionChanged(OnUnityVersionChanged).SetDock(Dock.Right),
                    new HyperlinkButton
                    {
                        Margin = new(0),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Content = _pathTextBlock = new TextBlock
                        {
                            FontWeight = FontWeight.Thin,
                            FontStyle = FontStyle.Italic,
                            FontSize = 11,
                            ClipToBounds = false,
                        }
                    }.OnClick(OnPathLinkClicked).SetDock(Dock.Right),
                    _titleTextBlock = new TextBlock
                    {
                        FontWeight = FontWeight.SemiBold,
                        FontSize = 15,
                        Margin = new(5, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                    }.SetDock(Dock.Right),
                ])
            }
        ]);
        ActualThemeVariantChanged += (_, _) => UpdateUnityVersionWarning();
    }

    private void OnMoveDownClicked() => MainWindow.MoveUnityProjectUp(unityProject);

    private void OnMoveUpClicked() => MainWindow.MoveUnityProjectDown(unityProject);

    private void OnPathLinkClicked()
    {
        OsUtils.OpenExplorer(unityProject.path);
    }

    public async void OpenProject()
    {
        if (unityProject is null)
            return;
        if (!unityProject.unity.HasValue)
        {
            _unityVersionComboBox.Focus();
            _unityVersionComboBox.IsDropDownOpen = true;
            _unityVersionComboBox.DropDownClosed += DropDownClosed;
            return;
        }
        unityProject.OpenProject();

        void DropDownClosed(object? sender, EventArgs e)
        {
            _unityVersionComboBox.DropDownClosed -= DropDownClosed;
            OpenProject();
        }
    }


    public override string ToString() => unityProject?.name ?? string.Empty;

    public void Update(UnityProject unityProject)
    {
        this.unityProject = unityProject;
        var index = UnityHubUtils.UnityProjects.IndexOf(unityProject);
        _moveDownMenuItem.IsEnabled = index >= 0 && index < UnityHubUtils.UnityProjects.Count - 1;
        _moveUpMenuItem.IsEnabled = index > 0 && index < UnityHubUtils.UnityProjects.Count;
        _titleTextBlock.Text = unityProject.name;
        _pathTextBlock.Text = unityProject.path;
        _unityVersionComboBox.SelectedIndex = unityProject.unity.HasValue
            ? UnityHubUtils.UnityInstallations.FindIndex(u => u.version == unityProject.unity.Value.version) + 1
            : 0;
        UpdateUnityVersionWarning();
    }

    static IEnumerable GetInstallationsEnum()
    {
        yield return "?";
        for (int i = 0; i < UnityHubUtils.UnityInstallations.Count; i++)
            yield return UnityHubUtils.UnityInstallations[i].version;
    }

    void OnUnityVersionChanged()
    {
        int ind = _unityVersionComboBox.SelectedIndex - 1;
        unityProject = new(unityProject.path, unityProject.lastModifiedDate, ind == -1 ? null : UnityHubUtils.UnityInstallations[ind]);
        UpdateUnityVersionWarning();
    }

    void UpdateUnityVersionWarning()
    {
        if (unityProject is not null && unityProject.unity.HasValue)
            _unityVersionComboBox.Foreground = ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
                ? Brushes.White
                : Brushes.Black;
        else
            _unityVersionComboBox.Foreground = Brushes.Red;
    }
}
