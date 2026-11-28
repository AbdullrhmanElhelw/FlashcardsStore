namespace FlashcardsStore.Application.DTOs.Products;

public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public int CardCount { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int AvailableStockQuantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public string BackPreviewText { get; set; } = string.Empty;
    public string? BackImageUrl { get; set; }
    public string BackKeyPoints { get; set; } = string.Empty;
    public string BackKeyPointsEn { get; set; } = string.Empty;
    public string SampleWordsText { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public bool InStock => AvailableStockQuantity > 0;
    public IReadOnlyList<FlashcardsStore.Application.Common.PronunciationItem> SampleWords { get; set; } = new List<FlashcardsStore.Application.Common.PronunciationItem>();
}
