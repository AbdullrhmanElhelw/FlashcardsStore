using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // e.g. ولي أمر, معلمة لغة إنجليزية, حضانة, مدرسة
    public int Rating { get; set; } = 5; // 1 to 5
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = true;
    public bool IsVerifiedBuyer { get; set; } = true;
}
