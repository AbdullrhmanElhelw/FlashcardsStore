using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Web.Api.Contracts.Auth;

/// <summary>Request payload for changing the signed-in mobile customer's password.</summary>
public sealed class MobileChangePasswordRequest
{
    [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [MinLength(6, ErrorMessage = "يجب أن تكون كلمة المرور الجديدة 6 أحرف على الأقل")]
    [MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}
