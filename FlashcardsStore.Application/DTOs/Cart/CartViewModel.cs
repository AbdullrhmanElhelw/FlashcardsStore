namespace FlashcardsStore.Application.DTOs.Cart;

public class CartViewModel
{
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalItems => Items.Sum(i => i.Quantity);
    public decimal SubTotal => Items.Sum(i => i.SubTotal);
    public decimal ShippingFee { get; set; } = 50.00m; // Standard Cairo/Giza default
    public decimal GrandTotal => SubTotal + (Items.Count > 0 ? ShippingFee : 0m);
    public bool IsEmpty => Items.Count == 0;
}
