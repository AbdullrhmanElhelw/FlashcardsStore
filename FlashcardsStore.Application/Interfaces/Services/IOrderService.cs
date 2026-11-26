using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Admin;
using FlashcardsStore.Application.DTOs.Cart;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IOrderService
{
    Task<Result<OrderDetailDto>> CreateOrderAsync(CheckoutDto checkout, List<CartItemDto> items, CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<OrderTrackingDto?> TrackOrderAsync(string orderNumber, string phoneNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderSummaryDto>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderSummaryDto>> GetRecentOrdersAsync(int count = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderSummaryDto>> GetCustomerOrdersAsync(int customerId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ApproveOrderAsync(int orderId, string adminUsername, CancellationToken cancellationToken = default);
    Task<Result<bool>> RejectOrderAsync(int orderId, string adminUsername, string? reason, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string adminUsername, string? notes, CancellationToken cancellationToken = default);
    Task<int> ReleaseExpiredStockReservationsAsync(CancellationToken cancellationToken = default);
    Task<AdminDashboardDto> GetAdminDashboardMetricsAsync(CancellationToken cancellationToken = default);
}
