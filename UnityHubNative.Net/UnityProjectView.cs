using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace UnityHubNative.Net;

internal sealed class UnityProjectView : Panel
{
    public UnityProject unityProject;

    private readonly Label _titleLabel;
    private readonly Label _pathLabel;
    private readonly Button _openBtn;

    public UnityProjectView(UnityProject unityProject) : this() => Update(unityProject);

    public override string ToString() => unityProject?.name ?? string.Empty;

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
                    .OnClick(() => unityProject?.OpenProject()),
                    _pathLabel = new Label
                    {
                        FontWeight = FontWeight.Thin,
                        FontStyle = FontStyle.Italic,
                        FontSize = 12,
                        Margin = new(5, 0, 0, 0),
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    }.SetDock(Dock.Right),
                    _titleLabel = new Label
                    {
                        FontWeight = FontWeight.SemiBold,
                        FontSize = 15,
                        Margin = new(5, 0, 0, 0),
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    }.SetDock(Dock.Right),
                ])
            }
        ]);
    }

    public void Update(UnityProject unityProject)
    {
        this.unityProject = unityProject;
        _titleLabel.Content = unityProject.name;
        _pathLabel.Content = unityProject.path;
        _openBtn.IsEnabled = unityProject.unity.HasValue;
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
}
