namespace FlashcardsStore.Application.Interfaces.Services;

public interface IStockReservationService
{
    Task<bool> ReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken = default);
    Task<bool> ReleaseStockAsync(int productId, int quantity, CancellationToken cancellationToken = default);
    Task<bool> CommitStockAsync(int productId, int quantity, CancellationToken cancellationToken = default);
}
