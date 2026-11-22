using FlashcardsStore.Mobile.Models.Enums;

namespace FlashcardsStore.Mobile.Models.Api;

public class CreateOrderRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? WhatsAppNumber { get; set; }
    public string? Email { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; } = CustomerType.Parent;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WalletTransfer;
    public string? Notes { get; set; }
    public List<OrderLineRequest> Items { get; set; } = new();
}
