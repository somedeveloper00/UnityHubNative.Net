using System.Data;
using MsBox.Avalonia;

namespace Counter;

static class UnityHubUtils
{
    public static List<UnityInstallation> UnityInstallations = [];
    public static List<UnityProject> UnityProjects = [];
    public static List<string> UnityInstallationSearchPaths = [];

    static readonly string s_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UnityHubNative");
    static readonly string s_searchPathsPath = Path.Combine(s_dir, "editorPaths.txt");
    static readonly string s_projectPathsPath = Path.Combine(s_dir, "projects.txt");

    static readonly string[] s_unityInstallationSearchPathsDefault = [
#if OS_WINDOWS
        "C:\\Program Files",
        "C:\\Program Files\\Unity Hub",
        "C:\\Program Files\\Unity\\Hub\\Editor",
#elif OS_LINUX
#elif OS_MAC
        "/Applications/Unity Hub.app"
#endif
    ];

    static readonly string s_executableRelativePath =
#if OS_WINDOWS
        "Editor\\Unity.exe"
#elif OS_LINUX
        "Editor/Unity"
#elif OS_MAC
        "Contents/MacOS/Unity"
#endif
    ;

    static readonly string[] s_projectVersionPath = ["ProjectSettings", "ProjectVersion.txt"];

    /// <summary>
    /// Calls all the load methods in order. 
    /// <list type="number">
    /// <item><see cref="LoadUnityInstallationSearchPaths"/></item> 
    /// <item><see cref="LoadUnityInstallations"/></item>
    /// <item><see cref="LoadUnityProjects"/></item>
    /// </list>
    /// </summary>
    public static void LoadAll()
    {
        LoadUnityInstallationSearchPaths();
        LoadUnityInstallations();
        LoadUnityProjects();
    }

    /// <summary>
    /// Loads <see cref="UnityInstallationSearchPaths"/>
    /// </summary>
    public static void LoadUnityInstallationSearchPaths()
    {
        EnsureSaveDirectoryExists();
        UnityInstallationSearchPaths.Clear();
        bool exists = File.Exists(s_searchPathsPath);
        if (exists)
        {
            Console.WriteLine("found Unity installation search paths data at \"{0}\"", s_searchPathsPath);
            UnityInstallationSearchPaths.AddRange(File.ReadAllLines(s_searchPathsPath));
        }
        else
        {
            Console.WriteLine("did not find Unity installation search paths data. using defaults");
            UnityInstallationSearchPaths.AddRange(s_unityInstallationSearchPathsDefault);
            SaveUnityInstallationSearchPaths();
        }
        Console.WriteLine("found Unity installation search paths:");
        for (int i = 0; i < UnityInstallationSearchPaths.Count; i++)
            Console.WriteLine("-> {0}", UnityInstallationSearchPaths[i]);
    }

    /// <summary>
    /// Saves <see cref="UnityInstallationSearchPaths"/>
    /// </summary>
    public static async void SaveUnityInstallationSearchPaths()
    {
        EnsureSaveDirectoryExists();
        try
        {
            File.WriteAllLines(s_searchPathsPath, UnityInstallationSearchPaths);
            Console.WriteLine("saved Unity installation paths to \"{0}\"", s_searchPathsPath);
        }
        catch (Exception ex)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard("Error while saving Unity Installations", ex.Message + " | Retry?", MsBox.Avalonia.Enums.ButtonEnum.OkCancel, MsBox.Avalonia.Enums.Icon.Error, Avalonia.Controls.WindowStartupLocation.CenterScreen);
            var result = await msg.ShowAsync();
            if (result == MsBox.Avalonia.Enums.ButtonResult.Ok)
                SaveUnityInstallationSearchPaths();
        }
    }

    /// <summary>
    /// Loads <see cref="UnityInstallations"/>. Requires 
    /// <see cref="UnityInstallationSearchPaths"/> to be loaded first.
    /// </summary>
    public static void LoadUnityInstallations()
    {
        UnityInstallations.Clear();
        foreach (var path in UnityInstallationSearchPaths)
        {
            if (!Directory.Exists(path))
                continue;
            foreach (var dir in Directory.GetDirectories(path))
            {
                var executable = Path.Combine(dir, s_executableRelativePath);
                if (File.Exists(executable))
                {
                    var version = ExtractVersionFromDirName(Path.GetFileName(dir));
                    UnityInstallations.Add(new UnityInstallation(executable, version));
                }
            }
        }
        Console.WriteLine("found Unity installations:");
        for (int i = 0; i < UnityInstallations.Count; i++)
            Console.WriteLine("-> {0}", UnityInstallations[i]);
    }

    /// <summary>
    /// Loads <see cref="UnityProjects"/>. Requires 
    /// <see cref="UnityInstallationSearchPaths"/> and 
    /// <see cref="UnityInstallations"/> to be loaded first.
    /// </summary>
    public static void LoadUnityProjects()
    {
        EnsureSaveDirectoryExists();
        UnityProjects.Clear();
        if (File.Exists(s_projectPathsPath))
        {
            var lines = File.ReadAllLines(s_projectPathsPath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (File.Exists(lines[i]))
                {
                    var ind = FindUnityVersionIndex(lines[i]);
                    UnityProjects.Add(new(lines[i], ind == -1 ? null : UnityInstallations[i]));
                }
            }
        }
        Console.WriteLine("found Unity projects:");
        for (int i = 0; i < UnityProjects.Count; i++)
            Console.WriteLine("-> {0}", UnityProjects[i]);
    }

    /// <summary>
    /// Saves <see cref="UnityProjects"/>
    /// </summary>
    public static async void SaveUnityProjects()
    {
        try
        {
            EnsureSaveDirectoryExists();
            File.WriteAllLines(s_projectPathsPath, UnityProjects.Select(p => p.path));
        }
        catch (Exception ex)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard("Error while saving Unity Projects", ex.Message + " | Retry?", MsBox.Avalonia.Enums.ButtonEnum.OkCancel, MsBox.Avalonia.Enums.Icon.Error, Avalonia.Controls.WindowStartupLocation.CenterScreen);
            var result = await msg.ShowAsync();
            if (result == MsBox.Avalonia.Enums.ButtonResult.Ok)
                SaveUnityProjects();
        }
    }

    /// <summary>
    /// Finds the index from <see cref="UnityInstallations"/> which is the unity installation 
    /// version for the unity project at <paramref name="path"/>. Returns -1 if not found.
    /// </summary>
    public static int FindUnityVersionIndex(string path)
    {
        EnsureSaveDirectoryExists();
        var projectVersionFile = Path.Combine([path, .. s_projectVersionPath]);
        if (File.Exists(projectVersionFile))
        {
            var version = File.ReadAllText(projectVersionFile)[..17];
            for (int i = 0; i < UnityInstallations.Count; i++)
                if (UnityInstallations[i].version == version)
                    return i;
        }
        return -1;
    }

    private static void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(s_dir))
        {
            Directory.CreateDirectory(s_dir);
            Console.WriteLine("created local data directory \"{0}\"", s_dir);
        }
    }

    private static string ExtractVersionFromDirName(string v)
    {
        Span<char> src = stackalloc char[v.Length];
        v.CopyTo(src);
        Span<char> r = stackalloc char[64];
        int ri = 0;
        for (int i = 0; i < src.Length; i++)
        {
            if (src[i] == ' ')
                if (ri > 0)
                    break;
                else
                    continue;
            if ((src[i] >= '0' && src[i] <= '9') || ri > 0)
                r[ri++] = src[i];
        }
        return new(r);
    }
}

readonly struct UnityInstallation(string path, string version) : IComparable<UnityInstallation>
{
    public readonly string path = path;
    public readonly string version = version;

    public override string ToString() => $"{{\"{path}\", \"{version}\"}}";
    public int CompareTo(UnityInstallation other) => path.CompareTo(other.path);
    public override int GetHashCode() => path.GetHashCode();
}

readonly struct UnityProject(string path, UnityInstallation? unity)
{
    public readonly string path = path;
    public readonly string name = Path.GetFileName(path);
    public readonly UnityInstallation? unity = unity;

    public override string ToString() => $"{{\"{path}\", \"{name}\", {unity}}}";
}
