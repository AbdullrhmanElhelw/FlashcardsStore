namespace FlashcardsStore.Mobile.Models.Api;

public class CreateReviewRequest
{
    public int ProductId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
