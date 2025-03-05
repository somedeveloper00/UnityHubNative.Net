using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MsBox.Avalonia;

namespace Counter;

internal sealed class UnityProjectView : Panel
{
    private readonly Label _titleLabel;
    private readonly Label _pathLabel;
    private readonly Button _openBtn;
    private UnityProject? _unityProject;

    public UnityProjectView(UnityProject unityProject) : this() => Update(unityProject);

    public override string ToString() => _unityProject?.name ?? string.Empty;

    public UnityProjectView() : base()
    {
        _unityProject = null;
        this.AddChildren
        ([
            new Border
            {
                CornerRadius = new(5),
                BoxShadow = new(new(){ Blur = 2, Color = new(200, 0, 0, 0), Spread = 0.5, OffsetX = 2, OffsetY = 2}),
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
                        FontWeight = FontWeight.DemiBold,
                        FontSize = 15,
                        Margin = new(5, 0, 0, 0),
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    }.SetDock(Dock.Right),
                ])
            }
        ]);
    }

    public void OpenProject()
    {
        Debug.WriteLine("opening " + _unityProject?.path);
        if (!Directory.Exists(_unityProject?.path))
        {
            MessageBoxManager.GetMessageBoxStandard("Cannot Open Project", $"Cannot open project at {_unityProject.path} because it could not be found.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
            return;
        }
        if (!File.Exists(_unityProject.unity?.path))
        {
            MessageBoxManager.GetMessageBoxStandard("Cannot Open Project", $"Cannot open project at {_unityProject.path} because Unity could not be found at {_unityProject.unity?.path}.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
            return;
        }
        try
        {
            Task.Run(() => Process.Start(new ProcessStartInfo
            {
                FileName = _unityProject.unity?.path,
                Arguments = $"-projectPath \"{_unityProject.path}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }));
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("Cannot Open Project", $"Cannot open project at {_unityProject.path} because an error occurred: {ex.Message}", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }

    public void Update(UnityProject unityProject)
    {
        _unityProject = unityProject;
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
