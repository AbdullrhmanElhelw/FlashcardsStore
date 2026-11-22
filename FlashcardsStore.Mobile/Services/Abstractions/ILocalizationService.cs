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
    string GetLocalized(string? arabic, string? english);
    void ToggleLanguage();
}
