using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Application.Services;

namespace FlashcardsStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentProofService, PaymentProofService>();
        services.AddScoped<IStockReservationService, StockReservationService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IStoreSettingsService, StoreSettingsService>();
        services.AddScoped<IWhatsAppLinkBuilder, WhatsAppLinkBuilder>();
        services.AddScoped<IWalletMessageGenerator, WalletMessageGenerator>();

        return services;
    }
}
