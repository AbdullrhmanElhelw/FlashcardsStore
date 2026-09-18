using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Web.Controllers;

public class WishlistController : Controller
{
    private readonly IProductService _productService;
    private readonly ILocalizationService _loc;

    public WishlistController(IProductService productService, ILocalizationService loc)
    {
        _productService = productService;
        _loc = loc;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? ids)
    {
        var idList = new List<int>();
        if (!string.IsNullOrWhiteSpace(ids))
        {
            foreach (var part in ids.Split(new[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(part.Trim(), out var parsedId))
                {
                    idList.Add(parsedId);
                }
            }
        }

        var products = idList.Any()
            ? await _productService.GetCardsByIdsAsync(idList)
            : Array.Empty<FlashcardsStore.Application.DTOs.Products.ProductCardViewModel>();

        ViewBag.IdsString = ids ?? "";
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> GetItems([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return Json(new { success = true, data = Array.Empty<object>() });

        var products = await _productService.GetCardsByIdsAsync(ids);
        return Json(new { success = true, data = products });
    }
}
