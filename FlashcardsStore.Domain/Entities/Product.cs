using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

public class Product : BaseEntity
{
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
    public int ReservedStockQuantity { get; set; }
    public int AvailableStockQuantity => Math.Max(0, StockQuantity - ReservedStockQuantity);
    public string ImageUrl { get; set; } = string.Empty;
    public string BackPreviewText { get; set; } = string.Empty;
    public string BackKeyPoints { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
}
