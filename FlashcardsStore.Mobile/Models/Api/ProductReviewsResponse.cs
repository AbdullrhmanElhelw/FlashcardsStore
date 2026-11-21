namespace FlashcardsStore.Mobile.Models.Api;

public class ProductReviewsResponse
{
    public List<ProductReview> Reviews { get; set; } = new();
    public ProductReviewSummary Summary { get; set; } = new();
}
