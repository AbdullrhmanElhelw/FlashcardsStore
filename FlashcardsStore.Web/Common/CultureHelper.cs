using System.Globalization;

namespace FlashcardsStore.Web.Common;

/// <summary>
/// Static helper methods for culture and RTL detection in views and controllers.
/// </summary>
public static class CultureHelper
{
    public const string DefaultCulture = "ar-EG";
    public const string EnglishCulture = "en-US";

    public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    public static string CurrentLanguageCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public static string Direction => IsRtl ? "rtl" : "ltr";

    public static string GetLocalized(string arText, string enText)
    {
        return IsRtl ? (string.IsNullOrWhiteSpace(arText) ? enText : arText)
                     : (string.IsNullOrWhiteSpace(enText) ? arText : enText);
    }
}
