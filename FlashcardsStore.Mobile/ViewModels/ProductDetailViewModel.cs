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
    [NotifyPropertyChangedFor(nameof(ImageCountLabel))]
    [NotifyPropertyChangedFor(nameof(HasMultipleImages))]
    private int _selectedImageIndex;

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
    private string _reviewSummaryText = string.Empty;

    private double _averageRating;
    private int _reviewCount;
    private string? _reviewLoadError;

    [ObservableProperty] private string _reviewerName = string.Empty;
    [ObservableProperty] private string _reviewerRole = string.Empty;
    [ObservableProperty] private string _reviewComment = string.Empty;
    [ObservableProperty] private int _selectedRatingIndex;
    [ObservableProperty] private bool _isImagePreviewOpen;
    [ObservableProperty] private string _previewImageUrl = string.Empty;

    public IReadOnlyList<string> RatingOptions => new[]
    {
        Loc["Rating5Stars"], Loc["Rating4Stars"], Loc["Rating3Stars"], Loc["Rating2Stars"], Loc["Rating1Star"]
    };

    public ObservableCollection<PronunciationItem> SampleWords { get; } = new();
    public ObservableCollection<string> KeyPoints { get; } = new();
    public ObservableCollection<ProductReview> Reviews { get; } = new();
    public ObservableCollection<string> ProductImages { get; } = new();

    public bool HasProduct => Product is not null;
    public bool HasMultipleImages => ProductImages.Count > 1;
    public string ImageCountLabel => ProductImages.Count == 0
        ? string.Empty
        : Loc.Format("ImageCountFormat", SelectedImageIndex + 1, ProductImages.Count);
    public bool CanAddToCart => Product is not null && Product.InStock && Product.AvailableStockQuantity > 0;
    public bool HasSampleWords => SampleWords.Count > 0;
    public bool HasKeyPoints => KeyPoints.Count > 0;
    public bool HasTeacherVoice => Product?.HasTeacherVoice == true;
    public bool HasDescription => !string.IsNullOrWhiteSpace(DisplayDescription);
    public string StockLabel => Product is null
        ? string.Empty
        : (Product.InStock ? Loc.Format("AvailableQuantityFormat", Product.AvailableStockQuantity) : Loc["Unavailable"]);
    public string FavoriteButtonText => Loc[IsFavorite ? "SavedFavorite" : "AddFavorite"];
    public string SpeedModeLabel => Loc[IsKidSlowMode ? "KidSlow" : "NormalSpeed"];
    public string TeacherVoiceButtonText => Loc[IsPlayingTeacherVoice ? "Stop" : "Play"];
    public string PriceLabel => Product is null ? string.Empty : Loc.Format("CurrencyAmount", Product.Price);
    public string CardCountLabel => Product is null ? string.Empty : Loc.Format("ProductCardCountFormat", Product.CardCount);
    public string DisplayTargetAudience => Product is null || string.IsNullOrWhiteSpace(Product.TargetAudience)
        ? string.Empty
        : Loc.Format("AudienceTarget", Loc.TranslateTargetAudience(Product.TargetAudience));
    public bool HasTargetAudience => !string.IsNullOrWhiteSpace(DisplayTargetAudience);
    public string DisplayGradeLevel => Product is null || string.IsNullOrWhiteSpace(Product.GradeLevel)
        ? string.Empty
        : Loc.Format("GradeTarget", Loc.TranslateGradeLevel(Product.GradeLevel));
    public bool HasGradeLevel => !string.IsNullOrWhiteSpace(DisplayGradeLevel);

    public string DisplayTitle => Product is null ? string.Empty : Loc.GetLocalized(Product.NameAr, Product.Name);
    public string DisplaySubtitle => Product is null ? string.Empty : (Loc.IsRtl ? Product.Name : Product.NameAr);
    public string DisplayCategory => Product is null ? string.Empty : Loc.GetLocalized(Product.CategoryNameAr, Product.CategoryName);
    public string DisplayDescription => Product is null ? string.Empty : Loc.GetLocalized(Product.DescriptionAr, Product.Description);

    public ProductDetailViewModel(
        IMobileApiClient api,
        ICartStore cart,
        ISpeechService speech,
        IAudioPlayerService audioPlayer,
        ICustomerSessionService session,
        IDialogService dialogs,
        ILocalizationService loc) : base(loc)
    {
        _api = api;
        _cart = cart;
        _speech = speech;
        _audioPlayer = audioPlayer;
        _session = session;
        _dialogs = dialogs;
        Title = Loc["ProductDetailTitle"];
        ReviewerName = session.CustomerName;
        _audioPlayer.PlaybackStateChanged += (_, playing) => IsPlayingTeacherVoice = playing;
        loc.LanguageChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(DisplayTitle));
            OnPropertyChanged(nameof(DisplaySubtitle));
            OnPropertyChanged(nameof(DisplayCategory));
            Title = Loc["ProductDetailTitle"];
            OnPropertyChanged(nameof(DisplayDescription));
            OnPropertyChanged(nameof(HasDescription));
            OnPropertyChanged(nameof(StockLabel));
            OnPropertyChanged(nameof(FavoriteButtonText));
            OnPropertyChanged(nameof(SpeedModeLabel));
            OnPropertyChanged(nameof(TeacherVoiceButtonText));
            OnPropertyChanged(nameof(PriceLabel));
            OnPropertyChanged(nameof(CardCountLabel));
            OnPropertyChanged(nameof(DisplayTargetAudience));
            OnPropertyChanged(nameof(DisplayGradeLevel));
            OnPropertyChanged(nameof(RatingOptions));
            OnPropertyChanged(nameof(ImageCountLabel));
            RefreshKeyPoints();
            RefreshReviewSummary();
        };
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
            OnPropertyChanged(nameof(DisplayTitle));
            OnPropertyChanged(nameof(DisplaySubtitle));
            OnPropertyChanged(nameof(DisplayCategory));
            OnPropertyChanged(nameof(DisplayDescription));
            OnPropertyChanged(nameof(HasDescription));
            OnPropertyChanged(nameof(StockLabel));
            OnPropertyChanged(nameof(PriceLabel));
            OnPropertyChanged(nameof(CardCountLabel));
            OnPropertyChanged(nameof(DisplayTargetAudience));
            OnPropertyChanged(nameof(DisplayGradeLevel));
            ProductImages.Clear();
            foreach (var imagePath in (detail.ImageUrl ?? string.Empty)
                         .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                ProductImages.Add(_api.GetImageUrl(imagePath));
            }
            if (ProductImages.Count == 0)
                ProductImages.Add("logo.png");
            SelectedImageIndex = 0;
            OnPropertyChanged(nameof(HasMultipleImages));
            OnPropertyChanged(nameof(ImageCountLabel));
            IsFavorite = _cart.IsFavorite(detail.Id);

            SampleWords.Clear();
            foreach (var w in detail.SampleWords)
            {
                SampleWords.Add(w);
            }
            OnPropertyChanged(nameof(HasSampleWords));
            RefreshKeyPoints();

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
    private void OpenImagePreview(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return;
        PreviewImageUrl = imageUrl;
        IsImagePreviewOpen = true;
    }

    [RelayCommand]
    private void CloseImagePreview() => IsImagePreviewOpen = false;

    [RelayCommand]
    private async Task OrderViaWhatsAppAsync()
    {
        if (Product is null) return;
        try
        {
            var cfg = await _api.GetConfigAsync();
            var phone = new string((cfg.WhatsAppNumberInternational ?? string.Empty).Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(phone))
            {
                await _dialogs.ShowAlertAsync(Loc["AlertTitle"], Loc["ErrorTitle"]);
                return;
            }
            var msg = Loc.IsRtl
                ? $"مرحباً مس خلود، أود الاستفسار وطلب باقة: {Product.NameAr} بسعر {Product.Price:0} ج.م"
                : $"Hello Miss Kholoud, I would like to order: {Product.Name} ({Product.Price:0} EGP)";
            var uri = $"https://wa.me/{phone}?text={Uri.EscapeDataString(msg)}";
            await Launcher.Default.OpenAsync(uri);
        }
        catch (Exception ex)
        {
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task ShareProductAsync()
    {
        if (Product is null) return;
        var shareUrl = $"https://card-store.runasp.net/Catalog/Details?slug={Uri.EscapeDataString(Product.Slug)}";
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = DisplayTitle,
            Text = $"{DisplayTitle} - {PriceLabel}",
            Uri = shareUrl
        });
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
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], ex.Message);
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
            await _dialogs.ShowToastAsync(Loc.Format("AddToCartToast", DisplayTitle));
        }
        catch (InvalidOperationException)
        {
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], Loc["CartProductUnavailable"]);
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
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], Loc["ReviewRequired"]);
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
            await _dialogs.ShowAlertAsync(Loc["ThankYou"], Loc["ReviewSubmitted"]);
            ReviewComment = string.Empty;
            ReviewerRole = string.Empty;
        }
        catch (MobileApiException ex)
        {
            await _dialogs.ShowAlertAsync(Loc["ErrorTitle"], ex.Message);
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
            _averageRating = response.Summary.AverageRating;
            _reviewCount = response.Summary.TotalReviews;
            _reviewLoadError = null;
            RefreshReviewSummary();
        }
        catch (MobileApiException ex)
        {
            _reviewLoadError = ex.Message;
            RefreshReviewSummary();
        }
    }

    private void RefreshReviewSummary()
    {
        ReviewSummaryText = _reviewLoadError is not null
            ? $"{Loc["ErrorTitle"]}: {_reviewLoadError}"
            : _reviewCount == 0
                ? Loc["NoReviewsBeFirst"]
                : Loc.Format("ReviewSummaryFormat", _averageRating, _reviewCount);
    }

    private void RefreshKeyPoints()
    {
        KeyPoints.Clear();
        if (Product is null)
        {
            OnPropertyChanged(nameof(HasKeyPoints));
            OnPropertyChanged(nameof(HasTargetAudience));
            OnPropertyChanged(nameof(HasGradeLevel));
            return;
        }

        var useEnDirect = !Loc.IsRtl && !string.IsNullOrWhiteSpace(Product.BackKeyPointsEn);
        var raw = useEnDirect ? Product.BackKeyPointsEn : Product.BackKeyPoints;

        foreach (var pt in (raw ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            KeyPoints.Add(useEnDirect ? pt : TranslateKeyPoint(pt));
        }

        OnPropertyChanged(nameof(HasKeyPoints));
        OnPropertyChanged(nameof(HasTargetAudience));
        OnPropertyChanged(nameof(HasGradeLevel));
    }

    private string TranslateKeyPoint(string pt)
    {
        if (Loc.IsRtl || Product is null) return pt;
        if (pt.Contains("كوشيه") || pt.Contains("350")) return "Premium 350gsm laminated cardstock";
        if (pt.Contains("زوايا") || pt.Contains("دائرية")) return "Safe rounded corners for children";
        if (pt.Contains("باركود") || pt.Contains("نطق") || pt.Contains("QR")) return "Audio pronunciation QR code";
        if (pt.Contains("ماء") || pt.Contains("تنظيف")) return "Waterproof & wipeable lamination";
        if (pt.Contains("وجهين") || pt.Contains("بطاقة")) return $"{Product.CardCount} Double-sided full color cards";
        if (pt.Contains("مثال") || pt.Contains("جملة") || pt.Contains("تمارين") || pt.Contains("جمل")) return "Example sentence for each word";
        if (pt.Contains("كلمة") || pt.Contains("كلمات")) return $"{Product.CardCount} Illustrated words";
        if (pt.Contains("تقسيم") || pt.Contains("أصوات") || pt.Contains("الأصوات")) return "Sound-group markers";
        if (pt.Contains("رسمة") || pt.Contains("رسوم") || pt.Contains("صور")) return "Illustration on every card";
        if (pt.Contains("مجموعتين") || pt.Contains("ألوان")) return "Color-coded flashcard groups";
        if (pt.Contains("ألعاب") || pt.Contains("أنشطة")) return "Interactive games & activities";
        return pt;
    }
}
