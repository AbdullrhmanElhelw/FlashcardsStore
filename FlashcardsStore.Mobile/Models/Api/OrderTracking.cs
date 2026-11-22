using FlashcardsStore.Mobile.Models.Enums;

namespace FlashcardsStore.Mobile.Models.Api;

public class OrderTracking
{
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal GrandTotal { get; set; }
    public int ItemCount { get; set; }
    public bool RequiresPaymentProof { get; set; }
    public bool HasUploadedProof { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public string StatusDescriptionAr { get; set; } = string.Empty;
}
