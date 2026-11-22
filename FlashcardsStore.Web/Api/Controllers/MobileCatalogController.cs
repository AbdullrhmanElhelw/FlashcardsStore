using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Web.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Exposes category and product catalog endpoints for the mobile storefront.</summary>
public sealed class MobileCatalogController : MobileApiControllerBase
{
    private readonly IProductService _products;
    private readonly ICategoryService _categories;

    public MobileCatalogController(IProductService products, ICategoryService categories)
    {
        _products = products;
        _categories = categories;
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
        return product is null ? NotFoundError("المنتج غير موجود") : Ok(product);
    }

    [HttpPost("products/by-ids")]
    public async Task<IActionResult> GetProductsByIds([FromBody] List<int>? ids, CancellationToken ct)
    {
        if (ids is null || ids.Count == 0)
            return Ok(Array.Empty<ProductCardViewModel>());

        var validIds = ids.Where(id => id > 0).Distinct().Take(50);
        return Ok(await _products.GetCardsByIdsAsync(validIds, ct));
    }
}
