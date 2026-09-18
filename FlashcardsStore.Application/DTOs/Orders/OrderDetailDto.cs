using FlashcardsStore.Domain.Enums;
using FlashcardsStore.Application.DTOs.PaymentProof;

namespace FlashcardsStore.Application.DTOs.Orders;

public class OrderDetailDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal GrandTotal => TotalAmount + ShippingFee;
    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }

    // Customer
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerWhatsApp { get; set; }
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; }

    // Items
    public List<OrderItemDetailDto> Items { get; set; } = new();

    // Payment Proof
    public PaymentProofDto? PaymentProof { get; set; }

    // Generated links
    public string? WhatsAppDirectLink { get; set; }
    public string? WalletPaymentInstructions { get; set; }
}
