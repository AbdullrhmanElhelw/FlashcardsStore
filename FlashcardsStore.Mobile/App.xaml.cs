using FlashcardsStore.Mobile.Pages;

namespace FlashcardsStore.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new(_services.GetRequiredService<BrandedSplashPage>());
}
