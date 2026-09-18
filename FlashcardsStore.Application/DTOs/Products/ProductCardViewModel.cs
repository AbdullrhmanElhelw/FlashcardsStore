namespace FlashcardsStore.Application.DTOs.Products;

public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public int CardCount { get; set; }
    public decimal Price { get; set; }
    public int AvailableStockQuantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string BackPreviewText { get; set; } = string.Empty;
    public string BackKeyPoints { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public bool InStock => AvailableStockQuantity > 0;
}
