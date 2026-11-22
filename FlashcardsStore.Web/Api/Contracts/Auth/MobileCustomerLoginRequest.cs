using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Web.Api.Contracts.Auth;

public sealed class MobileCustomerLoginRequest
{
    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Password { get; set; } = string.Empty;
}
