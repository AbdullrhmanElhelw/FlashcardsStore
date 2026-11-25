using System.Globalization;
using System.Text.Json;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Application.Services;

/// <summary>
/// Provides localized dictionary entries loaded from JSON resource files (ar.json / en.json).
/// Follows Single Responsibility Principle for UI string translations.
/// </summary>
public class LocalizationService : ILocalizationService
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> ArTranslations =
        new(() => LoadLanguageFile("ar"));

    private static readonly Lazy<IReadOnlyDictionary<string, string>> EnTranslations =
        new(() => LoadLanguageFile("en"));

    public bool IsRtl
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture;
            return culture.TextInfo.IsRightToLeft;
        }
    }

    public string CurrentCultureName => CultureInfo.CurrentUICulture.Name;

    public string CurrentLanguageCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        var primary = IsRtl ? ArTranslations.Value : EnTranslations.Value;
        if (primary.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
        {
            return value;
        }

        var fallback = IsRtl ? EnTranslations.Value : ArTranslations.Value;
        if (fallback.TryGetValue(key, out var fallbackValue) && !string.IsNullOrEmpty(fallbackValue))
        {
            return fallbackValue;
        }

        return key;
    }

    public string Get(string key, params object[] args)
    {
        var template = Get(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    public string GetLocalized(string arText, string enText)
    {
        if (!IsRtl && !string.IsNullOrWhiteSpace(enText))
        {
            return enText;
        }

        return string.IsNullOrWhiteSpace(arText) ? enText : arText;
    }

    public string TranslateTargetAudience(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var text = raw.Trim();
        if (IsRtl) return text;

        return text
            .Replace("ابتدائي (الصفوف 1-3)", "Primary (Grades 1-3)")
            .Replace("ابتدائي (الصفوف 4-6)", "Primary (Grades 4-6)")
            .Replace("رياض الأطفال (KG)", "Kindergarten (KG)")
            .Replace("تأسيس (KG)", "Foundation (KG)")
            .Replace("إعدادي", "Preparatory")
            .Replace("الأطفال من سن 4 إلى 7 سنوات", "Ages 4 to 7")
            .Replace("سن 6 إلى 10 سنوات", "Ages 6 to 10")
            .Replace("سن 7 إلى 13 سنة", "Ages 7 to 13")
            .Replace("أولياء الأمور", "Parents")
            .Replace("المعلمين والمعلمات", "Teachers")
            .Replace("المعلمين", "Teachers")
            .Replace("الصفوف", "Grades");
    }

    public string TranslateGradeLevel(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var key = raw.Trim().ToLowerInvariant();
        return key switch
        {
            "kg" or "kg1-kg2" or "kg-1-2" => IsRtl ? "رياض الأطفال (KG1 - KG2)" : "Kindergarten (KG1 - KG2)",
            "primary-1" => IsRtl ? "الأول الابتدائي" : "Primary 1",
            "primary-2" => IsRtl ? "الثاني الابتدائي" : "Primary 2",
            "primary-3" => IsRtl ? "الثالث الابتدائي" : "Primary 3",
            "primary-1-3" => IsRtl ? "الصفوف 1 - 3 ابتدائي" : "Primary 1 - 3",
            "primary-4-6" => IsRtl ? "الصفوف 4 - 6 ابتدائي" : "Primary 4 - 6",
            "prep" or "prep-1-3" => IsRtl ? "المرحلة الإعدادية" : "Preparatory",
            "all" or "all-levels" => IsRtl ? "جميع المستويات" : "All Levels",
            _ => raw.Trim()
        };
    }

    private static IReadOnlyDictionary<string, string> LoadLanguageFile(string languageCode)
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Resources",
            "Localization",
            $"{languageCode}.json");

        if (!File.Exists(filePath))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var json = File.ReadAllText(filePath);
        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        return parsed is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(parsed, StringComparer.OrdinalIgnoreCase);
    }
}
