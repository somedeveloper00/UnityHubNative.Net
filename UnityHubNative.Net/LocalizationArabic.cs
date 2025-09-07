using UnityHubNative.Net;

class LocalizationArabic : ILocalization
{
    public string LanguageName => "العربية";
    public string LanguageCode => "ar";
    public string TitleBar => "يونيتي هاب دوت نت";
    public string Menu_File => "ملف";
    public string Menu_CreateNewProject => "إنشاء مشروع جديد";
    public string Menu_AddExistingProject => "إضافة مشروع موجود";
    public string Menu_ReloadData => "إعادة تحميل البيانات";
    public string Menu_Project => "المشروع";
    public string Menu_Window => "النافذة";
    public string Menu_CloseWindow => "إغلاق النافذة";
    public string Menu_About => "حول يونيتي هاب دوت نت";
    public string Tab_Projects => "المشاريع";
    public string Tab_UnityVersions => "إصدارات يونيتي";
    public string Tab_Options => "الإعدادات";
    public string SearchProjects => "بحث في المشاريع";
    public string InstallSearchPaths => "مسارات البحث عن التثبيت";
    public string AddLocation => "إضافة مسار";
    public string AddsANewUnitySearchPath => "إضافة مسار بحث جديد ليونيتي";
    public string RemoveLocation => "إزالة المسار";
    public string RemovesTheSelectedUnitySearchPathItem => "إزالة مسار البحث المحدد";
    public string DetectedInstallations => "التثبيتات المكتشفة";
    public string InstallNew => "تثبيت جديد";
    public string InstallANewUnityEditorVersion => "تثبيت نسخة جديدة من محرر يونيتي";
    public string Reload => "إعادة تحميل";
    public string ReloadTheList => "إعادة تحميل القائمة";
    public string Appearance => "المظهر";
    public string ControlTheAppearenceOfTheAppCanAffectPerformance => "التحكم في مظهر التطبيق (قد يؤثر على الأداء)";
    public string TransparentWindow => "نافذة شفافة";
    public string Language => "اللغة";
    public string MakesTheWindowTransparentUsesMicaOnWindowsAndTheDesktopsBlurOnLinuxNeedsRestartToTakeEffect =>
        "يجعل النافذة شفافة. يستخدم Mica في ويندوز وتشويش سطح المكتب في لينكس.\nيتطلب إعادة تشغيل للتفعيل.";
    public string Acrilyc => "أكريليك";
    public string UseAcrylicBlurOnlyWorksOnWindowsNeedsRestartToTakeEffect =>
        "استخدام التشويش الأكريليك (يعمل فقط على ويندوز).\nيتطلب إعادة تشغيل للتفعيل.";
    public string BackgroundBlurIntensity => "شدة تشويش الخلفية";
    public string ChangesTheIntensityOfTheBackgroundBlur => "يغير شدة تشويش الخلفية";
    public string ExtendToTitlebar => "تمديد إلى شريط العنوان";
    public string ExtendsTheClientAreaToTheTitlebar => "تمديد منطقة العميل إلى شريط العنوان";
    public string Behaviour => "السلوك";
    public string CloseAfterOpeningAProject => "إغلاق بعد فتح المشروع";
    public string IfCheckedTheAppWillCloseAfterOpeningAProject =>
        "إذا تم التحديد، سيتم إغلاق التطبيق بعد فتح المشروع";
    public string FormatToOpenProjectInTerminal => "تنسيق فتح المشروع في الطرفية";
    public string CloseAfterOpenInTerminal => "إغلاق بعد الفتح في الطرفية";
    public string WhetherOrNotToCloseTheAppAfterOpeningProjectInTerminal =>
        "إغلاق أو عدم إغلاق التطبيق بعد فتح المشروع في الطرفية";
    public string DefinesTheProcessFormatOfWhenOpeningAProjectInTerminalPathWillBeReplacedByTheProjectPath =>
        "يحدد تنسيق العملية عند فتح المشروع في الطرفية. سيتم استبدال {path} بمسار المشروع.";
    public string RememberUnityProjectSelection => "تذكر المشروع المحدد في يونيتي";
    public string IfCheckedTheLastSelectedUnityProjectWillBeKeptAcrossSessions =>
        "إذا تم التحديد، سيتم الاحتفاظ بآخر مشروع يونيتي محدد بين الجلسات";
    public string Menu_Open => "فتح";
    public string Menu_OpenWith => "فتح باستخدام";
    public string Menu_OpenInTerminal => "فتح في الطرفية";
    public string Menu_RemoveFromList => "إزالة من القائمة";
    public string Menu_RevealInFileExplorer => "إظهار في مستكشف الملفات";
    public string Menu_MoveUpInList => "تحريك للأعلى";
    public string Menu_MoveDownInList => "تحريك للأسفل";
    public string PickFolderToSearchForUnityInstallations => "اختر مجلداً للبحث عن تثبيتات يونيتي";
    public string SelectTheFoldersContainingTheUnityProject => "اختر المجلدات التي تحتوي على مشروع يونيتي";
    public string ProjectHasAlreadyBeenAdded => "المشروع \"{0}\" تمت إضافته مسبقاً.";
    public string CannotAddProject => "لا يمكن إضافة المشروع";
    public string Error => "خطأ";
    public string About_1 => "تم التطوير باستخدام Avalonia وبإلهام من UnityHubNative";
    public string Close => "إغلاق";
    public string CreateANewUnityProject => "إنشاء مشروع يونيتي جديد";
    public string CantCreateNewProjects => "لا يمكن إنشاء مشاريع جديدة";
    public string NoUnityInstallationsFound => "لم يتم العثور على أي تثبيت ليونيتي";
    public string ProjectName => "اسم المشروع";
    public string NewUnityProject => "مشروع يونيتي جديد";
    public string Location => "المسار";
    public string Choose => "اختر";
    public string SelectTemplateForVersion => "اختر القالب للإصدار";
    public string Create => "إنشاء";
    public string CreateTheUnityProjectWithTheSpecifiedAttributes =>
        "إنشاء مشروع يونيتي بالخصائص المحددة";
    public string Cancel => "إلغاء";
    public string CancelTheCreationOfANewUnityProject => "إلغاء إنشاء مشروع يونيتي جديد";
    public string SelectWhereToWriteTheUnityProject => "اختر مكان حفظ مشروع يونيتي";
    public string CurrentPlatform => "المنصة الحالية";
    public string Platform_Windows => "ويندوز";
    public string Platform_MacOs => "ماك أو إس";
    public string Platform_Linux => "لينكس";
    public string Platform_iOS => "iOS";
    public string Platform_Android => "أندرويد";
    public string Platform_WebGL => "WebGL";
    public string Platform_UWP => "UWP";
    public string OpenWithSpecificEditor => "فتح باستخدام محرر محدد";
    public string SelectEditorVerisonToOpen => "اختر إصدار المحرر لفتح \"{0}\"";
}
