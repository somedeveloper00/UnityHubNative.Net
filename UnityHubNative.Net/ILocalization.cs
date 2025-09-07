namespace UnityHubNative.Net;

public interface ILocalization
{
    public static readonly ILocalization[] AllLocalizations =
    [
        new LocalizationEnglish(),
        new LocalizationFarsi(),
        new LocalizationNihongo(),
        new LocalizationZhongwenJianti(),
        new LocalizationPolski(),
        new LocalizationArabic()
    ];

    public static bool TryGetFromCode(string code, out ILocalization localization)
    {
        for (int i = 0; i < AllLocalizations.Length; i++)
            if (AllLocalizations[i].LanguageCode == code)
            {
                localization = AllLocalizations[i];
                return true;
            }
        localization = null;
        return false;
    }

    abstract string LanguageName { get; }
    abstract string LanguageCode { get; }
    abstract string TitleBar { get; }
    abstract string Menu_File { get; }
    abstract string Menu_CreateNewProject { get; }
    abstract string Menu_AddExistingProject { get; }
    abstract string Menu_ReloadData { get; }
    abstract string Menu_Project { get; }
    abstract string Menu_Window { get; }
    abstract string Menu_CloseWindow { get; }
    abstract string Menu_About { get; }
    abstract string Tab_Projects { get; }
    abstract string Tab_UnityVersions { get; }
    abstract string Tab_Options { get; }
    abstract string SearchProjects { get; }
    abstract string InstallSearchPaths { get; }
    abstract string AddLocation { get; }
    abstract string AddsANewUnitySearchPath { get; }
    abstract string RemoveLocation { get; }
    abstract string RemovesTheSelectedUnitySearchPathItem { get; }
    abstract string DetectedInstallations { get; }
    abstract string InstallNew { get; }
    abstract string InstallANewUnityEditorVersion { get; }
    abstract string Reload { get; }
    abstract string ReloadTheList { get; }
    abstract string Appearance { get; }
    abstract string ControlTheAppearenceOfTheAppCanAffectPerformance { get; }
    abstract string TransparentWindow { get; }
    abstract string Language { get; }
    abstract string MakesTheWindowTransparentUsesMicaOnWindowsAndTheDesktopsBlurOnLinuxNeedsRestartToTakeEffect { get; }
    abstract string Acrilyc { get; }
    abstract string UseAcrylicBlurOnlyWorksOnWindowsNeedsRestartToTakeEffect { get; }
    abstract string BackgroundBlurIntensity { get; }
    abstract string ChangesTheIntensityOfTheBackgroundBlur { get; }
    abstract string ExtendToTitlebar { get; }
    abstract string ExtendsTheClientAreaToTheTitlebar { get; }
    abstract string Behaviour { get; }
    abstract string CloseAfterOpeningAProject { get; }
    abstract string IfCheckedTheAppWillCloseAfterOpeningAProject { get; }
    abstract string FormatToOpenProjectInTerminal { get; }
    abstract string CloseAfterOpenInTerminal { get; }
    abstract string WhetherOrNotToCloseTheAppAfterOpeningProjectInTerminal { get; }
    abstract string DefinesTheProcessFormatOfWhenOpeningAProjectInTerminalPathWillBeReplacedByTheProjectPath { get; }
    abstract string RememberUnityProjectSelection { get; }
    abstract string IfCheckedTheLastSelectedUnityProjectWillBeKeptAcrossSessions { get; }
    abstract string Menu_Open { get; }
    abstract string Menu_OpenWith { get; }
    abstract string Menu_OpenInTerminal { get; }
    abstract string Menu_RemoveFromList { get; }
    abstract string Menu_RevealInFileExplorer { get; }
    abstract string Menu_MoveUpInList { get; }
    abstract string Menu_MoveDownInList { get; }
    abstract string PickFolderToSearchForUnityInstallations { get; }
    abstract string SelectTheFoldersContainingTheUnityProject { get; }
    abstract string ProjectHasAlreadyBeenAdded { get; }
    abstract string CannotAddProject { get; }
    abstract string Error { get; }
    abstract string About_1 { get; }
    abstract string Close { get; }
    abstract string CreateANewUnityProject { get; }
    abstract string CantCreateNewProjects { get; }
    abstract string NoUnityInstallationsFound { get; }
    abstract string ProjectName { get; }
    abstract string NewUnityProject { get; }
    abstract string Location { get; }
    abstract string Choose { get; }
    abstract string SelectTemplateForVersion { get; }
    abstract string Create { get; }
    abstract string CreateTheUnityProjectWithTheSpecifiedAttributes { get; }
    abstract string Cancel { get; }
    abstract string CancelTheCreationOfANewUnityProject { get; }
    abstract string SelectWhereToWriteTheUnityProject { get; }
    abstract string CurrentPlatform { get; }
    abstract string Platform_Windows { get; }
    abstract string Platform_MacOs { get; }
    abstract string Platform_Linux { get; }
    abstract string Platform_iOS { get; }
    abstract string Platform_Android { get; }
    abstract string Platform_WebGL { get; }
    abstract string Platform_UWP { get; }
    abstract string OpenWithSpecificEditor { get; }
    abstract string SelectEditorVerisonToOpen { get; }
}