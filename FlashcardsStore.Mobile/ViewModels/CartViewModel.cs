using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Cart;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class CartViewModel : BaseViewModel
{
    private readonly ICartStore _cart;
    private readonly IMobileApiClient _api;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _hasItems;

    [ObservableProperty]
    private decimal _subtotal;

    [ObservableProperty]
    private int _itemCount;

    public bool IsEmpty => !HasItems;
    public ObservableCollection<CartEntry> Items { get; } = new();

    public CartViewModel(
        ICartStore cart,
        IMobileApiClient api,
        INavigationService navigation,
        IDialogService dialogs)
    {
        _cart = cart;
        _api = api;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "السلة";
    }

    public string ResolveImageUrl(string? url) => _api.GetImageUrl(url);

    public void Refresh()
    {
        Items.Clear();
        foreach (var entry in _cart.Items)
        {
            Items.Add(new CartEntry
            {
                ProductId = entry.ProductId,
                NameAr = entry.NameAr,
                Name = entry.Name,
                ImageUrl = _api.GetImageUrl(entry.ImageUrl),
                Price = entry.Price,
                Quantity = entry.Quantity,
                AvailableStock = entry.AvailableStock
            });
        }
        HasItems = Items.Count > 0;
        Subtotal = _cart.Subtotal;
        ItemCount = _cart.ItemCount;
    }

    [RelayCommand]
    private void IncreaseQuantity(CartEntry? entry)
    {
        if (entry is null) return;
        _cart.UpdateQuantity(entry.ProductId, entry.Quantity + 1);
        Refresh();
    }

    [RelayCommand]
    private void DecreaseQuantity(CartEntry? entry)
    {
        if (entry is null) return;
        _cart.UpdateQuantity(entry.ProductId, entry.Quantity - 1);
        Refresh();
    }

    [RelayCommand]
    private void RemoveItem(CartEntry? entry)
    {
        if (entry is null) return;
        _cart.Remove(entry.ProductId);
        Refresh();
    }

    [RelayCommand]
    private async Task ClearCartAsync()
    {
        if (await _dialogs.ShowConfirmAsync("السلة", "هل تريدين إزالة جميع المنتجات من السلة؟"))
        {
            _cart.Clear();
            Refresh();
        }
    }

    [RelayCommand]
    private Task ProceedToCheckoutAsync() => _navigation.PushCheckoutAsync();

    [RelayCommand]
    private void BrowseCatalog() => _navigation.SwitchToHomeTab();
}
