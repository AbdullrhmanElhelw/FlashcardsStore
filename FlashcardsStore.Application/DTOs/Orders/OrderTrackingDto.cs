using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.DTOs.Orders;

public class OrderTrackingDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal GrandTotal { get; set; }
    public int ItemCount { get; set; }
    public bool RequiresPaymentProof => PaymentMethod == PaymentMethod.WalletTransfer && (Status == OrderStatus.Draft || Status == OrderStatus.PendingVerification);
    public bool HasUploadedProof { get; set; }
    public string StatusDescriptionAr { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
}
