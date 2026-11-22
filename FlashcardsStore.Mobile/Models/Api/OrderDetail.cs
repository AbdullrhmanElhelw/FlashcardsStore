using FlashcardsStore.Mobile.Models.Enums;

namespace FlashcardsStore.Mobile.Models.Api;

public class OrderDetail
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal GrandTotal { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public List<OrderLine> Items { get; set; } = new();
    public string? WhatsAppDirectLink { get; set; }
}
