using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(
        IProductService productService,
        ICategoryService categoryService,
        IHttpClientFactory httpClientFactory)
    {
        _productService = productService;
        _categoryService = categoryService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var featuredProducts = await _productService.GetFeaturedCardsAsync(cancellationToken);
        if (featuredProducts.Count == 0)
        {
            var catalogPage = await _productService.GetCatalogCardsAsync(
                new ProductFilterDto { PageIndex = 1, PageSize = 6 },
                cancellationToken);
            featuredProducts = catalogPage.Items;
        }
        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.Categories = categories;

        return View(featuredProducts);
    }

    [HttpGet("Apps")]
    public IActionResult Apps(string tab = "phonics")
    {
        ViewBag.ActiveTab = string.Equals(tab, "store", StringComparison.OrdinalIgnoreCase) ? "store" : "phonics";
        return View("Apps");
    }

    [HttpGet("PhonicsApp")]
    public IActionResult PhonicsApp() => RedirectToAction(nameof(Apps), new { tab = "phonics" });

    [HttpGet("StoreApp")]
    public IActionResult StoreApp() => RedirectToAction(nameof(Apps), new { tab = "store" });

    [HttpGet("Privacy")]
    public IActionResult Privacy() => View();

    [Route("Home/Error")]
    [AcceptVerbs("GET", "POST")]
    [IgnoreAntiforgeryToken]
    public IActionResult Error()
        => RedirectToAction("Index", "Catalog");

    [HttpGet("download/{appName}")]
    public async Task<IActionResult> DownloadApp(string appName, CancellationToken cancellationToken)
    {
        var target = (appName ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "store-app" => (
                Url: "https://github.com/AbdullrhmanElhelw/FlashcardsStore/releases/download/v1.0.3/com.kholoudelnady.flashcards-Signed.apk",
                FileName: "com.kholoudelnady.flashcards-Signed.apk"),
            "phonics-app" => (
                Url: "https://github.com/AbdullrhmanElhelw/FlashcardsStore/releases/download/phonics-v1.0.0/com.kholoudelnady.phonicsgame-Signed.apk",
                FileName: "com.kholoudelnady.phonicsgame-Signed.apk"),
            _ => default
        };

        if (string.IsNullOrEmpty(target.Url))
            return NotFound();

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(3);

        var response = await client.GetAsync(target.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            response.Dispose();
            return Redirect(target.Url);
        }

        Response.RegisterForDispose(response);
        if (response.Content.Headers.ContentLength is { } contentLength)
            Response.ContentLength = contentLength;

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return File(stream, "application/vnd.android.package-archive", target.FileName);
    }
}
