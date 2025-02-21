using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Classic.CommonControls.Dialogs;

namespace Counter;

class MainWindow : Window
{
    private const string InstallUnityUrl = "https://unity.com/releases/editor/archive";

    private static readonly List<UnityInstallation> s_unityInstallations = [];
    private static readonly List<UnityProject> s_projects = [];
    private static readonly HashSet<Control> s_removingControls = [];

    private static MainWindow s_instance;
    private static ListBox s_unityInstallationsParent;

    private static ListBox s_unityInstalltionSearchPathsParent;
    private static Button s_unityInstallationSearchRemoveBtn;
    private static Button s_revealBtn;
    private static Button s_removeFromListBtn;
    private static Button s_createNewBtn;
    private static Button s_addExistingBtn;
    private static Button s_openWithBtn;

    public MainWindow(object data)
    {
        s_instance = this;
        DataContext = data;
        Title = "Counter App";
        Content = CreateContent();
        UnityHubUtils.LoadAll();
        UpdateUnityVersionViews();
        UpdateUnitySearchPathViews();
        SizeToContent = SizeToContent.WidthAndHeight;
    }

    private static Panel CreateContent()
    {
        return new DockPanel
        {
            LastChildFill = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        }.AddChildren
        ([
            new Menu
            {
            }.SetDock(Dock.Top).AddItems
            ([
                new MenuItem
                {
                    Header = "_Project"
                }.AddItems
                ([
                    new MenuItem
                    {
                        Header = "_Create New Project",
                        HotKey = new KeyGesture(Key.N, KeyModifiers.Control),
                        Command = new MenuCommand(executed: CreateNewProject),
                    },
                    new MenuItem
                    {
                        Header = "_Add Existing Project",
                        HotKey = new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift),
                        Command = new MenuCommand(executed: AddExistingProject),
                    },
                    new MenuItem
                    {
                        Header = "_Remove From List",
                        HotKey = new KeyGesture(Key.OemMinus, KeyModifiers.Control),
                        Command = new MenuCommand(IsAnyProjectSelected, RemoveSelectedUnityProjectFromList),
                    },
                    new MenuItem
                    {
                        Header = "_Reveal In File Explorer",
                        HotKey = new KeyGesture(Key.F, KeyModifiers.Control),
                        Command = new MenuCommand(IsAnyProjectSelected, RevealSelectedUnityProjectInExplorer),
                    },
                ]),
                new MenuItem
                {
                    Header = "_Open"
                }.AddItems
                ([
                    new MenuItem
                    {
                        Header = "New"
                    },
                    new MenuItem
                    {
                        Header = "Load"
                    }
                ])
            ]),
            new DockPanel
            {
            }.AddChildren
            ([
                new TabControl
                {
                    Margin = new(5),
                }.AddItems
                ([
                    new TabItem
                    {
                        Header = "Projects",
                        FontSize = 15,
                        Content = new DockPanel
                        {
                        }.AddChildren
                        ([
                            new TextBox
                            {
                                Watermark = "Search projects...",
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                InnerRightContent = new Label
                                {
                                    Content = "🔍",
                                    HorizontalAlignment = HorizontalAlignment.Right,
                                },
                            }.SetDock(Dock.Top),
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Margin = new(0, 5),
                                Spacing = 2,
                            }.SetDock(Dock.Top).AddChildren
                            ([
                                s_revealBtn = new Button
                                {
                                    Content = "Reveal",
                                    IsEnabled = false,
                                },
                                s_removeFromListBtn = new Button
                                {
                                    Content = "Remove From List",
                                    IsEnabled = false,
                                },
                                s_createNewBtn = new Button
                                {
                                    Content = "Create New"
                                },
                                new Button
                                {
                                    Content = "Add Existing"
                                },
                                s_openWithBtn = new Button
                                {
                                    Content = "Open With",
                                    IsEnabled = false,
                                },
                            ]),
                            new DockPanel
                            {

                            }
                        ])
                    },
                    new TabItem
                    {
                        Header = "Unity Versions",
                        FontSize = 15,
                        Content = new DockPanel
                        {
                        }.AddChildren
                        ([
                            new DockPanel
                            {
                            }.SetDock(Dock.Bottom).AddChildren
                            ([
                                new Label
                                {
                                    Content = "Install Search Paths",
                                    HorizontalAlignment = HorizontalAlignment.Left
                                }.SetDock(Dock.Top),
                                new StackPanel
                                {
                                    Margin = new(5),
                                    Width = 120
                                }.SetDock(Dock.Right).AddChildren
                                ([
                                    new Button
                                    {
                                        Content = "Add Location",
                                        HorizontalAlignment = HorizontalAlignment.Stretch,
                                        IsEnabled = true,
                                    }.SetTooltip("Adds a new Unity search path")
                                    .OnClick(AddNewUnitySearchPath),
                                    s_unityInstallationSearchRemoveBtn = new Button
                                    {
                                        Content = "Remove\nLocation",
                                        HorizontalAlignment = HorizontalAlignment.Stretch,
                                        HorizontalContentAlignment = HorizontalAlignment.Center,
                                        IsEnabled = false
                                    }.SetTooltip("Removes the selected Unity search path item")
                                    .OnClick(RemoveSelectedUnitySearchPath)
                                ]),
                                new DockPanel
                                {
                                }.AddChildren
                                ([
                                    new ScrollViewer
                                    {
                                        Content = s_unityInstalltionSearchPathsParent = new ListBox
                                        {
                                        }.OnSelectionChanged(UnityInstallationSearchPathSelectedIndexChanged)
                                    }
                                ])
                            ]),
                            new Label
                            {
                                Content = "Detected Installations",
                            }.SetDock(Dock.Top),
                            new StackPanel
                            {
                                Width = 120,
                                Margin = new(5),
                                Spacing = 5,
                            }.SetDock(Dock.Right).AddChildren
                            ([
                                new Button
                                {
                                    Content = "Install New",
                                    HorizontalAlignment = HorizontalAlignment.Stretch
                                }.SetTooltip($"Install a new Unity Editor version\n{InstallUnityUrl}")
                                .OnClick(() => UrlUtils.OpenUrl(InstallUnityUrl)),
                                new Button
                                {
                                    Content = "Reload",
                                    HorizontalAlignment = HorizontalAlignment.Stretch
                                }.SetTooltip("Reload the list")
                                .OnClick(UnityHubUtils.LoadUnityInstallations, UpdateUnityVersionViews),
                            ]),
                            new DockPanel
                            {
                            }.AddChildren
                            ([
                                new ScrollViewer
                                {
                                    Content = s_unityInstallationsParent = new ListBox
                                    {
                                    }
                                }
                            ])
                        ])
                    }
                ])
            ])
        ]);
    }

    private static void RemoveSelectedUnitySearchPath(Button button, RoutedEventArgs args)
    {
        var index = GetSelectedUnityInstallationSearchPathsIndex();
        if (index < 0 || index >= UnityHubUtils.UnityInstallationSearchPaths.Count)
            return;
        UnityHubUtils.UnityInstallationSearchPaths.RemoveAt(index);
        UnityHubUtils.SaveUnityInstallationSearchPaths();
        UnityHubUtils.LoadUnityInstallations();
        UpdateUnitySearchPathViews();
        UpdateUnityVersionViews();
        s_unityInstalltionSearchPathsParent.UnselectAll();
        s_unityInstallationSearchRemoveBtn.IsEnabled = false;
    }

    private static void UnityInstallationSearchPathSelectedIndexChanged()
    {
        var index = GetSelectedUnityInstallationSearchPathsIndex();
        if (index < 0 || index >= UnityHubUtils.UnityInstallationSearchPaths.Count)
            s_unityInstallationSearchRemoveBtn.IsEnabled = false;
        else
            s_unityInstallationSearchRemoveBtn.IsEnabled = true;
    }

    private static async void AddNewUnitySearchPath()
    {
        try
        {
            var paths = await s_instance.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = true,
                Title = "Pick Folder to search for Unity Installations"
            });
            bool dirty = false;
            foreach (var path in paths)
            {
                if (path == null || path.Path == null)
                    continue;
                var folder = path.TryGetLocalPath();
                if (string.IsNullOrEmpty(folder))
                    continue;

                UnityHubUtils.UnityInstallationSearchPaths.Add(folder);
                dirty = true;
            }
            if (dirty)
            {
                UnityHubUtils.SaveUnityInstallationSearchPaths();
                UnityHubUtils.LoadUnityInstallations();
                UpdateUnitySearchPathViews();
                UpdateUnityVersionViews();
            }
        }
        catch (Exception ex)
        {
            await MessageBox.ShowDialog(s_instance, "Error", ex.Message, MessageBoxButtons.Ok, MessageBoxIcon.Error);
            Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
        }
    }

    private static int GetSelectedUnityInstallationIndex() => s_unityInstallationsParent.SelectedIndex;
    private static int GetSelectedUnityInstallationSearchPathsIndex() => s_unityInstalltionSearchPathsParent.SelectedIndex;

    private static void LoadUnityInstallations()
    {

    }

    private static void RevealSelectedUnityProjectInExplorer()
    {
        throw new NotImplementedException();
    }

    private static bool IsAnyProjectSelected()
    {
        throw new NotImplementedException();
    }

    private static bool CanRemoveSelectedUnityProjectFromList()
    {
        throw new NotImplementedException();
    }

    private static void RemoveSelectedUnityProjectFromList()
    {
        throw new NotImplementedException();
    }

    private static void AddExistingProject()
    {
        throw new NotImplementedException();
    }

    private static void CreateNewProject()
    {
        throw new NotImplementedException();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
    }

    private static void UpdateUnityVersionViews()
    {
        // update
        for (int i = 0; i < UnityHubUtils.UnityInstallations.Count; i++)
        {
            if (s_unityInstallationsParent.Items.Count <= i)
            {
                // create
                UnityInstallationView view = new();
                s_unityInstallationsParent.AddItems(view);
            }
            ((UnityInstallationView)s_unityInstallationsParent.Items[i]!).Update(UnityHubUtils.UnityInstallations[i]);
        }
        // delete
        for (int i = s_unityInstallationsParent.Items.Count - 1; i >= UnityHubUtils.UnityInstallations.Count; i--)
            s_unityInstallationsParent.Items.RemoveAt(i);
    }

    private static void UpdateUnitySearchPathViews()
    {
        // update
        for (int i = 0; i < UnityHubUtils.UnityInstallationSearchPaths.Count; i++)
        {
            if (s_unityInstalltionSearchPathsParent.Items.Count <= i)
            {
                // create
                UnityInstallationSearchPathView view = new();
                s_unityInstalltionSearchPathsParent.AddItems(view);
            }
            ((UnityInstallationSearchPathView)s_unityInstalltionSearchPathsParent.Items[i]!).Update(UnityHubUtils.UnityInstallationSearchPaths[i]);
        }
        // delete
        for (int i = s_unityInstalltionSearchPathsParent.Items.Count - 1; i >= UnityHubUtils.UnityInstallationSearchPaths.Count; i--)
            s_unityInstalltionSearchPathsParent.Items.RemoveAt(i);
    }
}
