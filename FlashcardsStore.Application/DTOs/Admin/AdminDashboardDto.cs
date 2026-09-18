using FlashcardsStore.Application.DTOs.Orders;

namespace FlashcardsStore.Application.DTOs.Admin;

public class AdminDashboardDto
{
    public int TotalOrders { get; set; }
    public int PendingVerificationOrders { get; set; }
    public int WhatsAppPendingOrders { get; set; }
    public int ApprovedOrders { get; set; }
    public int LowStockProductsCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<OrderSummaryDto> RecentOrders { get; set; } = new();
}
