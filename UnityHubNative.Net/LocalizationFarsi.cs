using UnityHubNative.Net;

class LocalizationFarsi : ILocalization
{
    public string LanguageName => "فارسی";
    public string LanguageCode => "fa";
    public string TitleBar => "یونیتی هاب دات نت";
    public string Menu_File => "فایل";
    public string Menu_CreateNewProject => "ایجاد پروژه جدید";
    public string Menu_AddExistingProject => "افزودن پروژه موجود";
    public string Menu_ReloadData => "بارگذاری دوباره داده ها";
    public string Menu_Project => "پروژه";
    public string Menu_Window => "پنجره";
    public string Menu_CloseWindow => "بستن پنجره";
    public string Menu_About => "درباره یونیتی هاب دات نت";
    public string Tab_Projects => "پروژه ها";
    public string Tab_UnityVersions => "نسخه های یونیتی";
    public string Tab_Options => "تنظیمات";
    public string SearchProjects => "جستجوی پروژه ها";
    public string InstallSearchPaths => "مسیرهای جستجوی نصب";
    public string AddLocation => "افزودن مسیر";
    public string AddsANewUnitySearchPath => "افزودن مسیر جدید جستجوی یونیتی";
    public string RemoveLocation => "حذف مسیر";
    public string RemovesTheSelectedUnitySearchPathItem => "مسیر انتخاب شده حذف می شود";
    public string DetectedInstallations => "نصب های شناسایی شده";
    public string InstallNew => "نصب جدید";
    public string InstallANewUnityEditorVersion => "نصب نسخه جدید یونیتی";
    public string Reload => "بارگذاری دوباره";
    public string ReloadTheList => "فهرست دوباره بارگذاری می شود";
    public string Appearance => "ظاهر";
    public string ControlTheAppearenceOfTheAppCanAffectPerformance => "کنترل ظاهر برنامه (ممکن است بر عملکرد تاثیر بگذارد)";
    public string TransparentWindow => "پنجره شفاف";
    public string Language => "زبان";
    public string MakesTheWindowTransparentUsesMicaOnWindowsAndTheDesktopsBlurOnLinuxNeedsRestartToTakeEffect =>
        "پنجره شفاف می شود. در ویندوز از Mica و در لینوکس از محو دسکتاپ استفاده می کند.\nبرای اعمال نیاز به راه اندازی مجدد است.";
    public string Acrilyc => "آکریلیک";
    public string UseAcrylicBlurOnlyWorksOnWindowsNeedsRestartToTakeEffect =>
        "استفاده از محو آکریلیک (فقط در ویندوز).\nبرای اعمال نیاز به راه اندازی مجدد است.";
    public string BackgroundBlurIntensity => "شدت محو پس زمینه";
    public string ChangesTheIntensityOfTheBackgroundBlur => "شدت محو پس زمینه را تغییر می دهد";
    public string ExtendToTitlebar => "گسترش به نوار عنوان";
    public string ExtendsTheClientAreaToTheTitlebar => "ناحیه کاربری به نوار عنوان گسترش می یابد";
    public string Behaviour => "رفتار";
    public string CloseAfterOpeningAProject => "بستن پس از باز کردن پروژه";
    public string IfCheckedTheAppWillCloseAfterOpeningAProject =>
        "اگر انتخاب شود، برنامه پس از باز کردن پروژه بسته خواهد شد";
    public string FormatToOpenProjectInTerminal => "قالب باز کردن پروژه در ترمینال";
    public string CloseAfterOpenInTerminal => "بستن پس از باز کردن در ترمینال";
    public string WhetherOrNotToCloseTheAppAfterOpeningProjectInTerminal =>
        "بستن یا نبستن برنامه پس از باز کردن پروژه در ترمینال";
    public string DefinesTheProcessFormatOfWhenOpeningAProjectInTerminalPathWillBeReplacedByTheProjectPath =>
        "قالب پردازش باز کردن پروژه در ترمینال را مشخص می کند. {path} با مسیر پروژه جایگزین می شود.";
    public string RememberUnityProjectSelection => "یادآوری پروژه انتخاب شده یونیتی";
    public string IfCheckedTheLastSelectedUnityProjectWillBeKeptAcrossSessions =>
        "اگر انتخاب شود، آخرین پروژه انتخاب شده یونیتی در جلسات بعدی حفظ می شود";
    public string Menu_Open => "باز کردن";
    public string Menu_OpenWith => "باز کردن با";
    public string Menu_OpenInTerminal => "باز کردن در ترمینال";
    public string Menu_RemoveFromList => "حذف از فهرست";
    public string Menu_RevealInFileExplorer => "نمایش در مرورگر فایل";
    public string Menu_MoveUpInList => "انتقال به بالا";
    public string Menu_MoveDownInList => "انتقال به پایین";
    public string PickFolderToSearchForUnityInstallations => "انتخاب پوشه برای جستجوی نصب های یونیتی";
    public string SelectTheFoldersContainingTheUnityProject => "پوشه های حاوی پروژه یونیتی را انتخاب کنید";
    public string ProjectHasAlreadyBeenAdded => "پروژه \"{0}\" قبلا اضافه شده است.";
    public string CannotAddProject => "امکان افزودن پروژه وجود ندارد";
    public string Error => "خطا";
    public string About_1 => "توسعه داده شده با Avalonia و الهام گرفته از UnityHubNative";
    public string Close => "بستن";
    public string CreateANewUnityProject => "ایجاد پروژه جدید یونیتی";
    public string CantCreateNewProjects => "امکان ایجاد پروژه جدید وجود ندارد";
    public string NoUnityInstallationsFound => "هیچ نصب یونیتی یافت نشد";
    public string ProjectName => "نام پروژه";
    public string NewUnityProject => "پروژه جدید یونیتی";
    public string Location => "مسیر";
    public string Choose => "انتخاب";
    public string SelectTemplateForVersion => "انتخاب قالب برای نسخه";
    public string Create => "ایجاد";
    public string CreateTheUnityProjectWithTheSpecifiedAttributes =>
        "پروژه یونیتی را با ویژگی های مشخص شده ایجاد کنید";
    public string Cancel => "لغو";
    public string CancelTheCreationOfANewUnityProject => "لغو ایجاد پروژه جدید یونیتی";
    public string SelectWhereToWriteTheUnityProject => "انتخاب محل ذخیره پروژه یونیتی";
    public string CurrentPlatform => "پلتفرم جاری";
    public string Platform_Windows => "ویندوز";
    public string Platform_MacOs => "مک اواس";
    public string Platform_Linux => "لینوکس";
    public string Platform_iOS => "iOS";
    public string Platform_Android => "اندروید";
    public string Platform_WebGL => "WebGL";
    public string Platform_UWP => "UWP";
    public string OpenWithSpecificEditor => "باز کردن با نسخه خاصی از ویرایشگر";
    public string SelectEditorVerisonToOpen => "انتخاب نسخه ویرایشگر برای باز کردن \"{0}\"";
}
