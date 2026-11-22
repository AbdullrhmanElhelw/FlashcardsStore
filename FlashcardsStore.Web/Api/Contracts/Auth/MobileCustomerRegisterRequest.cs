using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Web.Api.Contracts.Auth;

public sealed class MobileCustomerRegisterRequest
{
    [Required, StringLength(200, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 7)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
