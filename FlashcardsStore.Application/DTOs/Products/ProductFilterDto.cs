namespace FlashcardsStore.Application.DTOs.Products;

public class ProductFilterDto
{
    public int? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public string? GradeLevel { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } // "price_asc", "price_desc", "name", "newest"
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
