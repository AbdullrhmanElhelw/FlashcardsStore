using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class MobileApiClient(HttpClient httpClient) : IMobileApiClient
{
    public const string DefaultApiBaseUrl = "http://10.0.2.2:5059/";
    private const string ApiPrefix = "api/v1/";
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    public string BaseUrl => Preferences.Default.Get("api_base_url", DefaultApiBaseUrl);

    public void SetBaseUrl(string value)
    {
        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var address) ||
            (address.Scheme != Uri.UriSchemeHttp && address.Scheme != Uri.UriSchemeHttps) ||
            !string.IsNullOrEmpty(address.Query) || !string.IsNullOrEmpty(address.Fragment))
        {
            throw new ArgumentException("أدخل عنوان خادم صحيحاً يبدأ بـ http:// أو https://", nameof(value));
        }

        var normalized = address.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
            ? address.AbsoluteUri
            : address.AbsoluteUri + "/";
        Preferences.Default.Set("api_base_url", normalized);
    }

    public Task<StoreConfig> GetConfigAsync(CancellationToken cancellationToken = default)
        => GetAsync<StoreConfig>("config", cancellationToken);

    public Task<CustomerProfile> LoginAsync(CustomerLoginRequest request, CancellationToken cancellationToken = default)
        => SendAsync<CustomerProfile>(new HttpRequestMessage(HttpMethod.Post, BuildUri("auth/login"))
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        }, cancellationToken);

    public Task<CustomerProfile> RegisterAsync(CustomerRegisterRequest request, CancellationToken cancellationToken = default)
        => SendAsync<CustomerProfile>(new HttpRequestMessage(HttpMethod.Post, BuildUri("auth/register"))
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        }, cancellationToken);

    public Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => GetAsync<List<Category>>("categories", cancellationToken);

    public Task<List<ProductCard>> GetFeaturedAsync(CancellationToken cancellationToken = default)
        => GetAsync<List<ProductCard>>("products/featured", cancellationToken);

    public Task<PagedResult<ProductCard>> GetProductsAsync(
        int? categoryId = null,
        string? search = null,
        string? grade = null,
        string? sort = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string> { $"page={page}", "pageSize=12" };
        if (categoryId.HasValue) query.Add($"categoryId={categoryId.Value}");
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        if (!string.IsNullOrWhiteSpace(grade)) query.Add($"grade={Uri.EscapeDataString(grade)}");
        if (!string.IsNullOrWhiteSpace(sort)) query.Add($"sort={Uri.EscapeDataString(sort)}");
        return GetAsync<PagedResult<ProductCard>>("products?" + string.Join("&", query), cancellationToken);
    }

    public Task<ProductDetail> GetProductAsync(string slug, CancellationToken cancellationToken = default)
        => GetAsync<ProductDetail>($"products/{Uri.EscapeDataString(slug)}", cancellationToken);

    public Task<ProductReviewsResponse> GetReviewsAsync(int productId, CancellationToken cancellationToken = default)
        => GetAsync<ProductReviewsResponse>($"products/{productId}/reviews", cancellationToken);

    public async Task AddReviewAsync(CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(new HttpRequestMessage(HttpMethod.Post, BuildUri($"products/{request.ProductId}/reviews"))
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        }, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public Task<List<ProductCard>> GetProductsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        => SendAsync<List<ProductCard>>(new HttpRequestMessage(HttpMethod.Post, BuildUri("products/by-ids"))
        {
            Content = JsonContent.Create(ids.Distinct().ToList(), options: JsonOptions)
        }, cancellationToken);

    public Task<ShippingQuote> GetShippingQuoteAsync(string governorate, decimal subtotal, CancellationToken cancellationToken = default)
        => GetAsync<ShippingQuote>($"shipping-fee?governorate={Uri.EscapeDataString(governorate)}&subtotal={subtotal.ToString(System.Globalization.CultureInfo.InvariantCulture)}", cancellationToken);

    public Task<OrderDetail> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        => SendAsync<OrderDetail>(new HttpRequestMessage(HttpMethod.Post, BuildUri("orders"))
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        }, cancellationToken);

    public Task<OrderTracking> TrackOrderAsync(string orderNumber, string phone, CancellationToken cancellationToken = default)
        => GetAsync<OrderTracking>($"orders/track?orderNumber={Uri.EscapeDataString(orderNumber)}&phone={Uri.EscapeDataString(phone)}", cancellationToken);

    public async Task UploadPaymentProofAsync(
        string orderNumber,
        string phone,
        FileResult file,
        string? walletName,
        string? senderPhone,
        string? transactionReference,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => throw new MobileApiException("الصيغ المسموحة للإيصال هي JPG أو PNG أو WEBP")
        };

        await using var stream = await file.OpenReadAsync();
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(phone), "phone");
        form.Add(new StringContent(walletName ?? string.Empty), "bankOrWalletName");
        form.Add(new StringContent(senderPhone ?? string.Empty), "senderPhoneNumber");
        form.Add(new StringContent(transactionReference ?? string.Empty), "transactionReference");
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "proofFile", Path.GetFileName(file.FileName));

        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUri($"orders/{Uri.EscapeDataString(orderNumber)}/payment-proof"))
        {
            Content = form
        };
        using var response = await SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public string GetImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return string.Empty;
        return Uri.TryCreate(imageUrl, UriKind.Absolute, out var absolute)
            ? absolute.AbsoluteUri
            : new Uri(new Uri(BaseUrl, UriKind.Absolute), imageUrl.TrimStart('/')).AbsoluteUri;
    }

    private Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
        => SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, BuildUri(path)), cancellationToken);

    private Uri BuildUri(string path) => new(new Uri(BaseUrl, UriKind.Absolute), ApiPrefix + path);

    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using (request)
        using (var response = await SendAsync(request, cancellationToken))
        {
            await EnsureSuccessAsync(response, cancellationToken);
            try
            {
                var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
                return value ?? throw new MobileApiException("استجاب الخادم ببيانات غير مكتملة");
            }
            catch (JsonException exception)
            {
                throw new MobileApiException($"تعذر قراءة استجابة الخادم: {exception.Message}");
            }
        }
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new MobileApiException($"تعذر الاتصال بالخادم: {exception.Message}");
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = TryReadError(body);
        if (string.IsNullOrWhiteSpace(message))
        {
            message = response.StatusCode == HttpStatusCode.NotFound
                ? "لم يتم العثور على البيانات المطلوبة"
                : $"فشل الطلب ({(int)response.StatusCode})";
        }

        throw new MobileApiException(message);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private static string? TryReadError(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return null;
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("error", out var error)) return error.GetString();
            if (document.RootElement.TryGetProperty("title", out var title)) return title.GetString();
            if (document.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                return string.Join("\n", errors.EnumerateObject().SelectMany(property => property.Value.EnumerateArray()).Select(value => value.GetString()).Where(value => !string.IsNullOrWhiteSpace(value)));
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }
}
