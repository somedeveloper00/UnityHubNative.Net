using Avalonia.Controls;

namespace UnityHubNative.Net;

/// <summary>
/// view for Unity installation search path
/// </summary>
sealed class UnityInstallationSearchPathView : Panel
{
    private readonly Label _pathLabel;

    public UnityInstallationSearchPathView() : base()
    {
        Margin = new(0);
        this.AddChildren
        ([
            _pathLabel = new Label
            {
            }
        ]);
    }
    public void Update(string path) => _pathLabel.Content = path;
}
