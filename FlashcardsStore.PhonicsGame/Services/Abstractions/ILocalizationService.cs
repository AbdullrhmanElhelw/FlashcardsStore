namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface ILocalizationService
{
    string CurrentLang { get; }
    bool IsArabic { get; }
    string Direction { get; }
    event Action? Changed;
    string T(string key);
    void ToggleLanguage();
    void SetLanguage(string lang);
}
