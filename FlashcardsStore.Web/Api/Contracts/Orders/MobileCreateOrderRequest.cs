using System.ComponentModel.DataAnnotations;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Web.Api.Contracts.Orders;

public sealed class MobileCreateOrderRequest
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(20)]
    public string? WhatsAppNumber { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [Required, StringLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Governorate { get; set; } = string.Empty;

    public CustomerType CustomerType { get; set; } = CustomerType.Parent;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WalletTransfer;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required, MinLength(1), MaxLength(50)]
    public List<MobileOrderItem> Items { get; set; } = new();
}
