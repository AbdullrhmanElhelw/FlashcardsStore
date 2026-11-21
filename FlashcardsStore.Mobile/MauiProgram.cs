using FlashcardsStore.Mobile.Pages;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;
using FlashcardsStore.Mobile.ViewModels;
using Microsoft.Extensions.Logging;
#if ANDROID
using Android.Content.Res;
using Microsoft.Maui.Handlers;
#endif

namespace FlashcardsStore.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => fonts.AddFont("Cairo.ttf", "Cairo"));

        ConfigurePlatformHandlers();

        // Infrastructure & App Services
        builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(30) });
        builder.Services.AddSingleton<IMobileApiClient, MobileApiClient>();
        builder.Services.AddSingleton<ICartStore, CartStore>();
        builder.Services.AddSingleton<ICustomerSessionService, CustomerSessionService>();
        builder.Services.AddSingleton<IDialogService, MauiDialogService>();
        builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
        builder.Services.AddSingleton<ISpeechService, MauiSpeechService>();

        // ViewModels
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<CatalogViewModel>();
        builder.Services.AddTransient<ProductDetailViewModel>();
        builder.Services.AddTransient<WishlistViewModel>();
        builder.Services.AddTransient<CartViewModel>();
        builder.Services.AddTransient<CheckoutViewModel>();
        builder.Services.AddTransient<PaymentViewModel>();
        builder.Services.AddTransient<OrderTrackViewModel>();

        // Pages & Shell
        builder.Services.AddTransient<BrandedSplashPage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<StoreShell>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<CatalogPage>();
        builder.Services.AddTransient<ProductDetailPage>();
        builder.Services.AddTransient<WishlistPage>();
        builder.Services.AddTransient<CartPage>();
        builder.Services.AddTransient<CheckoutPage>();
        builder.Services.AddTransient<PaymentPage>();
        builder.Services.AddTransient<OrderTrackPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }

    private static void ConfigurePlatformHandlers()
    {
#if ANDROID
        var transparent = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, _) =>
            handler.PlatformView.BackgroundTintList = transparent);
        EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, _) =>
            handler.PlatformView.BackgroundTintList = transparent);
        PickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, _) =>
            handler.PlatformView.BackgroundTintList = transparent);
#endif
    }
}
