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
    private readonly ICustomerSessionService _session;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _hasItems;

    [ObservableProperty]
    private decimal _subtotal;

    [ObservableProperty]
    private int _itemCount;

    private decimal _freeShippingThreshold;
    private bool _configLoaded;

    public bool IsEmpty => !HasItems;
    public bool HasFreeShippingBar => HasItems && _freeShippingThreshold > 0;
    public bool IsFreeShippingUnlocked => _freeShippingThreshold > 0 && Subtotal >= _freeShippingThreshold;
    public double FreeShippingProgress => _freeShippingThreshold <= 0
        ? 0
        : Math.Min(1.0, (double)(Subtotal / _freeShippingThreshold));
    public string FreeShippingMessage => _freeShippingThreshold <= 0
        ? string.Empty
        : IsFreeShippingUnlocked
            ? Loc["FreeShippingUnlocked"]
            : Loc.Format("FreeShippingRemaining", Math.Max(0, _freeShippingThreshold - Subtotal));
    public string ItemCountLabel => Loc.Format("CartItemsCount", ItemCount);
    public string SubtotalLabel => Loc.Format("CurrencyAmount", Subtotal);
    public ObservableCollection<CartEntry> Items { get; } = new();

    public CartViewModel(
        ICartStore cart,
        IMobileApiClient api,
        ICustomerSessionService session,
        INavigationService navigation,
        IDialogService dialogs,
        ILocalizationService loc) : base(loc)
    {
        _cart = cart;
        _api = api;
        _session = session;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = Loc["CartTitle"];
        loc.LanguageChanged += (_, _) =>
        {
            Title = Loc["CartTitle"];
            Refresh();
        };
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
                DisplayName = Loc.GetLocalized(entry.NameAr, entry.Name),
                PriceLabel = Loc.Format("CurrencyAmount", entry.Price),
                SubtotalLabel = Loc.Format("CurrencyAmount", entry.Price * entry.Quantity),
                ImageUrl = _api.GetImageUrl(entry.ImageUrl),
                Price = entry.Price,
                Quantity = entry.Quantity,
                AvailableStock = entry.AvailableStock
            });
        }
        HasItems = Items.Count > 0;
        Subtotal = _cart.Subtotal;
        ItemCount = _cart.ItemCount;
        OnPropertyChanged(nameof(ItemCountLabel));
        OnPropertyChanged(nameof(SubtotalLabel));
        OnPropertyChanged(nameof(HasFreeShippingBar));
        OnPropertyChanged(nameof(IsFreeShippingUnlocked));
        OnPropertyChanged(nameof(FreeShippingProgress));
        OnPropertyChanged(nameof(FreeShippingMessage));
        if (!_configLoaded) _ = EnsureConfigLoadedAsync();
    }

    private async Task EnsureConfigLoadedAsync()
    {
        if (_configLoaded) return;
        try
        {
            var cfg = await _api.GetConfigAsync();
            _freeShippingThreshold = cfg.FreeShippingThreshold;
            _configLoaded = true;
            OnPropertyChanged(nameof(HasFreeShippingBar));
            OnPropertyChanged(nameof(IsFreeShippingUnlocked));
            OnPropertyChanged(nameof(FreeShippingProgress));
            OnPropertyChanged(nameof(FreeShippingMessage));
        }
        catch
        {
            // Optional enhancement; ignore transient config errors in cart
        }
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
        if (await _dialogs.ShowConfirmAsync(Loc["ClearCartTitle"], Loc["EmptyCartConfirm"]))
        {
            _cart.Clear();
            Refresh();
        }
    }

    [RelayCommand]
    private async Task ProceedToCheckoutAsync()
    {
        if (_session.IsSignedIn)
        {
            await _navigation.PushCheckoutAsync();
            return;
        }

        await _dialogs.ShowAlertAsync(Loc["LoginBtn"], Loc["LoginRequiredCheckout"]);
        await _navigation.PushLoginForCheckoutAsync();
    }

    [RelayCommand]
    private void BrowseCatalog() => _navigation.SwitchToHomeTab();
}
