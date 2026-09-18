using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var featuredProducts = await _productService.GetFeaturedCardsAsync(cancellationToken);
        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.Categories = categories;

        return View(featuredProducts);
    }
}
