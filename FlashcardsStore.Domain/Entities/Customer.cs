using FlashcardsStore.Domain.Common;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? WhatsAppNumber { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; } = CustomerType.Parent;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
