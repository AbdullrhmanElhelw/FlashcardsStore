using FlashcardsStore.PhonicsGame.Services;
using FlashcardsStore.PhonicsGame.Services.Abstractions;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.DependencyInjection;

/// <summary>Centralizes all application service and MVVM ViewModel registrations.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhonicsGameServices(this IServiceCollection services)
    {
        services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://card-store.runasp.net/"),
            Timeout = TimeSpan.FromSeconds(20)
        });

        // Core Services
        services.AddSingleton<IJsBridgeService, JsBridgeService>();
        services.AddSingleton<ICurriculumRepository, JsonCurriculumRepository>();
        services.AddSingleton<IOtaContentSyncService, OtaContentSyncService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<ISpeechService, SpeechService>();
        services.AddSingleton<ISpeechRecognitionService, SpeechRecognitionService>();
        services.AddSingleton<IChildProfileService, ChildProfileService>();
        services.AddSingleton<IActivationService, ActivationService>();
        services.AddSingleton<IGameProgressService, GameProgressService>();

        // Focused MVVM ViewModels
        services.AddSingleton<ModalsViewModel>();
        services.AddSingleton<AppShellViewModel>();
        services.AddSingleton<OnboardingViewModel>();
        services.AddSingleton<SoundboardViewModel>();
        services.AddSingleton<DictionaryViewModel>();
        services.AddSingleton<ShopViewModel>();
        services.AddSingleton<StoryPlayerViewModel>();
        services.AddSingleton<GameArenaViewModel>();
        services.AddSingleton<ParentReportViewModel>();
        services.AddSingleton<HomeViewModel>();

        return services;
    }
}
