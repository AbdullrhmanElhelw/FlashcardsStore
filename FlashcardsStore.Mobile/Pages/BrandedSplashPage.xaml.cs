using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Pages;

public partial class BrandedSplashPage : ContentPage
{
    private readonly ICustomerSessionService _session;
    private readonly INavigationService _navigation;
    private bool _hasStarted;

    public BrandedSplashPage(ICustomerSessionService session, INavigationService navigation)
    {
        InitializeComponent();
        _session = session;
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

        if (_session.IsSignedIn)
        {
            _navigation.SetStoreRoot();
        }
        else
        {
            _navigation.SetLoginRoot();
        }
    }
}
