using System.ComponentModel.DataAnnotations;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.DTOs.Orders;

public class CheckoutDto
{
    [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? WhatsAppNumber { get; set; }
    public string? Email { get; set; }

    [Required(ErrorMessage = "عنوان التوصيل بالتفصيل مطلوب")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "المدينة / الحي مطلوب")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "المحافظة مطلوبة")]
    public string Governorate { get; set; } = "القاهرة"; // Cairo default

    public CustomerType CustomerType { get; set; } = CustomerType.Parent;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WalletTransfer;
    public string? Notes { get; set; }
}
