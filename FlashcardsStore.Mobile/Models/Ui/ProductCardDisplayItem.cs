using CommunityToolkit.Mvvm.ComponentModel;
using FlashcardsStore.Mobile.Models.Api;

namespace FlashcardsStore.Mobile.Models.Ui;

public sealed partial class ProductCardDisplayItem : ObservableObject
{
    public ProductCard Source { get; }
    public string FullImageUrl { get; }

    public int Id => Source.Id;
    public string Slug => Source.Slug;
    public string NameAr => Source.NameAr;
    public string Name => Source.Name;
    public string CategoryNameAr => Source.CategoryNameAr;
    public string CategoryName => Source.CategoryName;
    public string GradeLevel => Source.GradeLevel;
    public int CardCount => Source.CardCount;
    public decimal Price => Source.Price;
    public bool InStock => Source.InStock && Source.AvailableStockQuantity > 0;
    public bool HasTeacherVoice => Source.HasTeacherVoice;

    [ObservableProperty]
    private bool _isRtl = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FavoriteIcon))]
    private bool _isFavorite;

    public string FavoriteIcon => IsFavorite ? "♥" : "♡";

    public string DisplayName => IsRtl ? NameAr : (string.IsNullOrWhiteSpace(Name) ? NameAr : Name);
    public string DisplayCategory => IsRtl ? CategoryNameAr : (string.IsNullOrWhiteSpace(CategoryName) ? CategoryNameAr : CategoryName);
    public string DisplayPrice => IsRtl ? $"{Price:0.##} ج.م" : $"{Price:0.##} EGP";
    public string DisplayCardCount => IsRtl ? $"{CardCount} بطاقة" : $"{CardCount} Cards";
    public string DisplayStockText => InStock
        ? (IsRtl ? $"متاح ({Source.AvailableStockQuantity})" : $"In Stock ({Source.AvailableStockQuantity})")
        : (IsRtl ? "نفدت الكمية" : "Out of Stock");

    partial void OnIsRtlChanged(bool value)
    {
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(DisplayCategory));
        OnPropertyChanged(nameof(DisplayPrice));
        OnPropertyChanged(nameof(DisplayCardCount));
        OnPropertyChanged(nameof(DisplayStockText));
    }

    public ProductCardDisplayItem(ProductCard source, string fullImageUrl, bool isFavorite, bool isRtl = true)
    {
        Source = source;
        FullImageUrl = fullImageUrl;
        _isFavorite = isFavorite;
        _isRtl = isRtl;
    }
}
