using Avalonia.Controls;

namespace UnityHubNative.Net
{
    sealed class UnityInstallationView : Panel
    {
        private Label _versionLabel;
        private Label _pathLabel;

        public UnityInstallationView()
        {
            _versionLabel = new()
            {
                FontSize = 15
            };
            _pathLabel = new()
            {
                FontSize = 12,
                FontStyle = Avalonia.Media.FontStyle.Italic
            };
            this.AddChildren
            ([
                new Border
                {
                    Focusable = true,
                    Margin = new(2),
                    Child = new StackPanel
                    {
                    }.AddChildren
                    ([
                        _versionLabel,
                        _pathLabel
                    ])
                }
            ]);
        }

        public void Update(UnityInstallation unityInstallation)
        {
            _versionLabel.Content = unityInstallation.version;
            _pathLabel.Content = unityInstallation.path;
        }
    }
}
