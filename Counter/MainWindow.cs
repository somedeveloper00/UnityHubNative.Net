using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;

namespace Counter;

class MainWindow : Window
{
    private const string InstallUnityUrl = "https://unity.com/releases/editor/archive";

    private static MainWindow s_instance;
    private static ListBox s_unityInstallationsParent;

    private static ListBox s_unityInstalltionSearchPathsParent;
    private static Button s_unityInstallationSearchRemoveBtn;

    private static SubmitableListBox s_unityProjectsParent;
    private static Button s_revealBtn;
    private static Button s_removeFromListBtn;
    private static Button s_createNewBtn;
    private static Button s_addExistingBtn;
    private static Button s_openWithBtn;
    private static TextBox s_projectSearchBox;
    private static AutoCompleteBox s_projectSearchBoxAutoComplete;

    public MainWindow(object data)
    {
        s_instance = this;
        DataContext = data;
        Title = "Counter CounterApp";
        Content = CreateContent();
        UnityHubUtils.LoadAll();
        UpdateUnityVersionViews();
        UpdateUnitySearchPathViews();
        UpdateUnityProjectViews();
        SizeToContent = SizeToContent.WidthAndHeight;
        TransparencyLevelHint =
        [
            WindowTransparencyLevel.Mica,
        ];
        Background = Brushes.Transparent;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private static Control CreateContent() => new DockPanel
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
                            //s_projectSearchBox = new TextBox
                            //{
                            //    Watermark = "Search projects...",
                            //    HorizontalAlignment = HorizontalAlignment.Stretch,
                            //    InnerRightContent = new Label
                            //    {
                            //    }
                            //}.OnTextChanged(OnProjectSearchTextChanged).SetDock(Dock.Top),
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
                                s_addExistingBtn = new Button
                                {
                                    Content = "Add Existing"
                                }.OnClick(AddExistingProject),
                                s_openWithBtn = new Button
                                {
                                    Content = "Open With",
                                    IsEnabled = false,
                                },
                            ]),
                            s_projectSearchBoxAutoComplete = new AutoCompleteBox
                            {
                                FilterMode = AutoCompleteFilterMode.None,
                                IsTextCompletionEnabled = true,
                                AsyncPopulator = PopulateUnityProjectSearchAutoCompletion,
                                Watermark = "Search Projects",
                                InnerRightContent = new Label
                                {
                                    Content = "🔍",
                                    HorizontalAlignment = HorizontalAlignment.Right,
                                },
                            }.SetDock(Dock.Top),
                            new DockPanel
                            {
                            }.AddChildren
                            ([
                                new ScrollViewer
                                {
                                    Content = s_unityProjectsParent = new SubmitableListBox
                                    {
                                    }.AddOnSubmit(OnUnityProjectListSubmitted).OnSelectionChanged(UnityProjectSelectedIndexChanged)
                                },
                            ]),
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
                                        SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected,
                                        Focusable = true
                                    }
                                },
                            ])
                        ])
                    }
                ])
            ])
        ]);

    private static Task<IEnumerable<object>> PopulateUnityProjectSearchAutoCompletion(string? filter, CancellationToken _)
    {
        if (filter == null)
        {
            return Task.FromResult<IEnumerable<object>>([]);
        }
        else
        {
            var views = UnityHubUtils.UnityProjects.Where(p =>
                p.path.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                p.lastModifiedDate.ToLongDateString().Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                p.name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                p.unity.HasValue && p.unity.Value.path.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                p.unity.HasValue && p.unity.Value.version.Contains(filter, StringComparison.InvariantCultureIgnoreCase)
            //).Select(p => p.name);
            ).Select(p => new UnityProjectView(p));
            return Task.FromResult<IEnumerable<object>>(views);
        }
    }

    private static void OnUnityProjectListSubmitted()
    {
        if (IsAnyProjectSelected())
        {
            ((UnityProjectView)s_unityProjectsParent!.Items[GetUnityProjectSelectedIndex()]!).OpenProject();
        }
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
        UpdateUnityProjectViews();
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

    private static void UnityProjectSelectedIndexChanged()
    {
        var index = GetUnityProjectSelectedIndex();
        Debug.WriteLine($"selection changed to {index}");
        s_revealBtn.IsEnabled = s_removeFromListBtn.IsEnabled = s_createNewBtn.IsEnabled = s_openWithBtn.IsEnabled = IsAnyProjectSelected();
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
                if (path == null)
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
            MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
        }
    }

    private static int GetSelectedUnityInstallationIndex() => s_unityInstallationsParent.SelectedIndex;
    private static int GetSelectedUnityInstallationSearchPathsIndex() => s_unityInstalltionSearchPathsParent.SelectedIndex;
    private static int GetUnityProjectSelectedIndex() => s_unityProjectsParent.SelectedIndex;

    private static void RevealSelectedUnityProjectInExplorer()
    {
        throw new NotImplementedException();
    }

    private static bool IsAnyProjectSelected()
    {
        var ind = GetUnityProjectSelectedIndex();
        return ind >= 0 && ind < UnityHubUtils.UnityProjects.Count;
    }

    private static bool CanRemoveSelectedUnityProjectFromList() => IsAnyProjectSelected();

    private static void RemoveSelectedUnityProjectFromList()
    {
        throw new NotImplementedException();
    }

    private static async void AddExistingProject()
    {
        try
        {
            var paths = await s_instance.StorageProvider.OpenFolderPickerAsync(new()
            {
                AllowMultiple = true,
                Title = "Select the folder(s) containing the Unity Project"
            });
            foreach (var path in paths)
            {
                if (path == null)
                    continue;
                var folder = path.TryGetLocalPath();
                if (string.IsNullOrEmpty(folder))
                    continue;
                if (UnityHubUtils.UnityProjects.Any(p => p.path == folder))
                {
                    MessageBoxManager.GetMessageBoxStandard($"Project \"{folder}\" has already been added.", "Cannot add project", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
                    continue;
                }
                bool dirty = false;
                if (UnityProject.TryLoadUnityProject(folder, out var result))
                {
                    UnityHubUtils.UnityProjects.Add(result);
                    dirty = true;
                }
                if (dirty)
                {
                    UnityHubUtils.SaveUnityProjects();
                    UpdateUnityProjectViews();
                }

            }
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
        }
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
        SyncListBoxWithView<UnityInstallation, UnityInstallationView>(s_unityInstallationsParent, UnityHubUtils.UnityInstallations);

        for (int i = 0; i < UnityHubUtils.UnityInstallations.Count; i++)
            ((UnityInstallationView)s_unityInstallationsParent.Items[i]!).Update(UnityHubUtils.UnityInstallations[i]);
    }

    private static void UpdateUnitySearchPathViews()
    {
        SyncListBoxWithView<string, UnityInstallationSearchPathView>(s_unityInstalltionSearchPathsParent, UnityHubUtils.UnityInstallationSearchPaths);

        for (int i = 0; i < UnityHubUtils.UnityInstallationSearchPaths.Count; i++)
            ((UnityInstallationSearchPathView)s_unityInstalltionSearchPathsParent.Items[i]!).Update(UnityHubUtils.UnityInstallationSearchPaths[i]);
    }

    private static void UpdateUnityProjectViews()
    {
        SyncListBoxWithView<UnityProject, UnityProjectView>(s_unityProjectsParent, UnityHubUtils.UnityProjects);

        for (int i = 0; i < UnityHubUtils.UnityProjects.Count; i++)
            ((UnityProjectView)s_unityProjectsParent.Items[i]!).Update(UnityHubUtils.UnityProjects[i]);
    }

    private static void SyncListBoxWithView<TItem, TView>(ListBox parent, List<TItem> items) where TView : new()
    {
        // update
        for (int i = 0; i < items.Count; i++)
        {
            if (parent.Items.Count <= i)
            {
                // create
                TView view = new();
                parent.AddItems(view);
            }
        }
        // delete
        for (int i = parent.Items.Count - 1; i >= items.Count; i--)
            parent.Items.RemoveAt(i);
    }
}

