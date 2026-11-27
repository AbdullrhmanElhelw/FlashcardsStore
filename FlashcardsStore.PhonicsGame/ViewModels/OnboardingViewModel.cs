using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages the 5-step Duolingo-style onboarding flow.</summary>
public sealed partial class OnboardingViewModel : BaseViewModel
{
    private const string KeyOnboardingDone = "phonics_onboarding_done";
    private readonly AppShellViewModel _shell;
    private readonly IChildProfileService _profileService;
    private readonly ModalsViewModel _modals;

    [ObservableProperty] private int _slideIndex;
    [ObservableProperty] private string _focus = "words";
    [ObservableProperty] private string _nameInput = string.Empty;
    [ObservableProperty] private string _avatarInput = "lion";
    [ObservableProperty] private int _goalMinutes = 10;

    public OnboardingViewModel(
        AppShellViewModel shell,
        IChildProfileService profileService,
        ModalsViewModel modals)
    {
        _shell = shell;
        _profileService = profileService;
        _modals = modals;
        var profile = _profileService.GetProfile();
        _nameInput = profile.Name;
        _avatarInput = profile.AvatarKey;
        _goalMinutes = profile.DailyGoalMinutes;
    }

    public Action? OnRequestStoriesStart { get; set; }
    public Action? OnRequestSentencesStart { get; set; }

    public Task SetLanguageAsync(string lang) => _shell.SetLanguageAsync(lang);

    [RelayCommand]
    public void SetSlide(int index) => SlideIndex = Math.Clamp(index, 0, 4);

    [RelayCommand]
    public void NextSlide()
    {
        if (SlideIndex < 4)
            SlideIndex++;
        else
            CompleteOnboarding();
    }

    [RelayCommand]
    public void CompleteOnboarding()
    {
        _profileService.SaveProfile(NameInput, AvatarInput, GoalMinutes);
        _modals.RefreshProfile();
        Preferences.Default.Set(KeyOnboardingDone, true);

        if (Focus == "stories" && OnRequestStoriesStart is not null)
        {
            OnRequestStoriesStart.Invoke();
            return;
        }

        if (Focus == "sentences" && OnRequestSentencesStart is not null)
        {
            OnRequestSentencesStart.Invoke();
            return;
        }

        _shell.NavigateTo(AppScreen.Home);
    }
}
