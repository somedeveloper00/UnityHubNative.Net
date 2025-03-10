using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using MsBox.Avalonia;

namespace UnityHubNative.Net;

class MainWindow : Window
{
    const string InstallUnityUrl = "https://unity.com/releases/editor/archive";

    public static MainWindow Instance { get; set; }
    static ListBox s_unityInstallationsParent;

    static ListBox s_unityInstalltionSearchPathsParent;
    static Button s_unityInstallationSearchRemoveBtn;

    static SubmitableListBox s_unityProjectsParent;
    static Button s_revealBtn;
    static Button s_removeFromListBtn;
    static Button s_openWithBtn;
    static AutoCompleteBox s_projectSearchBoxAutoComplete;

    static MenuItem s_removeFromListMenuItem;
    static MenuItem s_revealInFileExplorerMenuItem;
    static MenuItem s_openInDifferentVersionMenuItem;

    static DockPanel s_transparentPanel;
    static Slider s_backgroundBlurIntensitySlider;

    public MainWindow(object data)
    {
        Instance = this;
        DataContext = data;
        Title = "UnityHubNative.Net";
        Content = CreateContent();
        ReloadEverything();
        SizeToContent = SizeToContent.WidthAndHeight;
        SetupBackground();
        ActualThemeVariantChanged += (_, _) => SetupBackground();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        // focus on searchbar if typed a character
        if (e.KeyModifiers is KeyModifiers.None or KeyModifiers.Shift && (int)e.Key >= (int)Key.A && (int)e.Key <= (int)Key.Z)
        {
            if (!s_projectSearchBoxAutoComplete.IsKeyboardFocusWithin)
            {
                s_projectSearchBoxAutoComplete.Focus();
                s_projectSearchBoxAutoComplete.Text += e.KeyModifiers == KeyModifiers.Shift ? e.Key.ToString() : e.Key.ToString().ToLower();
                s_projectSearchBoxAutoComplete.CaretIndex = s_projectSearchBoxAutoComplete.Text.Length;
                e.Handled = true;
            }
            return;
        }

        // focus on the list of escaped
        if (e.Key == Key.Escape)
        {
            if (s_unityProjectsParent.SelectedItem != null)
            {
                s_unityProjectsParent.ContainerFromIndex(GetUnityProjectSelectedIndex())!.Focus();
                s_projectSearchBoxAutoComplete.Text = string.Empty;
            }

            return;
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (s_unityProjectsParent.SelectedItem != null)
            s_unityProjectsParent.ContainerFromIndex(0)!.Focus();
    }

    void SetupBackground()
    {
        if (UnityHubNativeNetApp.Config.transparent)
        {
            TransparencyLevelHint =
            [
                UnityHubNativeNetApp.Config.acrylic ? WindowTransparencyLevel.AcrylicBlur : WindowTransparencyLevel.Mica,
                WindowTransparencyLevel.Blur,
            ];
#if Windows

            Background = UnityHubNativeNetApp.Config.acrylic
                ? new SolidColorBrush(
                    ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark ? Colors.Black : Colors.White,
                    1 - UnityHubNativeNetApp.Config.blurIntensity)
                : Brushes.Transparent;
#endif
        }
    }

    static void ReloadEverything()
    {
        UnityHubUtils.LoadAll();
        UpdateUnityVersionViews();
        UpdateUnitySearchPathViews();
        UpdateUnityProjectViews();
    }

    static Control CreateContent() => new DockPanel
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
                    InputGesture = new KeyGesture(Key.N, KeyModifiers.Control),
                }.OnClick(OnCreateNewProjectClicked),
                new MenuItem
                {
                    Header = "_Add Existing Project",
                    HotKey = new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift),
                    InputGesture = new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift)
                }.OnClick(OnAddExistingProjectClicked),
                s_removeFromListMenuItem = new MenuItem
                {
                    Header = "_Remove From List",
                    HotKey = new KeyGesture(Key.Subtract, KeyModifiers.Control),
                    InputGesture = new KeyGesture(Key.Subtract, KeyModifiers.Control)
                }.OnClick(OnRemoveProjectFromListClicked),
                new Separator(),
                s_revealInFileExplorerMenuItem = new MenuItem
                {
                    Header = "_Reveal In File Explorer",
                    HotKey = new KeyGesture(Key.F, KeyModifiers.Control),
                    InputGesture = new KeyGesture(Key.F, KeyModifiers.Control),
                }.OnClick(OnRevealProjectClicked),
                s_openInDifferentVersionMenuItem = new MenuItem
                {
                    Header = "_Open In Different Version",
                    HotKey = new KeyGesture(Key.O, KeyModifiers.Control),
                    InputGesture = new KeyGesture(Key.O, KeyModifiers.Control),
                }.OnClick(OnOpenWithClicked),
                new MenuItem
                {
                    Header = "_Reload Data",
                    HotKey = new KeyGesture(Key.R, KeyModifiers.Control),
                    InputGesture = new KeyGesture(Key.R, KeyModifiers.Control),
                }.OnClick(ReloadEverything),
            ]),
            new MenuItem
            {
                Header = "_Window",
            }.AddItems
            ([
                new MenuItem
                {
                    Header = "_Close Window",
                    HotKey = new(Key.W, KeyModifiers.Control),
                    InputGesture = new(Key.W, KeyModifiers.Control)
                }.OnClick(static () => Instance.Close()),
                new MenuItem
                {
                    Header = "_About UnityHubNative.Net",
                }.OnClick(OnAboutClicked),
            ]),
        ]),
        new DockPanel
        {
        }.AddChildren
        ([
            new TabControl
            {
                TabStripPlacement = Dock.Top,
            }.AddItems
            ([
                new TabItem
                {
                    Header = "Projects",
                    Content = new DockPanel
                    {
                    }.AddChildren
                    ([
                        s_projectSearchBoxAutoComplete = new SubmittableAutoCompleteBox<UnityProjectView>
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
                        }.OnSubmit(u =>
                        {
                            u.unityProject.OpenProject();
                            return;
                        }).SetDock(Dock.Top),
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
                            }.OnClick(OnRevealProjectClicked),
                            s_removeFromListBtn = new Button
                            {
                                Content = "Remove From List",
                                IsEnabled = false,
                            }.OnClick(OnRemoveProjectFromListClicked),
                            new Button
                            {
                                Content = "Create New"
                            }.OnClick(OnCreateNewProjectClicked),
                            new Button
                            {
                                Content = "Add Existing"
                            }.OnClick(OnAddExistingProjectClicked),
                            s_openWithBtn = new Button
                            {
                                Content = "Open With",
                                IsEnabled = false,
                            }.OnClick(OnOpenWithClicked),
                        ]),
                        new DockPanel
                        {
                            Margin = new(0, 10)
                        }.AddChildren
                        ([
                            new ScrollViewer
                            {
                                Content = s_unityProjectsParent = new SubmitableListBox
                                {
                                    WrapSelection = true,
                                    SelectionMode = SelectionMode.AlwaysSelected | SelectionMode.Single,
                                    SelectedIndex = 0,
                                }.AddOnSubmit(OnUnityProjectListSubmitted).OnSelectionChanged(UnityProjectSelectedIndexChanged)
                            },
                        ]),
                    ])
                },
                new TabItem
                {
                    Header = "Unity Versions",
                    //FontSize = 15,
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
                },
                new TabItem
                {
                    Header = "Options",
                    Content = new DockPanel
                    {
                        LastChildFill = false
                    }.AddChildren
                    ([
                        new SettingsExpander
                        {
                            Header = new DockPanel
                            {
                                LastChildFill = false
                            }.AddChildren
                            ([
                                new TextBlock
                                {
                                    Text = "Transparent Window",
                                    VerticalAlignment = VerticalAlignment.Center,
                                }.SetDock(Dock.Left),
                                new CheckBox
                                {
                                    IsChecked = UnityHubNativeNetApp.Config.transparent,
                                    VerticalAlignment = VerticalAlignment.Center,
                                }.OnCheckChanged(OnTransparencyCheckboxChanged).SetDock(Dock.Right),
                            ]).SetTooltip("Makes the window transparent. Uses Mica on Windows and the desktop's blur on Linux.\nNeeds restart to take effect."),
                        }.SetDock(Dock.Top).AddItems
                        ([
                            new SettingsExpanderItem
                            {
                                Content = s_transparentPanel = new DockPanel
                                {
                                    IsEnabled = UnityHubNativeNetApp.Config.transparent,
                                    LastChildFill = false,
                                }.AddChildren
                                ([
                                    new DockPanel
                                    {
                                        LastChildFill = false
                                    }.AddChildren
                                    ([
                                        new TextBlock
                                        {
                                            Text = "Acrilyc",
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetTooltip("Use Acrylic blur. Only works on Windows.\nNeeds restart to take effect.").SetDock(Dock.Left),
                                        new CheckBox
                                        {
                                            IsChecked = UnityHubNativeNetApp.Config.acrylic,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.OnCheckChanged(OnAcrylicCheckboxChanged).SetDock(Dock.Right),
                                    ]).SetDock(Dock.Top),
                                ]),
                            },
                            new SettingsExpanderItem
                            {
                                Content = new DockPanel
                                {
                                    LastChildFill = false,
                                    IsEnabled = UnityHubNativeNetApp.Config.transparent && UnityHubNativeNetApp.Config.acrylic,
                                }.AddChildren
                                ([
                                    new TextBlock
                                    {
                                        Text = "Background Blur Intensity",
                                        VerticalAlignment = VerticalAlignment.Center,
                                    }.SetTooltip("Changes the intensity of the background blur.").SetDock(Dock.Left),
                                    s_backgroundBlurIntensitySlider = new Slider
                                    {
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Minimum = 0,
                                        Maximum = 1,
                                        Width = 100,
                                        TickFrequency = 0.1,
                                        TickPlacement = TickPlacement.BottomRight,
                                        IsSnapToTickEnabled = true,
                                        Value = UnityHubNativeNetApp.Config.blurIntensity
                                    }.OnValueChanged(OnAcrylicIntensitySliderValueChanged).SetDock(Dock.Right)
                                ]).SetDock(Dock.Top)
                            }
                        ]),
                        new SettingsExpander
                        {
                            Header = new DockPanel
                            {
                                LastChildFill = false
                            }.AddChildren
                            ([
                                new TextBlock
                                {
                                    Text = "Behaviour",
                                    VerticalAlignment = VerticalAlignment.Center,
                                }.SetDock(Dock.Left)
                            ]),
                        }.SetDock(Dock.Top).AddItems
                        ([
                            new SettingsExpanderItem
                            {
                                Content = new DockPanel
                                {
                                    LastChildFill = false
                                }.AddChildren
                                ([
                                    new TextBlock
                                    {
                                        Text = "Close after opening a project",
                                        VerticalAlignment = VerticalAlignment.Center,
                                    }.SetDock(Dock.Left),
                                    new CheckBox
                                    {
                                        IsChecked = UnityHubNativeNetApp.Config.closeAfterProjectOpen,
                                        VerticalAlignment = VerticalAlignment.Center,
                                    }.OnCheckChanged(OnCloseAfterOpenProjectCheckboxChanged).SetDock(Dock.Right)
                                ])
                            }.SetTooltip("If checked, the app will close after opening a project")
                        ])
                    ])
                }
            ])
        ])
    ]);

    static void OnCloseAfterOpenProjectCheckboxChanged()
    {
        UnityHubNativeNetApp.Config.closeAfterProjectOpen = !UnityHubNativeNetApp.Config.closeAfterProjectOpen;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
    }

    static void OnAcrylicIntensitySliderValueChanged()
    {
        UnityHubNativeNetApp.Config.blurIntensity = (float)s_backgroundBlurIntensitySlider.Value;
        Instance.SetupBackground();
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
    }

    static void OnAcrylicCheckboxChanged()
    {
        UnityHubNativeNetApp.Config.acrylic = !UnityHubNativeNetApp.Config.acrylic;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
    }

    static void OnTransparencyCheckboxChanged()
    {
        UnityHubNativeNetApp.Config.transparent = !UnityHubNativeNetApp.Config.transparent;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
        s_transparentPanel.IsEnabled = UnityHubNativeNetApp.Config.transparent;
    }

    static Task<IEnumerable<object>> PopulateUnityProjectSearchAutoCompletion(string? filter, CancellationToken _)
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

    static void OnUnityProjectListSubmitted()
    {
        if (!IsAnyProjectSelected())
            return;
        ((UnityProjectView)s_unityProjectsParent.Items[GetUnityProjectSelectedIndex()]!).OpenProject();
    }

    static void RemoveSelectedUnitySearchPath(Button button, RoutedEventArgs args)
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

    static void UnityInstallationSearchPathSelectedIndexChanged()
    {
        var index = GetSelectedUnityInstallationSearchPathsIndex();
        if (index < 0 || index >= UnityHubUtils.UnityInstallationSearchPaths.Count)
            s_unityInstallationSearchRemoveBtn.IsEnabled = false;
        else
            s_unityInstallationSearchRemoveBtn.IsEnabled = true;
    }

    static void UnityProjectSelectedIndexChanged()
    {
        var index = GetUnityProjectSelectedIndex();
        Debug.WriteLine($"selection changed to {index}");
        bool isAnySelected = IsAnyProjectSelected();

        // menu bar buttons
        s_revealBtn.IsEnabled = s_removeFromListBtn.IsEnabled = s_openWithBtn.IsEnabled = isAnySelected;

        // menu items 
        s_removeFromListMenuItem.IsEnabled = s_revealInFileExplorerMenuItem.IsEnabled = s_openInDifferentVersionMenuItem.IsEnabled = isAnySelected;
    }

    static async void AddNewUnitySearchPath()
    {
        try
        {
            var paths = await Instance.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
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
            Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            _ = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowWindowDialogAsync(Instance);
        }
    }

    static int GetSelectedUnityInstallationSearchPathsIndex() => s_unityInstalltionSearchPathsParent.SelectedIndex;

    static int GetUnityProjectSelectedIndex() => s_unityProjectsParent.SelectedIndex;

    static bool IsAnyProjectSelected()
    {
        var ind = GetUnityProjectSelectedIndex();
        return ind >= 0 && ind < UnityHubUtils.UnityProjects.Count;
    }

    static async void OnAddExistingProjectClicked()
    {
        try
        {
            var paths = await Instance.StorageProvider.OpenFolderPickerAsync(new()
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
                    _ = MessageBoxManager.GetMessageBoxStandard($"Project \"{folder}\" has already been added.", "Cannot add project", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info).ShowWindowDialogAsync(Instance);
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
            _ = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowWindowDialogAsync(Instance);
            Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
        }
    }

    static void OnOpenWithClicked()
    {
        var dialogue = new OpenWithDialogue(UnityHubUtils.UnityProjects[GetUnityProjectSelectedIndex()]);
        dialogue.ShowDialog(Instance);
    }

    static void OnCreateNewProjectClicked() => new CreateNewProjectDialogue().ShowDialog(Instance);

    static void OnRemoveProjectFromListClicked()
    {
        UnityHubUtils.UnityProjects.RemoveAt(GetUnityProjectSelectedIndex());
        UnityHubUtils.SaveUnityProjects();
        UnityHubUtils.LoadUnityProjects();
        UpdateUnityProjectViews();
    }

    static void OnRevealProjectClicked() => OsUtils.OpenExplorer(UnityHubUtils.UnityProjects[GetUnityProjectSelectedIndex()].path);

    static void OnAboutClicked(MenuItem item, RoutedEventArgs args) => new AboutDialogue().ShowDialog(Instance);

    static void UpdateUnityVersionViews()
    {
        SyncListBoxWithView<UnityInstallation, UnityInstallationView>(s_unityInstallationsParent, UnityHubUtils.UnityInstallations);

        for (int i = 0; i < UnityHubUtils.UnityInstallations.Count; i++)
            ((UnityInstallationView)s_unityInstallationsParent.Items[i]!).Update(UnityHubUtils.UnityInstallations[i]);
    }

    static void UpdateUnitySearchPathViews()
    {
        SyncListBoxWithView<string, UnityInstallationSearchPathView>(s_unityInstalltionSearchPathsParent, UnityHubUtils.UnityInstallationSearchPaths);

        for (int i = 0; i < UnityHubUtils.UnityInstallationSearchPaths.Count; i++)
            ((UnityInstallationSearchPathView)s_unityInstalltionSearchPathsParent.Items[i]!).Update(UnityHubUtils.UnityInstallationSearchPaths[i]);
    }

    static void UpdateUnityProjectViews()
    {
        SyncListBoxWithView<UnityProject, UnityProjectView>(s_unityProjectsParent, UnityHubUtils.UnityProjects);

        for (int i = 0; i < UnityHubUtils.UnityProjects.Count; i++)
            ((UnityProjectView)s_unityProjectsParent.Items[i]!).Update(UnityHubUtils.UnityProjects[i]);
    }

    static void SyncListBoxWithView<TItem, TView>(ListBox parent, List<TItem> items) where TView : new()
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

    static void ShowTbiDialogue()
    {
        _ = MessageBoxManager.GetMessageBoxStandard("To be implemented", "Not implemented yet", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Warning).ShowWindowDialogAsync(Instance);
    }
}

