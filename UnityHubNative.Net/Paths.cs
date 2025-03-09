using System.Diagnostics;

namespace UnityHubNative.Net;

static class Paths
{
    public static readonly string Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnityHubNative");
    public static readonly string SearchPathsPath = Path.Combine(Dir, "editorPaths.txt");
    public static readonly string ProjectPathsPath = Path.Combine(Dir, "projects.txt");
    public static readonly string ConfigPath = Path.Combine(Dir, "config.txt");

    static Paths()
    {
        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
            Debug.WriteLine("created local data directory \"{0}\"", Dir);
        }
    }
}
