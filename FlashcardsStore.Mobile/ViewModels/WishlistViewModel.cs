using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Ui;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class WishlistViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICartStore _cart;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _hasItems;

    [ObservableProperty]
    private bool _isRefreshing;

    public bool IsEmpty => !HasItems && !IsBusy && !HasError;
    public ObservableCollection<ProductCardDisplayItem> Items { get; } = new();

    public WishlistViewModel(
        IMobileApiClient api,
        ICartStore cart,
        INavigationService navigation,
        IDialogService dialogs,
        ILocalizationService loc) : base(loc)
    {
        _api = api;
        _cart = cart;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "المفضلة";
        loc.LanguageChanged += (_, _) =>
        {
            foreach (var item in Items) item.IsRtl = loc.IsRtl;
        };
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        ErrorMessage = string.Empty;
        var ids = _cart.Wishlist;
        if (ids.Count == 0)
        {
            Items.Clear();
            HasItems = false;
            OnPropertyChanged(nameof(IsEmpty));
            return;
        }

        IsBusy = true;
        OnPropertyChanged(nameof(IsEmpty));
        try
        {
            var products = await _api.GetProductsByIdsAsync(ids);
            Items.Clear();
            foreach (var p in products)
            {
                Items.Add(new ProductCardDisplayItem(p, _api.GetImageUrl(p.ImageUrl), true, Loc.IsRtl));
            }
            HasItems = Items.Count > 0;
        }
        catch (MobileApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private Task OpenProductAsync(ProductCardDisplayItem? item)
        => item is null ? Task.CompletedTask : _navigation.PushProductDetailAsync(item.Slug);

    [RelayCommand]
    private async Task RemoveFavoriteAsync(ProductCardDisplayItem? item)
    {
        if (item is null) return;
        _cart.ToggleFavorite(item.Id);
        Items.Remove(item);
        HasItems = Items.Count > 0;
        OnPropertyChanged(nameof(IsEmpty));
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AddToCartAsync(ProductCardDisplayItem? item)
    {
        if (item is null) return;
        try
        {
            _cart.Add(item.Source);
            await _dialogs.ShowToastAsync($"تمت إضافة «{item.NameAr}» إلى السلة");
        }
        catch (InvalidOperationException ex)
        {
            await _dialogs.ShowAlertAsync("تنبيه", ex.Message);
        }
    }
}
