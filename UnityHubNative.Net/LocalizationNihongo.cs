using UnityHubNative.Net;

class LocalizationNihongo : ILocalization
{
    public string LanguageName => "日本語";
    public string LanguageCode => "ja";
    public string TitleBar => "ユニティ ハブ ドット ネット";
    public string Menu_File => "ファイル";
    public string Menu_CreateNewProject => "新しいプロジェクトを作成";
    public string Menu_AddExistingProject => "既存のプロジェクトを追加";
    public string Menu_ReloadData => "データを再読み込み";
    public string Menu_Project => "プロジェクト";
    public string Menu_Window => "ウィンドウ";
    public string Menu_CloseWindow => "ウィンドウを閉じる";
    public string Menu_About => "ユニティ ハブ ドット ネット について";
    public string Tab_Projects => "プロジェクト";
    public string Tab_UnityVersions => "Unity バージョン";
    public string Tab_Options => "オプション";
    public string SearchProjects => "プロジェクトを検索";
    public string InstallSearchPaths => "インストール検索パス";
    public string AddLocation => "場所を追加";
    public string AddsANewUnitySearchPath => "新しい Unity 検索パスを追加します";
    public string RemoveLocation => "場所を削除";
    public string RemovesTheSelectedUnitySearchPathItem => "選択した Unity 検索パスを削除します";
    public string DetectedInstallations => "検出されたインストール";
    public string InstallNew => "新規インストール";
    public string InstallANewUnityEditorVersion => "新しい Unity エディターをインストール";
    public string Reload => "再読み込み";
    public string ReloadTheList => "リストを再読み込みします";
    public string Appearance => "外観";
    public string ControlTheAppearenceOfTheAppCanAffectPerformance => "アプリの外観を制御します（パフォーマンスに影響する可能性があります）";
    public string TransparentWindow => "透明ウィンドウ";
    public string Language => "言語";
    public string MakesTheWindowTransparentUsesMicaOnWindowsAndTheDesktopsBlurOnLinuxNeedsRestartToTakeEffect =>
        "ウィンドウを透明にします。Windows では Mica、Linux ではデスクトップのぼかしを使用します。\n再起動が必要です。";
    public string Acrilyc => "アクリル";
    public string UseAcrylicBlurOnlyWorksOnWindowsNeedsRestartToTakeEffect =>
        "アクリルぼかしを使用します（Windows のみ対応）。\n再起動が必要です。";
    public string BackgroundBlurIntensity => "背景ぼかしの強さ";
    public string ChangesTheIntensityOfTheBackgroundBlur => "背景ぼかしの強さを変更します";
    public string ExtendToTitlebar => "タイトルバーまで拡張";
    public string ExtendsTheClientAreaToTheTitlebar => "クライアント領域をタイトルバーまで拡張します";
    public string Behaviour => "動作";
    public string CloseAfterOpeningAProject => "プロジェクトを開いた後に終了";
    public string IfCheckedTheAppWillCloseAfterOpeningAProject =>
        "チェックすると、プロジェクトを開いた後にアプリが終了します";
    public string FormatToOpenProjectInTerminal => "ターミナルでプロジェクトを開く形式";
    public string CloseAfterOpenInTerminal => "ターミナルで開いた後に終了";
    public string WhetherOrNotToCloseTheAppAfterOpeningProjectInTerminal =>
        "プロジェクトをターミナルで開いた後にアプリを終了するかどうか";
    public string DefinesTheProcessFormatOfWhenOpeningAProjectInTerminalPathWillBeReplacedByTheProjectPath =>
        "ターミナルでプロジェクトを開くときのプロセス形式を定義します。{path} はプロジェクトのパスに置き換えられます。";
    public string RememberUnityProjectSelection => "選択した Unity プロジェクトを記憶";
    public string IfCheckedTheLastSelectedUnityProjectWillBeKeptAcrossSessions =>
        "チェックすると、最後に選択した Unity プロジェクトが次回以降も保持されます";
    public string Menu_Open => "開く";
    public string Menu_OpenWith => "アプリで開く";
    public string Menu_OpenInTerminal => "ターミナルで開く";
    public string Menu_RemoveFromList => "リストから削除";
    public string Menu_RevealInFileExplorer => "ファイルエクスプローラーで表示";
    public string Menu_MoveUpInList => "リスト内で上へ移動";
    public string Menu_MoveDownInList => "リスト内で下へ移動";
    public string PickFolderToSearchForUnityInstallations => "Unity のインストールを検索するフォルダーを選択";
    public string SelectTheFoldersContainingTheUnityProject => "Unity プロジェクトを含むフォルダーを選択してください";
    public string ProjectHasAlreadyBeenAdded => "プロジェクト「{0}」はすでに追加されています。";
    public string CannotAddProject => "プロジェクトを追加できません";
    public string Error => "エラー";
    public string About_1 => "Avalonia で開発され、UnityHubNative に着想を得ています";
    public string Close => "閉じる";
    public string CreateANewUnityProject => "新しい Unity プロジェクトを作成";
    public string CantCreateNewProjects => "新しいプロジェクトを作成できません";
    public string NoUnityInstallationsFound => "Unity のインストールが見つかりません";
    public string ProjectName => "プロジェクト名";
    public string NewUnityProject => "新しい Unity プロジェクト";
    public string Location => "場所";
    public string Choose => "選択";
    public string SelectTemplateForVersion => "バージョン用のテンプレートを選択";
    public string Create => "作成";
    public string CreateTheUnityProjectWithTheSpecifiedAttributes =>
        "指定した属性で Unity プロジェクトを作成します";
    public string Cancel => "キャンセル";
    public string CancelTheCreationOfANewUnityProject => "新しい Unity プロジェクトの作成をキャンセル";
    public string SelectWhereToWriteTheUnityProject => "Unity プロジェクトを保存する場所を選択";
    public string CurrentPlatform => "現在のプラットフォーム";
    public string Platform_Windows => "Windows";
    public string Platform_MacOs => "MacOS";
    public string Platform_Linux => "Linux";
    public string Platform_iOS => "iOS";
    public string Platform_Android => "Android";
    public string Platform_WebGL => "WebGL";
    public string Platform_UWP => "UWP";
    public string OpenWithSpecificEditor => "特定のエディターで開く";
    public string SelectEditorVerisonToOpen => "「{0}」を開くエディターのバージョンを選択";
}
