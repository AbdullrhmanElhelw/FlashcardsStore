using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.DTOs.Cart;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.DTOs.PaymentProof;
using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.DTOs.Reviews;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Web.Api;

/// <summary>
/// Stateless JSON API consumed by the .NET MAUI mobile app.
/// No cookies / sessions / antiforgery: the cart lives on the device and
/// prices are always re-read from the database when an order is created.
/// </summary>
[ApiController]
[IgnoreAntiforgeryToken]
[Route("api/v1")]
public class MobileApiController : ControllerBase
{
    private const long MaxProofBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedProofTypes = { "image/jpeg", "image/png", "image/webp" };

    private readonly IProductService _products;
    private readonly ICategoryService _categories;
    private readonly IOrderService _orders;
    private readonly IPaymentProofService _proofs;
    private readonly IStoreSettingsService _settings;
    private readonly IReviewService _reviews;
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<PaymentProofUploadDto> _proofValidator;

    public MobileApiController(
        IProductService products,
        ICategoryService categories,
        IOrderService orders,
        IPaymentProofService proofs,
        IStoreSettingsService settings,
        IValidator<PaymentProofUploadDto> proofValidator,
        IReviewService reviews,
        ICustomerRepository customers,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _products = products;
        _categories = categories;
        _orders = orders;
        _proofs = proofs;
        _settings = settings;
        _proofValidator = proofValidator;
        _reviews = reviews;
        _customers = customers;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("auth/register")]
    public async Task<ActionResult<MobileCustomerProfile>> Register(
        [FromBody] MobileCustomerRegisterRequest request,
        CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _customers.GetByEmailAsync(email, ct) is not null)
            return Conflict(new { error = "يوجد حساب مسجل بهذا البريد الإلكتروني" });
        if (await _customers.GetAccountByPhoneAsync(request.PhoneNumber.Trim(), ct) is not null)
            return Conflict(new { error = "رقم الهاتف مرتبط بحساب آخر بالفعل" });

        if (request.PhoneNumber.Count(char.IsDigit) < 7)
            return BadRequest(new { error = "أدخل رقم هاتف صحيحاً" });

        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = request.PhoneNumber.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            ShippingAddress = string.Empty,
            City = string.Empty,
            Governorate = string.Empty
        };
        await _customers.AddAsync(customer, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Ok(ToCustomerProfile(customer));
    }

    [HttpPost("auth/login")]
    public async Task<ActionResult<MobileCustomerProfile>> Login(
        [FromBody] MobileCustomerLoginRequest request,
        CancellationToken ct)
    {
        var customer = await _customers.GetByEmailAsync(request.Email, ct);
        if (customer?.PasswordHash is null || !_passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
            return Unauthorized(new { error = "البريد الإلكتروني أو كلمة المرور غير صحيحة" });

        return Ok(ToCustomerProfile(customer));
    }

    private static MobileCustomerProfile ToCustomerProfile(Customer customer)
        => new(customer.Id, customer.FullName, customer.Email ?? string.Empty, customer.PhoneNumber);

    [HttpGet("config")]
    public async Task<ActionResult<MobileStoreConfig>> GetConfig(CancellationToken ct)
    {
        var s = await _settings.GetSettingsAsync(ct);
        return new MobileStoreConfig
        {
            StoreNameAr = s.StoreNameAr,
            StoreNameEn = s.StoreNameEn,
            WhatsAppNumberInternational = _settings.GetWhatsAppNumberInternational(),
            ContactPhone = s.ContactPhone,
            ContactEmail = s.ContactEmail,
            VodafoneCashNumber = s.VodafoneCashNumber,
            InstaPayAddress = s.InstaPayAddress,
            InstaPayAccountName = s.InstaPayAccountName,
            PaymentInstructionsAr = s.PaymentInstructionsAr,
            ShippingFeeCairoGiza = s.ShippingFeeCairoGiza,
            ShippingFeeDeltaCanal = s.ShippingFeeDeltaCanal,
            ShippingFeeUpperEgypt = s.ShippingFeeUpperEgypt,
            FreeShippingThreshold = s.FreeShippingThreshold,
            EstimatedDeliveryTimeAr = s.EstimatedDeliveryTimeAr,
            AnnouncementBarEnabled = s.AnnouncementBarEnabled,
            AnnouncementTextAr = s.AnnouncementTextAr,
            AnnouncementTextEn = s.AnnouncementTextEn,
            AnnouncementLink = s.AnnouncementLink
        };
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
        => Ok(await _categories.GetAllCategoriesAsync(ct));

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] string? grade,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken ct = default)
    {
        var filter = new ProductFilterDto
        {
            CategoryId = categoryId,
            SearchTerm = search,
            GradeLevel = grade,
            SortBy = sort,
            PageIndex = Math.Max(1, page),
            PageSize = Math.Clamp(pageSize, 1, 50)
        };
        return Ok(await _products.GetCatalogCardsAsync(filter, ct));
    }

    [HttpGet("products/featured")]
    public async Task<IActionResult> GetFeatured(CancellationToken ct)
        => Ok(await _products.GetFeaturedCardsAsync(ct));

    [HttpGet("products/{slug}")]
    public async Task<IActionResult> GetProduct(string slug, CancellationToken ct)
    {
        var product = await _products.GetProductDetailBySlugAsync(slug, ct);
        return product == null ? NotFound(new { error = "المنتج غير موجود" }) : Ok(product);
    }

    [HttpPost("products/by-ids")]
    public async Task<IActionResult> GetProductsByIds([FromBody] List<int>? ids, CancellationToken ct)
    {
        if (ids == null || ids.Count == 0) return Ok(Array.Empty<ProductCardViewModel>());
        return Ok(await _products.GetCardsByIdsAsync(ids.Where(id => id > 0).Distinct().Take(50), ct));
    }

    [HttpGet("products/{productId:int}/reviews")]
    public async Task<IActionResult> GetReviews(int productId)
        => Ok(new
        {
            reviews = await _reviews.GetReviewsByProductIdAsync(productId),
            summary = await _reviews.GetProductReviewSummaryAsync(productId)
        });

    [HttpPost("products/{productId:int}/reviews")]
    public async Task<IActionResult> AddReview(int productId, [FromBody] CreateReviewDto request)
    {
        if (request.ProductId != productId) return BadRequest(new { error = "المنتج المحدد غير صحيح" });

        var result = await _reviews.AddReviewAsync(request);
        return result.IsSuccess
            ? Ok(new { success = true, review = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("shipping-fee")]
    public async Task<ActionResult<MobileShippingQuote>> GetShippingFee(
        [FromQuery] string governorate, [FromQuery] decimal subtotal, CancellationToken ct)
    {
        await _settings.GetSettingsAsync(ct); // warm cache
        var fee = _settings.CalculateShippingFee(governorate, Math.Max(0, subtotal));
        return new MobileShippingQuote
        {
            ShippingFee = fee,
            Subtotal = subtotal,
            GrandTotal = subtotal + fee,
            IsFreeShipping = fee == 0
        };
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] MobileCreateOrderRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        await _settings.GetSettingsAsync(ct); // warm cache

        // Merge duplicate lines and rebuild every line from the database (never trust client prices).
        var lines = new List<CartItemDto>();
        foreach (var group in request.Items.GroupBy(i => i.ProductId))
        {
            var product = await _products.GetByIdAsync(group.Key, ct);
            if (product == null || !product.IsActive)
                return BadRequest(new { error = "أحد المنتجات لم يعد متاحاً" });

            lines.Add(new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductNameAr = product.NameAr,
                ImageUrl = product.ImageUrl,
                UnitPrice = product.Price,
                Quantity = group.Sum(i => i.Quantity),
                AvailableStock = product.AvailableStockQuantity
            });
        }

        var checkout = new CheckoutDto
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            WhatsAppNumber = request.WhatsAppNumber?.Trim(),
            Email = request.Email?.Trim(),
            ShippingAddress = request.ShippingAddress.Trim(),
            City = request.City.Trim(),
            Governorate = request.Governorate.Trim(),
            CustomerType = request.CustomerType,
            PaymentMethod = request.PaymentMethod,
            Notes = request.Notes?.Trim()
        };

        var result = await _orders.CreateOrderAsync(checkout, lines, ct);
        if (!result.Succeeded) return BadRequest(new { error = result.Error });

        var order = result.Data!;
        return CreatedAtAction(nameof(Track), new { orderNumber = order.OrderNumber, phone = checkout.PhoneNumber }, order);
    }

    [HttpGet("orders/track")]
    public async Task<IActionResult> Track([FromQuery] string orderNumber, [FromQuery] string phone, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(phone))
            return BadRequest(new { error = "رقم الطلب ورقم الهاتف مطلوبان" });

        var tracking = await _orders.TrackOrderAsync(orderNumber.Trim(), phone.Trim(), ct);
        return tracking == null ? NotFound(new { error = "لم يتم العثور على الطلب، تأكد من رقم الطلب والهاتف" }) : Ok(tracking);
    }

    [HttpPost("orders/{orderNumber}/payment-proof")]
    [RequestSizeLimit(MaxProofBytes + 1024 * 64)]
    public async Task<IActionResult> UploadProof(
        string orderNumber,
        [FromForm] string phone,
        IFormFile? proofFile,
        [FromForm] string? bankOrWalletName,
        [FromForm] string? senderPhoneNumber,
        [FromForm] string? transactionReference,
        CancellationToken ct)
    {
        // Ownership check: order number + the phone used at checkout.
        var tracking = string.IsNullOrWhiteSpace(phone)
            ? null
            : await _orders.TrackOrderAsync(orderNumber.Trim(), phone.Trim(), ct);
        if (tracking == null) return NotFound(new { error = "لم يتم العثور على الطلب" });

        if (proofFile == null || proofFile.Length == 0)
            return BadRequest(new { error = "يرجى اختيار صورة الإيصال" });
        if (proofFile.Length > MaxProofBytes)
            return BadRequest(new { error = "حجم الصورة يجب ألا يتجاوز 5 ميجابايت" });
        if (!AllowedProofTypes.Contains(proofFile.ContentType, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { error = "الصيغة المسموحة: JPG أو PNG أو WEBP" });

        var order = await _orders.GetOrderByOrderNumberAsync(orderNumber.Trim(), ct);
        if (order == null) return NotFound(new { error = "لم يتم العثور على الطلب" });

        await using var stream = proofFile.OpenReadStream();
        var dto = new PaymentProofUploadDto
        {
            OrderId = order.Id,
            BankOrWalletName = bankOrWalletName,
            SenderPhoneNumber = senderPhoneNumber,
            TransactionReference = transactionReference,
            FileStream = stream,
            FileName = Path.GetFileName(proofFile.FileName),
            ContentType = proofFile.ContentType,
            FileLength = proofFile.Length
        };

        var validation = await _proofValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return BadRequest(new { error = validation.Errors.First().ErrorMessage });

        var result = await _proofs.UploadProofAsync(dto, ct);
        return result.Succeeded ? Ok(new { message = "تم رفع الإيصال بنجاح" }) : BadRequest(new { error = result.Error });
    }
}
