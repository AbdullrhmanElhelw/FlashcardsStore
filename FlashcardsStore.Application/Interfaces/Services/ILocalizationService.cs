namespace FlashcardsStore.Application.Interfaces.Services;

/// <summary>
/// Service providing localized UI text strings based on current culture.
/// Follows Single Responsibility Principle for UI string translations.
/// </summary>
public interface ILocalizationService
{
    bool IsRtl { get; }
    string CurrentCultureName { get; }
    string CurrentLanguageCode { get; }

    string Get(string key);
    string Get(string key, params object[] args);
    string GetLocalized(string arText, string enText);
    string TranslateTargetAudience(string? raw);
    string TranslateGradeLevel(string? raw);
}
