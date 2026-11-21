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
    public string GradeLevel => Source.GradeLevel;
    public int CardCount => Source.CardCount;
    public decimal Price => Source.Price;
    public bool InStock => Source.InStock && Source.AvailableStockQuantity > 0;
    public bool HasTeacherVoice => Source.HasTeacherVoice;
    public string StockText => InStock ? $"متاح ({Source.AvailableStockQuantity})" : "نفدت الكمية";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FavoriteIcon))]
    private bool _isFavorite;

    public string FavoriteIcon => IsFavorite ? "♥" : "♡";

    public ProductCardDisplayItem(ProductCard source, string fullImageUrl, bool isFavorite)
    {
        Source = source;
        FullImageUrl = fullImageUrl;
        _isFavorite = isFavorite;
    }
}
