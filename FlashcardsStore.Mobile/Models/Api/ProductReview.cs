namespace FlashcardsStore.Mobile.Models.Api;

public class ProductReview
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsVerifiedBuyer { get; set; }
    public DateTime CreatedAt { get; set; }

    public string StarsText => new('★', Math.Clamp(Rating, 0, 5));
}
