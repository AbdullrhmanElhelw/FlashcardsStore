using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

public class AdminUser : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Administrator";
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
