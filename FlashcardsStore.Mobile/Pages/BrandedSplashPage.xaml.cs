using FlashcardsStore.Mobile.Services.Abstractions;
using FlashcardsStore.Mobile.ViewModels;

namespace FlashcardsStore.Mobile.Pages;

public partial class BrandedSplashPage : ContentPage
{
    private readonly INavigationService _navigation;
    private bool _hasStarted;

    public BrandedSplashPage(INavigationService navigation)
    {
        InitializeComponent();
        _navigation = navigation;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_hasStarted) return;
        _hasStarted = true;

        BrandMark.Opacity = 0;
        BrandMark.Scale = 0.94;
        await Task.WhenAll(
            BrandMark.FadeToAsync(1, 650, Easing.CubicOut),
            BrandMark.ScaleToAsync(1, 650, Easing.CubicOut));
        await Task.Delay(700);
        await BrandMark.FadeToAsync(0, 220, Easing.CubicIn);

        var hasSeenOnboarding = Preferences.Default.Get(OnboardingViewModel.SeenOnboardingKey, false);
        if (hasSeenOnboarding)
        {
            _navigation.SetStoreRoot();
        }
        else
        {
            _navigation.SetOnboardingRoot();
        }
    }
}
