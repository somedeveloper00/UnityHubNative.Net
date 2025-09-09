using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using MsBox.Avalonia;

namespace UnityHubNative.Net;

sealed class MainWindow : Window
{
    const string InstallUnityUrl = "https://unity.com/releases/editor/archive";

    public static MainWindow Instance { get; set; }

    static ListBox s_unityInstallationsParent;

    static ListBox s_unityInstalltionSearchPathsParent;
    static Button s_unityInstallationSearchRemoveBtn;

    static SubmitableListBox s_unityProjectsParent;
    static AutoCompleteBox s_projectSearchBoxAutoComplete;

    static DockPanel s_transparentPanel;
    static Slider s_backgroundBlurIntensitySlider;
    static TextBox s_openInTerminalFormatText;
    static Key s_lastKey;
    static TabControl s_tabControl;
    static bool _updatingUnityProjectList;
    static ComboBox s_langaugeComboBox;

    public MainWindow(object data)
    {
        Instance = this;
        DataContext = data;
        Title = UnityHubNativeNetApp.Config.language.TitleBar;
        if (UnityHubNativeNetApp.Config.extendToTitlebar)
            ExtendClientAreaToDecorationsHint = true;
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
        if (e.Key == Key.Escape && s_lastKey == Key.Escape)
            Close();

        s_lastKey = e.Key;

        if (s_tabControl.SelectedIndex == 0)
        {
            // try open project
            if (e.Key == Key.Enter && TryGetSelectedProject(out var _))
            {
                OpenSelectedProject();
                e.Handled = true;
                return;
            }

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
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Key == Key.LeftShift)
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
            s_unityProjectsParent.ContainerFromIndex(s_unityInstallationsParent.SelectedIndex)!.Focus();
    }

    public static void ReloadEverything()
    {
        UnityHubUtils.LoadAll();
        UpdateUnityVersionViews();
        UpdateUnitySearchPathViews();
        UpdateUnityProjectViews();
        if (UnityHubNativeNetApp.Config.saveProjectSelection)
            LoadProjectSelectedIndex();
    }

    public static void OpenSelectedProjectWith()
    {
        if (TryGetSelectedProject(out var unityProject))
        {
            var dialogue = new OpenWithDialogue(unityProject);
            dialogue.ShowDialog(Instance);
        }
    }

    public static void OpenSelectedProjectInTerminal()
    {
        if (TryGetSelectedProject(out var unityProject))
        {
            OsUtils.OpenInTerminal(unityProject.path);
            if (UnityHubNativeNetApp.Config.closeAfterOpenInTerminal)
                Instance.Close();
        }
    }

    public static void OnRemoveProjectFromListClicked()
    {
        UnityHubUtils.UnityProjects.RemoveAt(GetUnityProjectSelectedIndex());
        UnityHubUtils.SaveUnityProjects();
        UnityHubUtils.LoadUnityProjects();
        UpdateUnityProjectViews();
    }

    public static void UpdateUnityVersionViews()
    {
        SyncListBoxWithView<UnityInstallation, UnityInstallationView>(s_unityInstallationsParent, UnityHubUtils.UnityInstallations);

        for (int i = 0; i < UnityHubUtils.UnityInstallations.Count; i++)
            ((UnityInstallationView)s_unityInstallationsParent.Items[i]!).Update(UnityHubUtils.UnityInstallations[i]);
    }

    public static void UpdateUnitySearchPathViews()
    {
        SyncListBoxWithView<string, UnityInstallationSearchPathView>(s_unityInstalltionSearchPathsParent, UnityHubUtils.UnityInstallationSearchPaths);

        for (int i = 0; i < UnityHubUtils.UnityInstallationSearchPaths.Count; i++)
            ((UnityInstallationSearchPathView)s_unityInstalltionSearchPathsParent.Items[i]!).Update(UnityHubUtils.UnityInstallationSearchPaths[i]);
    }

    public static void UpdateUnityProjectViews()
    {
        _updatingUnityProjectList = true;
        SyncListBoxWithView<UnityProject, UnityProjectView>(s_unityProjectsParent, UnityHubUtils.UnityProjects);

        for (int i = 0; i < UnityHubUtils.UnityProjects.Count; i++)
            ((UnityProjectView)s_unityProjectsParent.Items[i]!).Update(UnityHubUtils.UnityProjects[i]);
        _updatingUnityProjectList = false;
    }

    public static void MoveSelectedProjectDown()
    {
        var ind = GetUnityProjectSelectedIndex();
        if (ind == -1 || s_unityProjectsParent.Items[ind] == null)
            return;
        var targetIndex = ind == UnityHubUtils.UnityProjects.Count - 1 ? 0 : ind + 1;
        var unityProject = UnityHubUtils.UnityProjects[ind];
        UnityHubUtils.UnityProjects.RemoveAt(ind);
        UnityHubUtils.UnityProjects.Insert(targetIndex, unityProject);
        UnityHubUtils.SaveUnityProjects();
        var item = s_unityProjectsParent.Items[ind];
        s_unityProjectsParent.Items.RemoveAt(ind);
        s_unityProjectsParent.Items.Insert(targetIndex, item);
        s_unityProjectsParent.SelectedIndex = targetIndex;
        for (int i = 0; i < s_unityProjectsParent.Items.Count; i++)
            ((UnityProjectView)s_unityProjectsParent.Items[i]).Update(UnityHubUtils.UnityProjects[i]);
        s_unityProjectsParent.SetSelectedItem(targetIndex);
    }

    public static void MoveSelectedProjectUp()
    {
        var ind = GetUnityProjectSelectedIndex();
        if (ind == -1 || s_unityProjectsParent.Items[ind] == null)
            return;
        var targetIndex = ind == 0 ? UnityHubUtils.UnityProjects.Count - 1 : ind - 1;
        var unityProject = UnityHubUtils.UnityProjects[ind];
        UnityHubUtils.UnityProjects.RemoveAt(ind);
        UnityHubUtils.UnityProjects.Insert(targetIndex, unityProject);
        UnityHubUtils.SaveUnityProjects();
        var item = s_unityProjectsParent.Items[ind];
        s_unityProjectsParent.Items.RemoveAt(ind);
        s_unityProjectsParent.Items.Insert(targetIndex, item);
        s_unityProjectsParent.SelectedIndex = targetIndex;
        for (int i = 0; i < s_unityProjectsParent.Items.Count; i++)
            ((UnityProjectView)s_unityProjectsParent.Items[i]).Update(UnityHubUtils.UnityProjects[i]);
        s_unityProjectsParent.SetSelectedItem(targetIndex);
    }

    static Control CreateContent() => new DockPanel
    {
        LastChildFill = true,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
    }.AddChildren
    ([
        new DockPanel
        {
            LastChildFill = false
        }.AddChildren
        ([
            new Menu
            {
            }.SetDock(Dock.Left).AddItems
            ([
                new MenuItem
                {
                    Background = Brushes.Transparent,
                    Header = UnityHubNativeNetApp.Config.language.Menu_File
                }.AddItems
                ([
                    new MenuItem
                    {
                        Header = UnityHubNativeNetApp.Config.language.Menu_CreateNewProject,
                        HotKey = new KeyGesture(Key.N, KeyModifiers.Control),
                        InputGesture = new KeyGesture(Key.N, KeyModifiers.Control),
                    }.OnClick(OnCreateNewProjectClicked),
                    new MenuItem
                    {
                        Header = UnityHubNativeNetApp.Config.language.Menu_AddExistingProject,
                        HotKey = new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift),
                        InputGesture = new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift)
                    }.OnClick(OnAddExistingProjectClicked),
                    new MenuItem
                    {
                        Header = UnityHubNativeNetApp.Config.language.Menu_ReloadData,
                        HotKey = new KeyGesture(Key.R, KeyModifiers.Control),
                        InputGesture = new KeyGesture(Key.R, KeyModifiers.Control),
                    }.OnClick(ReloadEverything),
                ]),
                new MenuItem
                {
                    Header = UnityHubNativeNetApp.Config.language.Menu_Project,
                }.AddItems
                (CreateProjectMenuItems(() => (s_unityProjectsParent.SelectedItem as UnityProjectView)?.unityProject ?? null)),
                new MenuItem
                {
                    Header = UnityHubNativeNetApp.Config.language.Menu_Window,
                }.AddItems
                ([
                    new MenuItem
                    {
                        Header = UnityHubNativeNetApp.Config.language.Menu_CloseWindow,
                        HotKey = new(Key.W, KeyModifiers.Control),
                        InputGesture = new(Key.W, KeyModifiers.Control)
                    }.OnClick(static () => Instance.Close()),
                    new MenuItem
                    {
                        Header = UnityHubNativeNetApp.Config.language.Menu_About,
                    }.OnClick(OnAboutClicked),
                ]),
            ]),
        ]).SetDock(Dock.Top),
        new DockPanel
        {
        }.AddChildren
        ([
            s_tabControl = new TabControl
            {
                TabStripPlacement = Dock.Top,
            }.AddItems
            ([
                new TabItem
                {
                    Header = UnityHubNativeNetApp.Config.language.Tab_Projects,
                    Content = new DockPanel
                    {
                    }.AddChildren
                    ([
                        s_projectSearchBoxAutoComplete = new SubmittableAutoCompleteBox<UnityProjectView>
                        {
                            FilterMode = AutoCompleteFilterMode.None,
                            IsTextCompletionEnabled = true,
                            AsyncPopulator = PopulateUnityProjectSearchAutoCompletion,
                            Watermark = UnityHubNativeNetApp.Config.language.SearchProjects,
                            InnerRightContent = new Label
                            {
                                Content = "🔍",
                                HorizontalAlignment = HorizontalAlignment.Right,
                            },
                        }.OnSubmit(static u => u.unityProject.OpenProject()).SetDock(Dock.Top),
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
                                }.AddOnSubmit(OpenSelectedProject).OnSelectionChanged(UnityProjectSelectedIndexChanged)
                            },
                        ]),
                    ])
                },
                new TabItem
                {
                    Header = UnityHubNativeNetApp.Config.language.Tab_UnityVersions,
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
                                Content = UnityHubNativeNetApp.Config.language.InstallSearchPaths,
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
                                    Content = UnityHubNativeNetApp.Config.language.AddLocation,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    IsEnabled = true,
                                }.SetTooltip(UnityHubNativeNetApp.Config.language.AddsANewUnitySearchPath)
                                .OnClick(AddNewUnitySearchPath),
                                s_unityInstallationSearchRemoveBtn = new Button
                                {
                                    Content = UnityHubNativeNetApp.Config.language.RemoveLocation,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    IsEnabled = false
                                }.SetTooltip(UnityHubNativeNetApp.Config.language.RemovesTheSelectedUnitySearchPathItem)
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
                            Content = UnityHubNativeNetApp.Config.language.DetectedInstallations,
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
                                Content = UnityHubNativeNetApp.Config.language.InstallNew,
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            }.SetTooltip($"{UnityHubNativeNetApp.Config.language.InstallANewUnityEditorVersion}\n{InstallUnityUrl}")
                            .OnClick(() => UrlUtils.OpenUrl(InstallUnityUrl)),
                            new Button
                            {
                                Content = UnityHubNativeNetApp.Config.language.Reload,
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            }.SetTooltip(UnityHubNativeNetApp.Config.language.ReloadTheList)
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
                    Header = UnityHubNativeNetApp.Config.language.Tab_Options,
                    Content = new ScrollViewer
                    {
                        IsScrollInertiaEnabled = true,
                        HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
                        Content = new DockPanel
                        {
                            LastChildFill = false,
                            Margin = new(10),
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
                                        Text = UnityHubNativeNetApp.Config.language.Appearance,
                                        VerticalAlignment = VerticalAlignment.Center,
                                    }.SetTooltip(UnityHubNativeNetApp.Config.language.ControlTheAppearenceOfTheAppCanAffectPerformance).SetDock(Dock.Left)
                                ]),
                            }.SetTooltip("")
                            .SetDock(Dock.Top)
                            .AddItems
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
                                            Text = UnityHubNativeNetApp.Config.language.LanguageName,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetDock(Dock.Left),
                                        s_langaugeComboBox = new ComboBox
                                        {
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.AddItems(ILocalization.AllLocalizations.Select(localization => new ComboBoxItem
                                        {
                                            Content = localization.LanguageName
                                        }).ToArray()).SetSelectedItem(ILocalization.AllLocalizations.IndexOf(UnityHubNativeNetApp.Config.language)).OnSelectionChanged(OnLanguageChanged).SetDock(Dock.Right),
                                    ]).SetTooltip(UnityHubNativeNetApp.Config.language.MakesTheWindowTransparentUsesMicaOnWindowsAndTheDesktopsBlurOnLinuxNeedsRestartToTakeEffect),
                                },
                                new SettingsExpanderItem
                                {
                                    Content = new DockPanel
                                    {
                                        LastChildFill = false
                                    }.AddChildren
                                    ([
                                        new TextBlock
                                        {
                                            Text = UnityHubNativeNetApp.Config.language.TransparentWindow,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetDock(Dock.Left),
                                        new CheckBox
                                        {
                                            IsChecked = UnityHubNativeNetApp.Config.transparent,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.OnCheckChanged(OnTransparencyCheckboxChanged).SetDock(Dock.Right),
                                    ]).SetTooltip(UnityHubNativeNetApp.Config.language.MakesTheWindowTransparentUsesMicaOnWindowsAndTheDesktopsBlurOnLinuxNeedsRestartToTakeEffect),
                                },
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
                                                Text = UnityHubNativeNetApp.Config.language.Acrilyc,
                                                VerticalAlignment = VerticalAlignment.Center,
                                            }.SetTooltip(UnityHubNativeNetApp.Config.language.UseAcrylicBlurOnlyWorksOnWindowsNeedsRestartToTakeEffect).SetDock(Dock.Left),
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
                                            Text = UnityHubNativeNetApp.Config.language.BackgroundBlurIntensity,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetTooltip(UnityHubNativeNetApp.Config.language.ChangesTheIntensityOfTheBackgroundBlur).SetDock(Dock.Left),
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
                                },
                                new SettingsExpanderItem
                                {
                                    Content = new DockPanel
                                    {
                                        LastChildFill = false,
                                    }.AddChildren
                                    ([
                                        new TextBlock
                                        {
                                            Text = UnityHubNativeNetApp.Config.language.ExtendToTitlebar,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetTooltip(UnityHubNativeNetApp.Config.language.ExtendsTheClientAreaToTheTitlebar).SetDock(Dock.Left),
                                        new CheckBox
                                        {
                                            IsChecked = UnityHubNativeNetApp.Config.extendToTitlebar
                                        }.OnCheckChanged(OnExtendToTitlebarCheckChanged).SetDock(Dock.Right)
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
                                        Text = UnityHubNativeNetApp.Config.language.Behaviour,
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
                                            Text = UnityHubNativeNetApp.Config.language.CloseAfterOpeningAProject,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetDock(Dock.Left),
                                        new CheckBox
                                        {
                                            IsChecked = UnityHubNativeNetApp.Config.closeAfterProjectOpen,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.OnCheckChanged(OnCloseAfterOpenProjectCheckboxChanged).SetDock(Dock.Right)
                                    ])
                                }.SetTooltip(UnityHubNativeNetApp.Config.language.IfCheckedTheAppWillCloseAfterOpeningAProject),
                                new SettingsExpanderItem
                                {
                                    Content = new DockPanel
                                    {
                                        LastChildFill = false
                                    }.AddChildren
                                    ([
                                        new DockPanel
                                        {
                                        }.AddChildren
                                        ([
                                            new TextBlock
                                            {
                                                Text = UnityHubNativeNetApp.Config.language.FormatToOpenProjectInTerminal,
                                                VerticalAlignment = VerticalAlignment.Center,
                                                Margin = new(0, 0, 10, 0),
                                            }.SetDock(Dock.Left),
                                            s_openInTerminalFormatText = new TextBox
                                            {
                                                Text = UnityHubNativeNetApp.Config.openInTerminalFormat,
                                                VerticalAlignment = VerticalAlignment.Center,
                                            }.OnTextChanged(OnOpenInTerminalFormatChanged).SetDock(Dock.Right)
                                        ]).SetDock(Dock.Top),
                                        new DockPanel
                                        {
                                            LastChildFill = false
                                        }.AddChildren
                                        ([
                                            new TextBlock
                                            {
                                                Text = UnityHubNativeNetApp.Config.language.CloseAfterOpenInTerminal,
                                                VerticalAlignment = VerticalAlignment.Center,
                                                Margin = new(0, 0, 10, 0),
                                            }.SetDock(Dock.Left),
                                            new CheckBox
                                            {
                                                IsChecked = UnityHubNativeNetApp.Config.closeAfterOpenInTerminal,
                                                VerticalAlignment = VerticalAlignment.Center,
                                            }.OnCheckChanged(OnCloseAfterOpenInTerminalChanged).SetDock(Dock.Right)
                                        ]).SetDock(Dock.Top).SetTooltip(UnityHubNativeNetApp.Config.language.WhetherOrNotToCloseTheAppAfterOpeningProjectInTerminal),
                                    ])
                                }.SetTooltip(UnityHubNativeNetApp.Config.language.DefinesTheProcessFormatOfWhenOpeningAProjectInTerminalPathWillBeReplacedByTheProjectPath),
                                new SettingsExpanderItem
                                {
                                    Content = new DockPanel
                                    {
                                        LastChildFill = false
                                    }.AddChildren
                                    ([
                                        new TextBlock
                                        {
                                            Text = UnityHubNativeNetApp.Config.language.RememberUnityProjectSelection,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.SetDock(Dock.Left),
                                        new CheckBox
                                        {
                                            IsChecked = UnityHubNativeNetApp.Config.saveProjectSelection,
                                            VerticalAlignment = VerticalAlignment.Center,
                                        }.OnCheckChanged(OnRememberProjectSelectionChanged).SetDock(Dock.Right)
                                    ])
                                }.SetTooltip(UnityHubNativeNetApp.Config.language.IfCheckedTheLastSelectedUnityProjectWillBeKeptAcrossSessions),
                            ])
                        ])
                    }
                }
            ])
        ])
    ]);

    static void OnLanguageChanged()
    {
        UnityHubNativeNetApp.Config.language = ILocalization.AllLocalizations[s_langaugeComboBox.SelectedIndex];
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
        var old = Instance;
        new MainWindow(null).Show();
        Instance.Position = old.Position;
        Instance.ClientSize = old.ClientSize;
        old.Close();
    }

    static void OnRememberProjectSelectionChanged()
    {
        UnityHubNativeNetApp.Config.saveProjectSelection = !UnityHubNativeNetApp.Config.saveProjectSelection;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
        if (UnityHubNativeNetApp.Config.saveProjectSelection)
            SaveProjectSelectedIndex();
    }

    static void OnCloseAfterOpenInTerminalChanged()
    {
        UnityHubNativeNetApp.Config.closeAfterOpenInTerminal = !UnityHubNativeNetApp.Config.closeAfterOpenInTerminal;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
    }

    static void OnOpenInTerminalFormatChanged()
    {
        UnityHubNativeNetApp.Config.openInTerminalFormat = s_openInTerminalFormatText.Text;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
    }

    static void OnExtendToTitlebarCheckChanged()
    {
        UnityHubNativeNetApp.Config.extendToTitlebar = !UnityHubNativeNetApp.Config.extendToTitlebar;
        Instance.ExtendClientAreaToDecorationsHint = UnityHubNativeNetApp.Config.extendToTitlebar;
        UnityHubNativeNetApp.SaveConfig(UnityHubNativeNetApp.Config);
    }

    public static MenuItem[] CreateProjectMenuItems(Func<UnityProject> unityProjectGetter)
    {
        return
        [
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_Open,
                InputGesture = new(Key.Enter),
            }.OnLayoutUpdate((item) => item.IsEnabled = unityProjectGetter()?.unity.HasValue == true)
            .OnClick(OpenSelectedProject),
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_OpenWith,
                HotKey = new(Key.Enter, KeyModifiers.Alt),
                InputGesture = new(Key.Enter, KeyModifiers.Alt),
            }.OnClick(OpenSelectedProjectWith),
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_OpenInTerminal,
                HotKey = new(Key.Enter, KeyModifiers.Alt | KeyModifiers.Shift),
                InputGesture = new(Key.Enter, KeyModifiers.Alt | KeyModifiers.Shift),
            }.OnClick(OpenSelectedProjectInTerminal),
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_RemoveFromList,
                HotKey = new(Key.Delete),
                InputGesture = new(Key.Delete)
            }.OnClick(OnRemoveProjectFromListClicked),
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_RevealInFileExplorer,
                HotKey = new KeyGesture(Key.F, KeyModifiers.Control),
                InputGesture = new KeyGesture(Key.F, KeyModifiers.Control),
            }.OnClick(RevealSelectedProject),
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_MoveUpInList,
                HotKey = new(Key.Up, KeyModifiers.Shift),
                InputGesture = new(Key.Up, KeyModifiers.Shift),
            }.OnLayoutUpdate((item) => item.IsEnabled = unityProjectGetter is not null && UnityHubUtils.UnityProjects.Skip(1).Contains(unityProjectGetter()))
            .OnClick(MoveSelectedProjectUp),
            new MenuItem
            {
                Header = UnityHubNativeNetApp.Config.language.Menu_MoveDownInList,
                HotKey = new(Key.Down, KeyModifiers.Shift),
                InputGesture = new(Key.Down, KeyModifiers.Shift),
            }.OnLayoutUpdate((item) => item.IsEnabled = unityProjectGetter is not null && UnityHubUtils.UnityProjects.SkipLast(1).Contains(unityProjectGetter()))
            .OnClick(MoveSelectedProjectDown),
        ];
    }

    static void OpenSelectedProject()
    {
        if (s_unityProjectsParent.SelectedItem is UnityProjectView view)
            view.OpenProject();
    }

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
        if (!_updatingUnityProjectList && UnityHubNativeNetApp.Config.saveProjectSelection)
            SaveProjectSelectedIndex();
    }

    static void SaveProjectSelectedIndex() => File.WriteAllText(Paths.SelectedProject, GetUnityProjectSelectedIndex().ToString());

    static void LoadProjectSelectedIndex() => s_unityProjectsParent.SelectedIndex = File.Exists(Paths.SelectedProject) && int.TryParse(File.ReadAllText(Paths.SelectedProject), out var result) ? result : 0;

    static async void AddNewUnitySearchPath()
    {
        try
        {
            var paths = await Instance.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = true,
                Title = UnityHubNativeNetApp.Config.language.PickFolderToSearchForUnityInstallations
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
            _ = MessageBoxManager.GetMessageBoxStandard(UnityHubNativeNetApp.Config.language.Error, ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowWindowDialogAsync(Instance);
        }
    }

    static int GetSelectedUnityInstallationSearchPathsIndex() => s_unityInstalltionSearchPathsParent.SelectedIndex;

    static int GetUnityProjectSelectedIndex() => s_unityProjectsParent.SelectedIndex;

    static bool TryGetSelectedProject(out UnityProject unityProject)
    {
        if (s_projectSearchBoxAutoComplete.SelectedItem is UnityProjectView view)
        {
            unityProject = view.unityProject;
            return true;
        }
        var ind = GetUnityProjectSelectedIndex();
        if (ind < 0 || ind >= UnityHubUtils.UnityProjects.Count)
        {
            unityProject = default;
            return false;
        }
        unityProject = UnityHubUtils.UnityProjects[ind];
        return true;
    }

    static async void OnAddExistingProjectClicked()
    {
        try
        {
            var paths = await Instance.StorageProvider.OpenFolderPickerAsync(new()
            {
                AllowMultiple = true,
                Title = UnityHubNativeNetApp.Config.language.SelectTheFoldersContainingTheUnityProject
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
                    _ = MessageBoxManager.GetMessageBoxStandard(string.Format(UnityHubNativeNetApp.Config.language.ProjectHasAlreadyBeenAdded, folder), UnityHubNativeNetApp.Config.language.CannotAddProject, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info).ShowWindowDialogAsync(Instance);
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
            _ = MessageBoxManager.GetMessageBoxStandard(UnityHubNativeNetApp.Config.language.Error, ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowWindowDialogAsync(Instance);
            Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
        }
    }

    static void OnCreateNewProjectClicked() => new CreateNewProjectDialogue().ShowDialog(Instance);

    static void RevealSelectedProject()
    {
        if (TryGetSelectedProject(out var unityProject))
            OsUtils.OpenExplorer(unityProject.path);
    }

    static void OnAboutClicked(MenuItem item, RoutedEventArgs args) => new AboutDialogue().ShowDialog(Instance);

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
}

