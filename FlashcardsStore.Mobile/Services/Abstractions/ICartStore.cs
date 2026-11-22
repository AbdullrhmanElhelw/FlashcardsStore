using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Models.Cart;

namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface ICartStore
{
    event EventHandler? Changed;
    IReadOnlyList<CartEntry> Items { get; }
    IReadOnlyList<int> Wishlist { get; }
    int ItemCount { get; }
    decimal Subtotal { get; }
    string? TakeLoadWarning();
    void Add(ProductCard product, int quantity = 1);
    void UpdateQuantity(int productId, int quantity);
    void Remove(int productId);
    void Clear();
    bool IsFavorite(int productId);
    void ToggleFavorite(int productId);
}
