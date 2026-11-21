using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Models.Ui;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class CatalogViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICartStore _cart;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    private int _currentPage = 1;
    private int? _selectedCategoryId;
    private string? _selectedGrade;
    private bool _hasNextPage;
    private bool _initialized;

    [ObservableProperty] private string _announcementText = string.Empty;
    [ObservableProperty] private bool _hasAnnouncement;
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private SortOptionItem _selectedSortOption;
    [ObservableProperty] private string _resultSummary = string.Empty;
    [ObservableProperty] private string _pageLabel = "صفحة 1 / 1";
    [ObservableProperty] private bool _canGoPrevious;
    [ObservableProperty] private bool _canGoNext;
    [ObservableProperty] private int _cartBadgeCount;
    [ObservableProperty] private bool _hasCartItems;
    [ObservableProperty] private bool _hasFeatured;
    [ObservableProperty] private bool _isRefreshing;

    public ObservableCollection<CategoryChipItem> Categories { get; } = new();
    public ObservableCollection<GradeChipItem> Grades { get; } = new();
    public ObservableCollection<ProductCardDisplayItem> FeaturedProducts { get; } = new();
    public ObservableCollection<ProductCardDisplayItem> Products { get; } = new();

    public IReadOnlyList<SortOptionItem> SortOptions { get; } = new[]
    {
        new SortOptionItem { Key = string.Empty, Title = "الأحدث" },
        new SortOptionItem { Key = "price_asc", Title = "السعر الأقل" },
        new SortOptionItem { Key = "price_desc", Title = "السعر الأعلى" },
        new SortOptionItem { Key = "name", Title = "الاسم" }
    };

    public CatalogViewModel(
        IMobileApiClient api,
        ICartStore cart,
        INavigationService navigation,
        IDialogService dialogs)
    {
        _api = api;
        _cart = cart;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "الرئيسية";
        _selectedSortOption = SortOptions[0];

        SeedGrades();
        _cart.Changed += (_, _) => RefreshCartAndFavoritesState();
        RefreshCartAndFavoritesState();
    }

    partial void OnSelectedSortOptionChanged(SortOptionItem value)
    {
        if (!_initialized) return;
        _currentPage = 1;
        _ = LoadProductsAsync();
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        var warning = _cart.TakeLoadWarning();
        if (!string.IsNullOrWhiteSpace(warning))
        {
            await _dialogs.ShowAlertAsync("تنبيه", warning);
        }

        RefreshCartAndFavoritesState();
        if (_initialized) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var configTask = _api.GetConfigAsync();
            var categoriesTask = _api.GetCategoriesAsync();
            var featuredTask = _api.GetFeaturedAsync();

            await Task.WhenAll(configTask, categoriesTask, featuredTask);

            var config = configTask.Result;
            AnnouncementText = config.AnnouncementTextAr;
            HasAnnouncement = config.AnnouncementBarEnabled && !string.IsNullOrWhiteSpace(config.AnnouncementTextAr);

            BuildCategoryChips(categoriesTask.Result);

            FeaturedProducts.Clear();
            foreach (var p in featuredTask.Result)
            {
                FeaturedProducts.Add(ToDisplayItem(p));
            }
            HasFeatured = FeaturedProducts.Count > 0;

            _initialized = true;
            await LoadProductsAsync();
        }
        catch (MobileApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        _initialized = false;
        await InitializeAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        _currentPage = 1;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SearchQuery = string.Empty;
        _selectedCategoryId = null;
        _selectedGrade = null;
        SelectedSortOption = SortOptions[0];

        foreach (var c in Categories) c.IsSelected = c.Id is null;
        foreach (var g in Grades) g.IsSelected = g.Value is null;

        _currentPage = 1;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task SelectCategoryAsync(CategoryChipItem? chip)
    {
        if (chip is null) return;
        _selectedCategoryId = chip.Id;
        foreach (var c in Categories) c.IsSelected = c == chip;
        _currentPage = 1;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task SelectGradeAsync(GradeChipItem? chip)
    {
        if (chip is null) return;
        _selectedGrade = chip.Value;
        foreach (var g in Grades) g.IsSelected = g == chip;
        _currentPage = 1;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (_currentPage <= 1) return;
        _currentPage--;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (!_hasNextPage) return;
        _currentPage++;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private Task OpenProductAsync(ProductCardDisplayItem? item)
        => item is null ? Task.CompletedTask : _navigation.PushProductDetailAsync(item.Slug);

    [RelayCommand]
    private async Task AddToCartAsync(ProductCardDisplayItem? item)
    {
        if (item is null) return;
        try
        {
            _cart.Add(item.Source);
            RefreshCartAndFavoritesState();
            await _dialogs.ShowToastAsync($"تمت إضافة «{item.NameAr}» إلى السلة");
        }
        catch (InvalidOperationException ex)
        {
            await _dialogs.ShowAlertAsync("تنبيه", ex.Message);
        }
    }

    [RelayCommand]
    private void ToggleFavorite(ProductCardDisplayItem? item)
    {
        if (item is null) return;
        _cart.ToggleFavorite(item.Id);
        RefreshCartAndFavoritesState();
    }

    [RelayCommand]
    private void OpenCart() => _navigation.SwitchToCartTab();

    private async Task LoadProductsAsync()
    {
        ErrorMessage = string.Empty;
        try
        {
            var result = await _api.GetProductsAsync(
                _selectedCategoryId,
                SearchQuery,
                _selectedGrade,
                SelectedSortOption?.Key,
                _currentPage);

            Products.Clear();
            foreach (var p in result.Items)
            {
                Products.Add(ToDisplayItem(p));
            }

            _hasNextPage = result.HasNextPage;
            CanGoPrevious = _currentPage > 1;
            CanGoNext = result.HasNextPage;
            PageLabel = $"صفحة {result.PageIndex} / {Math.Max(1, result.TotalPages)}";
            ResultSummary = result.TotalCount == 0
                ? "لا توجد بطاقات مطابقة للبحث"
                : $"عرض {result.Items.Count} من أصل {result.TotalCount} باقة";
        }
        catch (MobileApiException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private ProductCardDisplayItem ToDisplayItem(ProductCard p)
        => new(p, _api.GetImageUrl(p.ImageUrl), _cart.IsFavorite(p.Id));

    private void RefreshCartAndFavoritesState()
    {
        CartBadgeCount = _cart.ItemCount;
        HasCartItems = CartBadgeCount > 0;
        foreach (var p in FeaturedProducts) p.IsFavorite = _cart.IsFavorite(p.Id);
        foreach (var p in Products) p.IsFavorite = _cart.IsFavorite(p.Id);
    }

    private void BuildCategoryChips(List<Category> apiCategories)
    {
        Categories.Clear();
        Categories.Add(new CategoryChipItem { Id = null, Title = "الكل", IconName = "home_house.png", IsSelected = _selectedCategoryId is null });
        foreach (var c in apiCategories)
        {
            var icon = c.Slug?.ToLowerInvariant() switch
            {
                var s when s is not null && s.Contains("phonics") => "home_phonics.png",
                var s when s is not null && s.Contains("grammar") => "home_grammar.png",
                var s when s is not null && s.Contains("vocab") => "home_vocabulary.png",
                var s when s is not null && s.Contains("sight") => "home_sight_words.png",
                _ => "home_house.png"
            };
            Categories.Add(new CategoryChipItem { Id = c.Id, Title = c.NameAr, IconName = icon, IsSelected = _selectedCategoryId == c.Id });
        }
    }

    private void SeedGrades()
    {
        Grades.Clear();
        var items = new (string? val, string label)[]
        {
            (null, "الكل"),
            ("KG1 - KG2", "KG1 - KG2"),
            ("Primary 1", "أولى ابتدائي"),
            ("Primary 2", "تانية ابتدائي"),
            ("Primary 3", "تالتة ابتدائي"),
            ("Primary 4 - 6", "الصفوف العليا")
        };
        foreach (var (val, label) in items)
        {
            Grades.Add(new GradeChipItem { Value = val, Title = label, IsSelected = val is null });
        }
    }
}
