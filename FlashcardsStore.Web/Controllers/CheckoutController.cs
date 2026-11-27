using System.Security.Cryptography;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.DTOs.PaymentProof;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Domain.Enums;
using FlashcardsStore.Infrastructure.Persistence;
using FlashcardsStore.Web.Common;

namespace FlashcardsStore.Web.Controllers;

public class CheckoutController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPaymentProofService _paymentProofService;
    private readonly CartSessionManager _cartManager;
    private readonly IValidator<CheckoutDto> _checkoutValidator;
    private readonly IValidator<PaymentProofUploadDto> _proofValidator;
    private readonly ILocalizationService _loc;
    private readonly FlashcardsStoreDbContext _db;

    public CheckoutController(
        IOrderService orderService,
        IPaymentProofService paymentProofService,
        CartSessionManager cartManager,
        IValidator<CheckoutDto> checkoutValidator,
        IValidator<PaymentProofUploadDto> proofValidator,
        ILocalizationService loc,
        FlashcardsStoreDbContext db)
    {
        _orderService = orderService;
        _paymentProofService = paymentProofService;
        _cartManager = cartManager;
        _checkoutValidator = checkoutValidator;
        _proofValidator = proofValidator;
        _loc = loc;
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var cart = _cartManager.GetCartViewModel();
        if (cart.IsEmpty)
        {
            return RedirectToAction("Index", "Catalog");
        }

        ViewBag.Cart = cart;
        var model = new CheckoutDto();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(CheckoutDto dto, CancellationToken cancellationToken)
    {
        var cart = _cartManager.GetCartViewModel();
        if (cart.IsEmpty)
        {
            return RedirectToAction("Index", "Catalog");
        }

        // Clear implicit English validation errors before running explicit Arabic FluentValidation
        ModelState.Clear();

        var validationResult = await _checkoutValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            ViewBag.Cart = cart;
            return View("Index", dto);
        }

        var result = await _orderService.CreateOrderAsync(dto, cart.Items, cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر إنشاء الطلب، يرجى المحاولة لاحقاً");
            ViewBag.Cart = cart;
            return View("Index", dto);
        }

        // Clear cart after order creation
        _cartManager.ClearCart();

        // Auto-generate a Jolly Phonics App activation code for this order
        await EnsureOrderPhonicsCodeAsync(result.Data.OrderNumber, result.Data.CustomerName, cancellationToken);

        if (dto.PaymentMethod == PaymentMethod.WalletTransfer)
        {
            return RedirectToAction("PaymentInstructions", new { orderNumber = result.Data.OrderNumber });
        }

        return RedirectToAction("Confirmation", new { orderNumber = result.Data.OrderNumber });
    }

    [HttpGet]
    public async Task<IActionResult> PaymentInstructions(string orderNumber, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber, cancellationToken);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProof(
        int orderId,
        IFormFile? proofFile,
        string? bankOrWalletName,
        string? senderPhoneNumber,
        string? transactionReference,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            return NotFound();
        }

        if (proofFile == null || proofFile.Length == 0)
        {
            TempData["ErrorMessage"] = _loc.Get("ToastSelectProofFile");
            return RedirectToAction("PaymentInstructions", new { orderNumber = order.OrderNumber });
        }

        var uploadDto = new PaymentProofUploadDto
        {
            OrderId = orderId,
            BankOrWalletName = bankOrWalletName,
            SenderPhoneNumber = senderPhoneNumber,
            TransactionReference = transactionReference,
            FileStream = proofFile.OpenReadStream(),
            FileName = proofFile.FileName,
            ContentType = proofFile.ContentType,
            FileLength = proofFile.Length
        };

        var validationResult = await _proofValidator.ValidateAsync(uploadDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
            return RedirectToAction("PaymentInstructions", new { orderNumber = order.OrderNumber });
        }

        var result = await _paymentProofService.UploadProofAsync(uploadDto, cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction("PaymentInstructions", new { orderNumber = order.OrderNumber });
        }

        TempData["SuccessMessage"] = _loc.Get("ToastProofUploaded");
        return RedirectToAction("Confirmation", new { orderNumber = order.OrderNumber });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(string orderNumber, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber, cancellationToken);
        if (order == null)
        {
            return NotFound();
        }

        var noteKey = $"Order:{orderNumber}";
        var codeEntity = await _db.PhonicsActivationCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Notes == noteKey, cancellationToken);
        ViewBag.PhonicsCode = codeEntity?.Code;

        return View(order);
    }

    private async Task EnsureOrderPhonicsCodeAsync(string orderNumber, string? customerName, CancellationToken ct)
    {
        var noteKey = $"Order:{orderNumber}";
        var exists = await _db.PhonicsActivationCodes.AnyAsync(c => c.Notes == noteKey, ct);
        if (exists) return;

        string code;
        do
        {
            code = GenerateOrderCode();
        } while (await _db.PhonicsActivationCodes.AnyAsync(c => c.Code == code, ct));

        _db.PhonicsActivationCodes.Add(new PhonicsActivationCode
        {
            Code = code,
            BundleName = $"طلب المتجر ({customerName ?? orderNumber})",
            Notes = noteKey,
            MaxDevices = 2,
            IsActive = true
        });
        await _db.SaveChangesAsync(ct);
    }

    private static string GenerateOrderCode()
    {
        const string alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
        Span<char> p1 = stackalloc char[4];
        Span<char> p2 = stackalloc char[4];
        for (var i = 0; i < 4; i++)
        {
            p1[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
            p2[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
        }
        return $"KHO-{new string(p1)}-{new string(p2)}";
    }
}
