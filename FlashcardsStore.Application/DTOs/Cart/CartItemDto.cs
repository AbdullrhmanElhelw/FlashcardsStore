namespace FlashcardsStore.Application.DTOs.Cart;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string PrimaryImageUrl => (ImageUrl ?? string.Empty)
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .FirstOrDefault() ?? "/images/cards/card-phonics-1.svg";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int AvailableStock { get; set; }
    public decimal SubTotal => UnitPrice * Quantity;
}
