using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    public ILocalizationService Loc { get; }

    protected BaseViewModel(ILocalizationService loc)
    {
        Loc = loc;
        Loc.LanguageChanged += (_, _) => OnPropertyChanged(nameof(Loc));
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private void ToggleLanguage() => Loc.ToggleLanguage();
}
