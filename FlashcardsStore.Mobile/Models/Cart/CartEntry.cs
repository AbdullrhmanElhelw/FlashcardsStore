using CommunityToolkit.Mvvm.ComponentModel;

namespace FlashcardsStore.Mobile.Models.Cart;

public sealed partial class CartEntry : ObservableObject
{
    public int ProductId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PriceLabel { get; set; } = string.Empty;
    public string SubtotalLabel { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Subtotal))]
    [NotifyPropertyChangedFor(nameof(CanIncrease))]
    private int _quantity;

    public int AvailableStock { get; set; }

    public decimal Subtotal => Price * Quantity;
    public bool CanIncrease => Quantity < AvailableStock;
}
