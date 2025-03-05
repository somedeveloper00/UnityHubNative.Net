using System.Data;
using System.Diagnostics;
using MsBox.Avalonia;

namespace Counter;

static class UnityHubUtils
{
    public static List<UnityInstallation> UnityInstallations = [];
    public static List<UnityProject> UnityProjects = [];
    public static List<string> UnityInstallationSearchPaths = [];

    static readonly string s_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnityHubNative");
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

    static readonly string s_projectVersionPath = Path.Combine("ProjectSettings", "ProjectVersion.txt");

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
            Debug.WriteLine("found Unity installation search paths data at \"{0}\"", s_searchPathsPath);
            UnityInstallationSearchPaths.AddRange(File.ReadAllLines(s_searchPathsPath));
        }
        else
        {
            Debug.WriteLine("did not find Unity installation search paths data. using defaults");
            UnityInstallationSearchPaths.AddRange(s_unityInstallationSearchPathsDefault);
            SaveUnityInstallationSearchPaths();
        }
        Debug.WriteLine("found Unity installation search paths:");
        for (int i = 0; i < UnityInstallationSearchPaths.Count; i++)
            Debug.WriteLine("-> {0}", UnityInstallationSearchPaths[i]);
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
            Debug.WriteLine("saved Unity installation paths to \"{0}\"", s_searchPathsPath);
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
        Debug.WriteLine("found Unity installations:");
        for (int i = 0; i < UnityInstallations.Count; i++)
            Debug.WriteLine("-> {0}", UnityInstallations[i]);
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
                if (UnityProject.TryLoadUnityProject(lines[i], out var result))
                    UnityProjects.Add(result);
        }
        Debug.WriteLine("found Unity projects:");
        for (int i = 0; i < UnityProjects.Count; i++)
            Debug.WriteLine("-> {0}", UnityProjects[i]);
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
    /// versionString for the unity project at <paramref name="path"/>. Returns -1 if not found.
    /// </summary>
    public static int FindUnityVersionIndex(string path, out string versionString)
    {
        EnsureSaveDirectoryExists();
        var projectVersionFile = Path.Combine(path, s_projectVersionPath);
        if (File.Exists(projectVersionFile))
        {
            var lines = File.ReadAllLines(projectVersionFile);
            versionString = string.Empty;
            for (int i = 0; i < lines.Length; i++)
            {
                versionString = ExtractVersionFromDirName(lines[i]);
                if (!string.IsNullOrEmpty(versionString))
                    break;
            }
            if (string.IsNullOrEmpty(versionString))
            {
                versionString = string.Empty;
                return -1;
            }
            for (int i = 0; i < UnityInstallations.Count; i++)
                if (UnityInstallations[i].version == versionString)
                    return i;
        }
        versionString = string.Empty;
        return -1;
    }

    private static void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(s_dir))
        {
            Directory.CreateDirectory(s_dir);
            Debug.WriteLine("created local data directory \"{0}\"", s_dir);
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

class UnityProject(string path, DateTime lastModifiedDate, UnityInstallation? unity)
{
    public readonly string path = path;
    public readonly string name = Path.GetFileName(path);
    public readonly DateTime lastModifiedDate = lastModifiedDate;
    public readonly UnityInstallation? unity = unity;

    public static bool TryLoadUnityProject(string path, out UnityProject result)
    {
        try
        {
            var ind = UnityHubUtils.FindUnityVersionIndex(path, out var version);
            UnityInstallation? unityInstallation = ind != -1 ? UnityHubUtils.UnityInstallations[ind] : null;
            var lastModDate = new DirectoryInfo(path).LastWriteTime;
            result = new(path, lastModDate, unityInstallation);
            return true;
        }
        catch (Exception exception)
        {
            MessageBoxManager.GetMessageBoxStandard("Unable to add project", exception.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Warning);
            Debug.WriteLine($"{exception.Message}\n{exception.StackTrace}");
            result = default;
            return false;
        }
    }

    public override string ToString() => $"{{\"{path}\", \"{name}\", {unity}}}";
}
