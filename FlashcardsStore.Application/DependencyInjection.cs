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
        services.AddSingleton<IWhatsAppLinkBuilder, WhatsAppLinkBuilder>();
        services.AddSingleton<IWalletMessageGenerator, WalletMessageGenerator>();

        return services;
    }
}
