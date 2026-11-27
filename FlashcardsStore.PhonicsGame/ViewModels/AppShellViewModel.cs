using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;
using Microsoft.JSInterop;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Coordinates root screen navigation, language switching, OTA sync, and Android Back button.</summary>
public sealed partial class AppShellViewModel : BaseViewModel
{
    private const string KeyOnboardingDone = "phonics_onboarding_done";
    private readonly ILocalizationService _loc;
    private readonly IJsBridgeService _js;
    private readonly IOtaContentSyncService _otaSync;
    private readonly IGameProgressService _progress;
    private readonly IActivationService _activation;
    private readonly ModalsViewModel _modals;

    [ObservableProperty]
    private AppScreen _currentScreen = AppScreen.Splash;

    public AppShellViewModel(
        ILocalizationService loc,
        IJsBridgeService js,
        IOtaContentSyncService otaSync,
        IGameProgressService progress,
        IActivationService activation,
        ModalsViewModel modals)
    {
        _loc = loc;
        _js = js;
        _otaSync = otaSync;
        _progress = progress;
        _activation = activation;
        _modals = modals;
        _loc.Changed += NotifyAll;
        _activation.ActivationChanged += NotifyAll;
        _modals.PropertyChanged += (_, _) => NotifyAll();
    }

    public int AvailableStars => _progress.AvailableStars;
    public bool IsActivated => _activation.IsActivated;
    public bool ShowBottomNav => CurrentScreen is AppScreen.Home or AppScreen.GamesHub or AppScreen.Progress or AppScreen.Soundboard or AppScreen.Dictionary or AppScreen.ParentReport;

    public async Task InitializeAsync(IJSRuntime js)
    {
        _js.Attach(js);
        await SyncDocumentDirectionAsync();
        _ = _otaSync.SyncIfNeededAsync();
        _ = _activation.VerifyActivationAsync();

        if (CurrentScreen == AppScreen.Splash)
        {
            await Task.Delay(1500);
            var seenOnboarding = Preferences.Default.Get(KeyOnboardingDone, false);
            CurrentScreen = seenOnboarding ? AppScreen.Home : AppScreen.Onboarding;
        }
    }

    public void NavigateTo(AppScreen screen) => CurrentScreen = screen;

    [RelayCommand]
    public void GoHome() => CurrentScreen = AppScreen.Home;

    [RelayCommand]
    public async Task ToggleLanguageAsync()
    {
        _loc.ToggleLanguage();
        await SyncDocumentDirectionAsync();
        NotifyAll();
    }

    public async Task SetLanguageAsync(string lang)
    {
        _loc.SetLanguage(lang);
        await SyncDocumentDirectionAsync();
        NotifyAll();
    }

    public bool HandleHardwareBackButton()
    {
        if (_modals.ShowActivationModal)
        {
            _modals.CloseActivationModal();
            return true;
        }

        if (_modals.ShowProfileModal)
        {
            _modals.CloseProfileModal();
            return true;
        }

        if (CurrentScreen is AppScreen.Playing or AppScreen.StoryPlayer or AppScreen.Soundboard or AppScreen.Shop or AppScreen.Dictionary)
        {
            CurrentScreen = AppScreen.Home;
            return true;
        }

        return false;
    }

    public void RefreshHeader() => NotifyAll();

    private Task SyncDocumentDirectionAsync()
        => _js.InvokeAsync("phonicsDom.setLocale", _loc.CurrentLang, _loc.Direction);
}
