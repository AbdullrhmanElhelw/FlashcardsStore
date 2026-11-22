using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class ProductDetailViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICartStore _cart;
    private readonly ISpeechService _speech;
    private readonly IAudioPlayerService _audioPlayer;
    private readonly ICustomerSessionService _session;
    private readonly IDialogService _dialogs;

    private string _slug = string.Empty;
    private bool _loaded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasProduct))]
    [NotifyPropertyChangedFor(nameof(StockLabel))]
    [NotifyPropertyChangedFor(nameof(CanAddToCart))]
    [NotifyPropertyChangedFor(nameof(HasSampleWords))]
    [NotifyPropertyChangedFor(nameof(HasTeacherVoice))]
    [NotifyPropertyChangedFor(nameof(HasDescription))]
    private ProductDetail? _product;

    [ObservableProperty]
    private string _fullImageUrl = "logo.png";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FavoriteButtonText))]
    private bool _isFavorite;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SpeedModeLabel))]
    private bool _isKidSlowMode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TeacherVoiceButtonText))]
    private bool _isPlayingTeacherVoice;

    [ObservableProperty]
    private string _reviewSummaryText = "لا توجد تقييمات بعد";

    [ObservableProperty] private string _reviewerName = string.Empty;
    [ObservableProperty] private string _reviewerRole = string.Empty;
    [ObservableProperty] private string _reviewComment = string.Empty;
    [ObservableProperty] private int _selectedRatingIndex;

    public IReadOnlyList<string> RatingOptions { get; } = new[]
    {
        "5 نجوم ★★★★★", "4 نجوم ★★★★", "3 نجوم ★★★", "2 نجمتان ★★", "1 نجمة ★"
    };

    public ObservableCollection<PronunciationItem> SampleWords { get; } = new();
    public ObservableCollection<ProductReview> Reviews { get; } = new();

    public bool HasProduct => Product is not null;
    public bool CanAddToCart => Product is not null && Product.InStock && Product.AvailableStockQuantity > 0;
    public bool HasSampleWords => SampleWords.Count > 0;
    public bool HasTeacherVoice => Product?.HasTeacherVoice == true;
    public bool HasDescription => !string.IsNullOrWhiteSpace(Product?.DescriptionAr);
    public string StockLabel => Product is null
        ? string.Empty
        : (Product.InStock ? $"متوفر — الكمية المتاحة: {Product.AvailableStockQuantity}" : "غير متوفر حالياً");
    public string FavoriteButtonText => IsFavorite ? "في المفضلة ♥" : "أضف للمفضلة ♡";
    public string SpeedModeLabel => IsKidSlowMode ? "بطيء للأطفال (0.75x)" : "سرعة طبيعية (1.0x)";
    public string TeacherVoiceButtonText => IsPlayingTeacherVoice ? "إيقاف" : "تشغيل";

    public ProductDetailViewModel(
        IMobileApiClient api,
        ICartStore cart,
        ISpeechService speech,
        IAudioPlayerService audioPlayer,
        ICustomerSessionService session,
        IDialogService dialogs)
    {
        _api = api;
        _cart = cart;
        _speech = speech;
        _audioPlayer = audioPlayer;
        _session = session;
        _dialogs = dialogs;
        Title = "تفاصيل المنتج";
        ReviewerName = session.CustomerName;
        _audioPlayer.PlaybackStateChanged += (_, playing) => IsPlayingTeacherVoice = playing;
    }

    public void SetSlug(string slug)
    {
        if (_slug == slug && _loaded) return;
        _slug = slug;
        _loaded = false;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (_loaded || string.IsNullOrWhiteSpace(_slug)) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var detail = await _api.GetProductAsync(_slug);
            Product = detail;
            FullImageUrl = string.IsNullOrWhiteSpace(detail.ImageUrl) ? "logo.png" : _api.GetImageUrl(detail.ImageUrl);
            IsFavorite = _cart.IsFavorite(detail.Id);

            SampleWords.Clear();
            foreach (var w in detail.SampleWords)
            {
                SampleWords.Add(w);
            }
            OnPropertyChanged(nameof(HasSampleWords));

            await LoadReviewsInternalAsync(detail.Id);
            _loaded = true;
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
    private void ToggleSpeedMode() => IsKidSlowMode = !IsKidSlowMode;

    [RelayCommand]
    private Task SpeakWordAsync(PronunciationItem? item)
        => item is null ? Task.CompletedTask : _speech.SpeakAsync(item.Word, IsKidSlowMode);

    [RelayCommand]
    private Task SpeakSentenceAsync(PronunciationItem? item)
        => item is null || string.IsNullOrWhiteSpace(item.ExampleSentence)
            ? Task.CompletedTask
            : _speech.SpeakAsync(item.ExampleSentence, IsKidSlowMode);

    [RelayCommand]
    private async Task PlayAllWordsAsync()
    {
        foreach (var item in SampleWords)
        {
            await _speech.SpeakAsync(item.Word, IsKidSlowMode);
            await Task.Delay(500);
        }
    }

    [RelayCommand]
    private async Task PlayTeacherVoiceAsync()
    {
        if (Product is null || string.IsNullOrWhiteSpace(Product.AudioUrl)) return;
        try
        {
            await _audioPlayer.TogglePlayAsync(_api.GetImageUrl(Product.AudioUrl));
        }
        catch (Exception ex)
        {
            await _dialogs.ShowAlertAsync("تنبيه", ex.Message);
        }
    }

    public void StopTeacherVoice() => _audioPlayer.Stop();

    [RelayCommand]
    private async Task AddToCartAsync()
    {
        if (Product is null) return;
        try
        {
            _cart.Add(new ProductCard
            {
                Id = Product.Id,
                Name = Product.Name,
                NameAr = Product.NameAr,
                Slug = Product.Slug,
                Price = Product.Price,
                ImageUrl = Product.ImageUrl,
                AvailableStockQuantity = Product.AvailableStockQuantity,
                InStock = Product.InStock
            });
            await _dialogs.ShowToastAsync($"تمت إضافة «{Product.NameAr}» إلى السلة");
        }
        catch (InvalidOperationException ex)
        {
            await _dialogs.ShowAlertAsync("تنبيه", ex.Message);
        }
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        if (Product is null) return;
        _cart.ToggleFavorite(Product.Id);
        IsFavorite = _cart.IsFavorite(Product.Id);
    }

    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        if (Product is null) return;
        var commentText = ReviewComment.Trim();
        if (string.IsNullOrWhiteSpace(ReviewerName) || string.IsNullOrWhiteSpace(ReviewerRole) || commentText.Length < 5)
        {
            await _dialogs.ShowAlertAsync("تنبيه", "أكمل الاسم والصفة واكتب تعليقاً من 5 أحرف على الأقل.");
            return;
        }

        IsBusy = true;
        try
        {
            await _api.AddReviewAsync(new CreateReviewRequest
            {
                ProductId = Product.Id,
                CustomerName = ReviewerName.Trim(),
                Role = ReviewerRole.Trim(),
                Comment = commentText,
                Rating = Math.Max(1, 5 - SelectedRatingIndex)
            });
            await _dialogs.ShowAlertAsync("شكراً لكِ ✓", "تم إرسال تقييمك للمراجعة وسيظهر بعد الاعتماد.");
            ReviewComment = string.Empty;
            ReviewerRole = string.Empty;
        }
        catch (MobileApiException ex)
        {
            await _dialogs.ShowAlertAsync("خطأ", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadReviewsInternalAsync(int productId)
    {
        try
        {
            var response = await _api.GetReviewsAsync(productId);
            Reviews.Clear();
            foreach (var r in response.Reviews)
            {
                Reviews.Add(r);
            }
            ReviewSummaryText = response.Summary.TotalReviews == 0
                ? "لا توجد تقييمات بعد — كوني أول من يقيّم الباقة"
                : $"⭐ {response.Summary.AverageRating:0.0} من 5  ·  {response.Summary.TotalReviews} تقييم";
        }
        catch (MobileApiException ex)
        {
            ReviewSummaryText = $"تعذر تحميل التقييمات: {ex.Message}";
        }
    }
}
