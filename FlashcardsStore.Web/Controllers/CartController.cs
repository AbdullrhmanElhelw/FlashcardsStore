using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.DTOs.Cart;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Web.Common;

namespace FlashcardsStore.Web.Controllers;

public class CartController : Controller
{
    private readonly CartSessionManager _cartManager;
    private readonly IProductService _productService;

    public CartController(CartSessionManager cartManager, IProductService productService)
    {
        _cartManager = cartManager;
        _productService = productService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = _cartManager.GetCartViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetByIdAsync(productId, cancellationToken);
        if (product == null || product.AvailableStockQuantity < quantity)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString("عفواً، الكمية المطلوبة غير متوفرة في المخزن"));
            Response.Headers.Append("HX-Toast-Type", "error");
            var currentModel = _cartManager.GetCartViewModel();
            return PartialView("_CartDrawer", currentModel);
        }

        _cartManager.AddItem(new CartItemDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductNameAr = product.NameAr,
            ImageUrl = product.ImageUrl,
            UnitPrice = product.Price,
            Quantity = quantity,
            AvailableStock = product.AvailableStockQuantity
        });

        var model = _cartManager.GetCartViewModel();

        var triggerData = JsonSerializer.Serialize(new { count = model.TotalItems, openDrawer = true });
        Response.Headers.Append("HX-Trigger", $"{{ \"cartUpdated\": {triggerData} }}");
        Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString($"تمت إضافة {product.NameAr} إلى السلة"));

        return PartialView("_CartDrawer", model);
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        _cartManager.UpdateQuantity(productId, quantity);
        var model = _cartManager.GetCartViewModel();

        var triggerData = JsonSerializer.Serialize(new { count = model.TotalItems, openDrawer = false });
        Response.Headers.Append("HX-Trigger", $"{{ \"cartUpdated\": {triggerData} }}");

        return PartialView("_CartDrawer", model);
    }

    [HttpPost]
    public IActionResult RemoveItem(int productId)
    {
        _cartManager.RemoveItem(productId);
        var model = _cartManager.GetCartViewModel();

        var triggerData = JsonSerializer.Serialize(new { count = model.TotalItems, openDrawer = false });
        Response.Headers.Append("HX-Trigger", $"{{ \"cartUpdated\": {triggerData} }}");
        Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString("تم حذف المنتج من السلة"));

        return PartialView("_CartDrawer", model);
    }

    [HttpGet]
    public IActionResult Drawer()
    {
        var model = _cartManager.GetCartViewModel();
        return PartialView("_CartDrawer", model);
    }

    [HttpGet]
    public IActionResult Summary()
    {
        var model = _cartManager.GetCartViewModel();
        return PartialView("_CartSummary", model);
    }
}
