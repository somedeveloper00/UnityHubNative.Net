using System.Diagnostics;

namespace UnityHubNative.Net;

static class OsUtils
{
    public static void OpenExplorer(string dir)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = dir,
            UseShellExecute = true
        });
    }
}
