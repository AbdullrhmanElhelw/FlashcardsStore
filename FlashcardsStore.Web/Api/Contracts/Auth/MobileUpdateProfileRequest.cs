using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Web.Api.Contracts.Auth;

/// <summary>Request payload for updating the signed-in mobile customer's profile.</summary>
public sealed class MobileUpdateProfileRequest
{
    [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Governorate { get; set; } = string.Empty;
}
