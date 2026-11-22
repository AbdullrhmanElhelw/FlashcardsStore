using System.Text.Json;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Models.Cart;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class CartStore : ICartStore
{
    private const string CartKey = "customer_cart_v1";
    private const string WishlistKey = "customer_wishlist_v1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly List<CartEntry> _items;
    private readonly List<int> _wishlist;

    public event EventHandler? Changed;

    public CartStore()
    {
        _items = Load<List<CartEntry>>(CartKey, out var cartWarning) ?? new();
        _wishlist = Load<List<int>>(WishlistKey, out var wishlistWarning) ?? new();
        LoadWarning = string.Join(" ", new[] { cartWarning, wishlistWarning }.Where(w => !string.IsNullOrWhiteSpace(w)));
    }

    public string? LoadWarning { get; private set; }
    public IReadOnlyList<CartEntry> Items => _items;
    public IReadOnlyList<int> Wishlist => _wishlist;
    public int ItemCount => _items.Sum(item => item.Quantity);
    public decimal Subtotal => _items.Sum(item => item.Subtotal);

    public void Add(ProductCard product, int quantity = 1)
    {
        if (quantity < 1) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (!product.InStock || product.AvailableStockQuantity < 1)
            throw new InvalidOperationException("هذا المنتج غير متوفر حالياً");

        var existing = _items.FirstOrDefault(item => item.ProductId == product.Id);
        if (existing == null)
        {
            _items.Add(new CartEntry
            {
                ProductId = product.Id,
                NameAr = product.NameAr,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = Math.Min(quantity, product.AvailableStockQuantity),
                AvailableStock = product.AvailableStockQuantity
            });
        }
        else
        {
            existing.AvailableStock = product.AvailableStockQuantity;
            existing.Quantity = Math.Min(existing.Quantity + quantity, product.AvailableStockQuantity);
        }

        SaveCart();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(entry => entry.ProductId == productId);
        if (item == null) return;
        if (quantity <= 0)
        {
            Remove(productId);
            return;
        }

        item.Quantity = Math.Min(quantity, Math.Max(1, item.AvailableStock));
        SaveCart();
    }

    public void Remove(int productId)
    {
        _items.RemoveAll(item => item.ProductId == productId);
        SaveCart();
    }

    public void Clear()
    {
        _items.Clear();
        SaveCart();
    }

    public bool IsFavorite(int productId) => _wishlist.Contains(productId);

    public string? TakeLoadWarning()
    {
        var warning = LoadWarning;
        LoadWarning = null;
        return warning;
    }

    public void ToggleFavorite(int productId)
    {
        if (!_wishlist.Remove(productId)) _wishlist.Add(productId);
        Preferences.Default.Set(WishlistKey, JsonSerializer.Serialize(_wishlist, JsonOptions));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void SaveCart()
    {
        Preferences.Default.Set(CartKey, JsonSerializer.Serialize(_items, JsonOptions));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private static T? Load<T>(string key, out string? warning)
    {
        warning = null;
        var json = Preferences.Default.Get(key, string.Empty);
        if (string.IsNullOrWhiteSpace(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException)
        {
            Preferences.Default.Remove(key);
            warning = "تعذر استعادة بيانات محفوظة تالفة؛ تم حذفها ويمكنك متابعة استخدام المتجر.";
            return default;
        }
    }
}
