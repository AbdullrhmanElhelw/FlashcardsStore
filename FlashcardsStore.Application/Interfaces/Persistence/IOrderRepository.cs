using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Order?> GetWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetWithDetailsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetRecentOrdersAsync(int count = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetExpiredPendingVerificationOrdersAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}
