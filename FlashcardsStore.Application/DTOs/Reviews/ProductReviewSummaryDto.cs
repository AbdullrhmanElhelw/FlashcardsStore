namespace FlashcardsStore.Application.DTOs.Reviews;

public class ProductReviewSummaryDto
{
    public int ProductId { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }

    public int FiveStarPercentage => TotalReviews > 0 ? (int)Math.Round((double)FiveStarCount / TotalReviews * 100) : 0;
    public int FourStarPercentage => TotalReviews > 0 ? (int)Math.Round((double)FourStarCount / TotalReviews * 100) : 0;
    public int ThreeStarPercentage => TotalReviews > 0 ? (int)Math.Round((double)ThreeStarCount / TotalReviews * 100) : 0;
    public int TwoStarPercentage => TotalReviews > 0 ? (int)Math.Round((double)TwoStarCount / TotalReviews * 100) : 0;
    public int OneStarPercentage => TotalReviews > 0 ? (int)Math.Round((double)OneStarCount / TotalReviews * 100) : 0;
}
