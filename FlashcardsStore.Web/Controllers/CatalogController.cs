using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Web.Controllers;

public class CatalogController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IReviewService _reviewService;

    public CatalogController(
        IProductService productService,
        ICategoryService categoryService,
        IReviewService reviewService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? categoryId, string? searchTerm, string? gradeLevel, string? sortBy, int page = 1, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        var filter = new ProductFilterDto
        {
            CategoryId = categoryId,
            SearchTerm = searchTerm,
            GradeLevel = gradeLevel,
            SortBy = sortBy,
            PageIndex = page,
            PageSize = 12
        };

        var pagedResult = await _productService.GetCatalogCardsAsync(filter, cancellationToken);
        var featured = await _productService.GetFeaturedCardsAsync(cancellationToken);

        ViewBag.Categories = categories;
        ViewBag.CurrentFilter = filter;
        ViewBag.FeaturedProducts = featured;

        return View(pagedResult);
    }

    [HttpGet]
    public async Task<IActionResult> Filter(ProductFilterDto filter, int page = 1, CancellationToken cancellationToken = default)
    {
        filter.PageIndex = page;
        if (filter.PageSize <= 0) filter.PageSize = 12;

        var pagedResult = await _productService.GetCatalogCardsAsync(filter, cancellationToken);
        ViewBag.CurrentFilter = filter;
        ViewBag.PagedInfo = pagedResult;
        ViewBag.PaginationBaseUrl = $"/Catalog?gradeLevel={Uri.EscapeDataString(filter.GradeLevel ?? "")}&categoryId={filter.CategoryId ?? 0}&searchTerm={Uri.EscapeDataString(filter.SearchTerm ?? "")}&sortBy={Uri.EscapeDataString(filter.SortBy ?? "")}";
        return PartialView("_ProductGrid", pagedResult);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string slug, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetProductDetailBySlugAsync(slug, cancellationToken);
        if (product == null)
        {
            return NotFound();
        }

        var reviews = await _reviewService.GetReviewsByProductIdAsync(product.Id);
        var reviewSummary = await _reviewService.GetProductReviewSummaryAsync(product.Id);

        ViewBag.Reviews = reviews;
        ViewBag.ReviewSummary = reviewSummary;

        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] FlashcardsStore.Application.DTOs.Reviews.CreateReviewDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, message = string.Join(" | ", errors) });
        }

        var result = await _reviewService.AddReviewAsync(dto);
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new { success = true, review = result.Value });
    }
}
