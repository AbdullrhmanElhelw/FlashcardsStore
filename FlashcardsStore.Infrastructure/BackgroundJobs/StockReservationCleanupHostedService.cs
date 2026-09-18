using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Infrastructure.BackgroundJobs;

public class StockReservationCleanupHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockReservationCleanupHostedService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

    public StockReservationCleanupHostedService(
        IServiceProvider serviceProvider,
        ILogger<StockReservationCleanupHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StockReservationCleanupHostedService started. Running every {Minutes} minutes.", Interval.TotalMinutes);

        // Initial brief wait after startup before first check
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                var releasedCount = await orderService.ReleaseExpiredStockReservationsAsync(stoppingToken);
                if (releasedCount > 0)
                {
                    _logger.LogInformation("Released expired stock reservations for {Count} order(s).", releasedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while releasing expired stock reservations.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("StockReservationCleanupHostedService stopped.");
    }
}
