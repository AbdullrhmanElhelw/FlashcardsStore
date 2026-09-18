using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.DTOs.Orders;

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; }
    public decimal GrandTotal { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool HasPaymentProof { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalItems { get; set; }
}
