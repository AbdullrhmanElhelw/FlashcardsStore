using FlashcardsStore.Mobile.Models.Api;

namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface IMobileApiClient
{
    string BaseUrl { get; }
    void SetBaseUrl(string value);
    string GetImageUrl(string? imageUrl);

    Task<StoreConfig> GetConfigAsync(CancellationToken cancellationToken = default);
    Task<CustomerProfile> LoginAsync(CustomerLoginRequest request, CancellationToken cancellationToken = default);
    Task<CustomerProfile> RegisterAsync(CustomerRegisterRequest request, CancellationToken cancellationToken = default);
    Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<List<ProductCard>> GetFeaturedAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<ProductCard>> GetProductsAsync(
        int? categoryId = null,
        string? search = null,
        string? grade = null,
        string? sort = null,
        int page = 1,
        CancellationToken cancellationToken = default);
    Task<ProductDetail> GetProductAsync(string slug, CancellationToken cancellationToken = default);
    Task<ProductReviewsResponse> GetReviewsAsync(int productId, CancellationToken cancellationToken = default);
    Task AddReviewAsync(CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<List<ProductCard>> GetProductsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<ShippingQuote> GetShippingQuoteAsync(string governorate, decimal subtotal, CancellationToken cancellationToken = default);
    Task<OrderDetail> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderTracking> TrackOrderAsync(string orderNumber, string phone, CancellationToken cancellationToken = default);
    Task UploadPaymentProofAsync(
        string orderNumber,
        string phone,
        FileResult file,
        string? walletName,
        string? senderPhone,
        string? transactionReference,
        CancellationToken cancellationToken = default);
}
