using FlashcardsStore.PhonicsGame.Services;
using FlashcardsStore.PhonicsGame.Services.Abstractions;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.DependencyInjection;

/// <summary>Centralizes all application service and ViewModel registrations.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhonicsGameServices(this IServiceCollection services)
    {
        services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://card-store.runasp.net/"),
            Timeout = TimeSpan.FromSeconds(20)
        });

        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<ISpeechService, SpeechService>();
        services.AddSingleton<IActivationService, ActivationService>();
        services.AddSingleton<IGameProgressService, GameProgressService>();
        services.AddSingleton<PhonicsGameViewModel>();

        return services;
    }
}
