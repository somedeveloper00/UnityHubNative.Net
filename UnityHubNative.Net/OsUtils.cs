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

    public static void OpenInTerminal(string command)
    {
#if Windows
        var (filename, args) = ExtractCommand(UnityHubNativeNetApp.Config.openInTerminalFormat.Replace("{path}", command, StringComparison.InvariantCultureIgnoreCase));
        Process.Start(new ProcessStartInfo
        {
            FileName = filename,
            Arguments = args,
            UseShellExecute = true
        });
#endif
    }

    static (string fileName, string args) ExtractCommand(string command)
    {
        bool insideQuote = false;
        for (int i = 0; i < command.Length; i++)
        {
            if (command[i] == '\"' && command.Length > 0 && command[i - 1] != '\\')
                insideQuote = !insideQuote;
            else if (command[i] == ' ')
                return (command[..i], command.Length > i ? command[(i + 1)..] : string.Empty);
        }
        return (command, string.Empty);
    }
}
