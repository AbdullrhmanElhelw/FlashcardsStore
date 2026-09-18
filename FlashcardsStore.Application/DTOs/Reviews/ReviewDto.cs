namespace FlashcardsStore.Application.DTOs.Reviews;

public class ReviewDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public bool IsVerifiedBuyer { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameAr { get; set; } = string.Empty;
}
