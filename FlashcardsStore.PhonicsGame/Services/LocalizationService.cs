using System.Reflection;
using System.Text.Json;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Loads AR/EN translations from embedded JSON resources and persists language preference.</summary>
public sealed class LocalizationService : ILocalizationService
{
    private const string KeyLang = "phonics_lang";
    private readonly Dictionary<string, string> _ar;
    private readonly Dictionary<string, string> _en;
    private string _currentLang;

    public event Action? Changed;

    public LocalizationService()
    {
        _currentLang = Preferences.Default.Get(KeyLang, "ar");
        _ar = LoadJson("ar.json");
        _en = LoadJson("en.json");
    }

    public string CurrentLang => _currentLang;
    public bool IsArabic => _currentLang == "ar";
    public string Direction => IsArabic ? "rtl" : "ltr";

    public void ToggleLanguage() => SetLanguage(IsArabic ? "en" : "ar");

    public void SetLanguage(string lang)
    {
        var normalized = lang == "en" ? "en" : "ar";
        if (_currentLang == normalized) return;
        _currentLang = normalized;
        Preferences.Default.Set(KeyLang, normalized);
        Changed?.Invoke();
    }

    public string T(string key)
    {
        var map = IsArabic ? _ar : _en;
        return map.TryGetValue(key, out var val) ? val : key;
    }

    private static Dictionary<string, string> LoadJson(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream is null)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
