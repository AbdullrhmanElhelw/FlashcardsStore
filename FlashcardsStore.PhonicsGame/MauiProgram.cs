using FlashcardsStore.PhonicsGame.Services;
using Microsoft.Extensions.Logging;

namespace FlashcardsStore.PhonicsGame;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Cairo.ttf", "Cairo");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://card-store.runasp.net/"),
            Timeout = TimeSpan.FromSeconds(20)
        });

        builder.Services.AddSingleton<SpeechService>();
        builder.Services.AddSingleton<ActivationService>();
        builder.Services.AddSingleton<GameProgressService>();

        return builder.Build();
    }
}
