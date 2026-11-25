using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Ui;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class OnboardingViewModel : BaseViewModel
{
    public const string SeenOnboardingKey = "has_seen_onboarding_v1";

    private readonly INavigationService _navigation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLastSlide))]
    [NotifyPropertyChangedFor(nameof(NextButtonText))]
    private int _currentPosition;

    public ObservableCollection<OnboardingSlide> Slides { get; } = new();

    public bool IsLastSlide => CurrentPosition >= Slides.Count - 1;
    public string NextButtonText => Loc[IsLastSlide ? "OnboardingStart" : "OnboardingNext"];

    public OnboardingViewModel(INavigationService navigation, ILocalizationService loc) : base(loc)
    {
        _navigation = navigation;
        Title = Loc["StartOnboarding"];
        RefreshSlides();
        loc.LanguageChanged += (_, _) =>
        {
            Title = Loc["StartOnboarding"];
            RefreshSlides();
            OnPropertyChanged(nameof(NextButtonText));
        };
    }

    private void RefreshSlides()
    {
        var position = CurrentPosition;
        Slides.Clear();
        Slides.Add(new OnboardingSlide
        {
            Title = Loc["OnboardingDiscoverTitle"],
            Description = Loc["OnboardingDiscoverDescription"],
            ImageName = "onboarding_discover.png"
        });
        Slides.Add(new OnboardingSlide
        {
            Title = Loc["OnboardingChooseTitle"],
            Description = Loc["OnboardingChooseDescription"],
            ImageName = "onboarding_choose.png"
        });
        Slides.Add(new OnboardingSlide
        {
            Title = Loc["OnboardingPracticeTitle"],
            Description = Loc["OnboardingPracticeDescription"],
            ImageName = "onboarding_practice.png"
        });
        CurrentPosition = Math.Min(position, Slides.Count - 1);
    }

    [RelayCommand]
    private void NextOrFinish()
    {
        if (!IsLastSlide)
        {
            CurrentPosition++;
            return;
        }
        CompleteOnboarding();
    }

    [RelayCommand]
    private void Skip() => CompleteOnboarding();

    private void CompleteOnboarding()
    {
        Preferences.Default.Set(SeenOnboardingKey, true);
        _navigation.SetStoreRoot();
    }
}
