using System.Diagnostics;

namespace UnityHubNative.Net;

static class OsUtils
{
    public static void OpenExplorer(string dir)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = dir,
            UseShellExecute = true
        });
    }

    public static void OpenInTerminal(string path)
    {
#if Windows
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/K cd /d \"{path}\""
        });
#endif
    }
}
