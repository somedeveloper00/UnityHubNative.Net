using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace UnityHubNative.Net;

internal sealed class UnityProjectView : Panel
{
    public UnityProject unityProject;
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

    private void OnPathLinkClicked()
    {
        OsUtils.OpenExplorer(unityProject.path);
    }

    public void OpenProject()
    {
        if (unityProject is null)
            return;
        if (!unityProject.unity.HasValue)
        {
            _unityVersionComboBox.Focus();
            _unityVersionComboBox.IsDropDownOpen = true;
            return;
        }
        unityProject.OpenProject();
    }

    public override string ToString() => unityProject?.name ?? string.Empty;

    public void Update(UnityProject unityProject)
    {
        this.unityProject = unityProject;
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
