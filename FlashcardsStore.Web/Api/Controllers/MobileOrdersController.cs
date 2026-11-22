using FluentValidation;
using FlashcardsStore.Application.DTOs.Cart;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.DTOs.PaymentProof;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Web.Api.Common;
using FlashcardsStore.Web.Api.Contracts.Orders;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Handles stateless order creation, order tracking, and payment proof uploads.</summary>
public sealed class MobileOrdersController : MobileApiControllerBase
{
    private const long MaxProofBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedProofTypes = { "image/jpeg", "image/png", "image/webp" };

    private readonly IProductService _products;
    private readonly IOrderService _orders;
    private readonly IPaymentProofService _proofs;
    private readonly IStoreSettingsService _settings;
    private readonly IValidator<PaymentProofUploadDto> _proofValidator;

    public MobileOrdersController(
        IProductService products,
        IOrderService orders,
        IPaymentProofService proofs,
        IStoreSettingsService settings,
        IValidator<PaymentProofUploadDto> proofValidator)
    {
        _products = products;
        _orders = orders;
        _proofs = proofs;
        _settings = settings;
        _proofValidator = proofValidator;
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] MobileCreateOrderRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        await _settings.GetSettingsAsync(ct); // warm cache

        var lines = await BuildValidatedOrderLinesAsync(request.Items, ct);
        if (lines is null)
            return Fail("أحد المنتجات لم يعد متاحاً");

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
        if (!result.Succeeded)
            return Fail(result.Error ?? "تعذر إنشاء الطلب");

        var order = result.Data!;
        return CreatedAtAction(nameof(Track), new { orderNumber = order.OrderNumber, phone = checkout.PhoneNumber }, order);
    }

    [HttpGet("orders/track")]
    public async Task<IActionResult> Track([FromQuery] string orderNumber, [FromQuery] string phone, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(phone))
            return Fail("رقم الطلب ورقم الهاتف مطلوبان");

        var tracking = await _orders.TrackOrderAsync(orderNumber.Trim(), phone.Trim(), ct);
        return tracking is null
            ? NotFoundError("لم يتم العثور على الطلب، تأكد من رقم الطلب والهاتف")
            : Ok(tracking);
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
        if (tracking is null)
            return NotFoundError("لم يتم العثور على الطلب");

        var fileError = ValidateProofFile(proofFile);
        if (fileError is not null)
            return Fail(fileError);

        var order = await _orders.GetOrderByOrderNumberAsync(orderNumber.Trim(), ct);
        if (order is null)
            return NotFoundError("لم يتم العثور على الطلب");

        await using var stream = proofFile!.OpenReadStream();
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
            return Fail(validation.Errors.First().ErrorMessage);

        var result = await _proofs.UploadProofAsync(dto, ct);
        return result.Succeeded
            ? OkMessage("تم رفع الإيصال بنجاح")
            : Fail(result.Error ?? "تعذر رفع الإيصال");
    }

    private async Task<List<CartItemDto>?> BuildValidatedOrderLinesAsync(
        IEnumerable<MobileOrderItem> items,
        CancellationToken ct)
    {
        // Merge duplicate lines and rebuild every line from the database (never trust client prices).
        var lines = new List<CartItemDto>();
        foreach (var group in items.GroupBy(i => i.ProductId))
        {
            var product = await _products.GetByIdAsync(group.Key, ct);
            if (product is null || !product.IsActive)
                return null;

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

        return lines;
    }

    private static string? ValidateProofFile(IFormFile? proofFile)
    {
        if (proofFile is null || proofFile.Length == 0)
            return "يرجى اختيار صورة الإيصال";
        if (proofFile.Length > MaxProofBytes)
            return "حجم الصورة يجب ألا يتجاوز 5 ميجابايت";
        if (!AllowedProofTypes.Contains(proofFile.ContentType, StringComparer.OrdinalIgnoreCase))
            return "الصيغة المسموحة: JPG أو PNG أو WEBP";
        return null;
    }
}
