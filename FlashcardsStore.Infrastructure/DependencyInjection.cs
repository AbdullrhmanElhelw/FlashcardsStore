using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Infrastructure.BackgroundJobs;
using FlashcardsStore.Infrastructure.Persistence;
using FlashcardsStore.Infrastructure.Persistence.Repositories;
using FlashcardsStore.Infrastructure.Storage;

namespace FlashcardsStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string webRootPath)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=FlashcardsStoreDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<FlashcardsStoreDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories & UnitOfWork
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPaymentProofRepository, PaymentProofRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IStoreSettingsRepository, StoreSettingsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register File Storage
        services.AddSingleton<IFileStorageService>(new LocalFileStorageService(webRootPath));

        // Register Password Hasher (PBKDF2 SHA-256 with 100,000 iterations & salt)
        services.AddSingleton<IPasswordHasher, FlashcardsStore.Infrastructure.Security.PasswordHasher>();

        // Register Background Hosted Service for 24-hour stock cleanup
        services.AddHostedService<StockReservationCleanupHostedService>();

        return services;
    }
}
