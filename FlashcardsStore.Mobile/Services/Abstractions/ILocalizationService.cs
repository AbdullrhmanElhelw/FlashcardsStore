using System.ComponentModel;

namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface ILocalizationService : INotifyPropertyChanged
{
    bool IsRtl { get; }
    FlowDirection FlowDirection { get; }
    string SwitchButtonLabel { get; }
    string LanguageLabel { get; }
    event EventHandler? LanguageChanged;

    string this[string key] { get; }
    string Get(string key);
    string Format(string key, params object?[] arguments);
    string GetLocalized(string? arabic, string? english);
    string TranslateTargetAudience(string? raw);
    string TranslateGradeLevel(string? raw);
    void ToggleLanguage();
}
