using FlashcardsStore.Application.DTOs.Categories;
using FlashcardsStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Manages flashcard categories and storefront display ordering.</summary>
public sealed class AdminCategoriesController : AdminControllerBase
{
    private readonly ICategoryService _categoryService;

    public AdminCategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("Categories")]
    public async Task<IActionResult> Categories(
        string? search,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync(cancellationToken)).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            categories = categories.Where(c =>
                (!string.IsNullOrEmpty(c.NameAr) && c.NameAr.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Slug) && c.Slug.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var filtered = categories.ToList();
        var pagedResult = BuildPagedResult(filtered, page, pageSize, out var pagedCategories);

        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Categories?search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return AdminView("Categories", pagedCategories);
    }

    [HttpGet("CreateCategory")]
    public IActionResult CreateCategory()
        => AdminView("CreateCategory", new CreateCategoryDto { DisplayOrder = 10 });

    [HttpPost("CreateCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Slug))
            dto.Slug = dto.Name.ToLowerInvariant().Replace(" ", "-");

        if (!ModelState.IsValid)
            return AdminView("CreateCategory", dto);

        var result = await _categoryService.CreateCategoryAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر إضافة القسم");
            return AdminView("CreateCategory", dto);
        }

        TempData["SuccessMessage"] = $"تم إنشاء قسم «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction(nameof(Categories));
    }

    [HttpGet("EditCategory/{id:int?}")]
    public async Task<IActionResult> EditCategory(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category is null) return NotFound();

        return AdminView("EditCategory", new UpdateCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder
        });
    }

    [HttpPost("EditCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return AdminView("EditCategory", dto);

        var result = await _categoryService.UpdateCategoryAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر تحديث القسم");
            return AdminView("EditCategory", dto);
        }

        TempData["SuccessMessage"] = $"تم تحديث قسم «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost("DeleteCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Error ?? "تعذر حذف القسم";
        else
            TempData["SuccessMessage"] = "تم حذف القسم بنجاح ✓";

        return RedirectToAction(nameof(Categories));
    }
}
