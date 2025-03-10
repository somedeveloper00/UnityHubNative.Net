using System.Collections;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace UnityHubNative.Net;

internal sealed class UnityProjectView : Panel
{
    public UnityProject unityProject;
    readonly ComboBox _unityVersionComboBox;
    readonly TextBlock _titleTextBlock;
    readonly TextBlock _pathTextBlock;
    readonly Button _openBtn;

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
                    _openBtn = new Button
                    {
                        Margin = new(5, 5, 20, 5),
                        Content = "Open"
                    }.SetTooltip("Open the project").SetDock(Dock.Right)
                    .OnClick(OpenProject),
                    _unityVersionComboBox = new ComboBox
                    {
                         ItemsSource = GetInstallationsEnum(),
                         Margin = new(5, 0),
                         MinWidth = 120,
                         VerticalAlignment = VerticalAlignment.Center,
                         WrapSelection = true
                    }.OnSelectionChanged(OnUnityVersionChanged).SetDock(Dock.Right),
                    _pathTextBlock = new TextBlock
                    {
                        FontWeight = FontWeight.Thin,
                        FontStyle = FontStyle.Italic,
                        FontSize = 12,
                        Margin = new(5, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                    }.SetDock(Dock.Right),
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
        _openBtn.IsEnabled = unityProject.unity.HasValue;
        UpdateUnityVersionWarning();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        Debug.WriteLine("got focus");
        _openBtn.RaiseEvent(new()
        {
            Handled = e.Handled,
            Route = e.Route,
            RoutedEvent = e.RoutedEvent,
            Source = e.Source
        });
        e.Handled = true;
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
        if (ind == -1)
            unityProject = new(unityProject.path, unityProject.lastModifiedDate, null);
        else
            unityProject = new(unityProject.path, unityProject.lastModifiedDate, UnityHubUtils.UnityInstallations[ind]);
        UpdateUnityVersionWarning();
    }

    void UpdateUnityVersionWarning()
    {
        if (unityProject is not null && unityProject.unity.HasValue)
        {
            _openBtn.IsEnabled = true;
            _unityVersionComboBox.Foreground = ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark ? Brushes.White : Brushes.Black;
        }
        else
        {
            _openBtn.IsEnabled = false;
            _unityVersionComboBox.Foreground = Brushes.Red;
        }
    }
}
