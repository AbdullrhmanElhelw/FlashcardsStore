namespace FlashcardsStore.Mobile.Models.Api;

public class OrderLine
{
    public int ProductId { get; set; }
    public string ProductNameAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
}
