using System.Text.RegularExpressions;
using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Manages flashcard pack CRUD, image uploads, and teacher voice audio recordings.</summary>
public sealed class AdminProductsController : AdminControllerBase
{
    private const string DefaultProductImage = "/images/cards/card-phonics-1.svg";

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IFileStorageService _fileStorageService;

    public AdminProductsController(
        IProductService productService,
        ICategoryService categoryService,
        IFileStorageService fileStorageService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("Products")]
    public async Task<IActionResult> Products(
        int? categoryId,
        string? status,
        string? search,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var allProducts = await _productService.GetAllProductsAsync(cancellationToken);
        var query = allProducts.AsEnumerable();

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "all" : status.Trim().ToLowerInvariant();
        query = normalizedStatus switch
        {
            "active" => query.Where(p => p.IsActive),
            "inactive" => query.Where(p => !p.IsActive),
            "lowstock" => query.Where(p => p.StockQuantity < 5),
            _ => query
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                (!string.IsNullOrEmpty(p.NameAr) && p.NameAr.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.GradeLevel) && p.GradeLevel.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.TargetAudience) && p.TargetAudience.Contains(term, StringComparison.OrdinalIgnoreCase))
            );
        }

        var filtered = query.ToList();
        var pagedResult = BuildPagedResult(filtered, page, pageSize, out var pagedProducts);

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.SelectedStatus = normalizedStatus;
        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Products?categoryId={categoryId}&status={normalizedStatus}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return AdminView("Products", pagedProducts);
    }

    [HttpGet("CreateProduct")]
    public async Task<IActionResult> CreateProduct(CancellationToken cancellationToken)
    {
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return AdminView("CreateProduct", new CreateProductDto
        {
            IsActive = true,
            StockQuantity = 20,
            CardCount = 30
        });
    }

    [HttpPost("CreateProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(
        CreateProductDto dto,
        List<IFormFile>? imageFiles,
        IFormFile? backImageFile,
        IFormFile? audioFile,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            dto.Slug = await GenerateUniqueSlugAsync(dto.Name, dto.NameAr, null, cancellationToken);
            ModelState.Remove(nameof(dto.Slug));
        }

        var uploadedPaths = await SaveUploadedImagesAsync(imageFiles, cancellationToken);
        if (uploadedPaths.Count > 0)
            dto.ImageUrl = string.Join(",", uploadedPaths);
        else if (string.IsNullOrWhiteSpace(dto.ImageUrl))
            dto.ImageUrl = DefaultProductImage;
        ModelState.Remove(nameof(dto.ImageUrl));

        var savedBackImage = await SaveSingleImageAsync(backImageFile, cancellationToken);
        if (savedBackImage is not null)
            dto.BackImageUrl = savedBackImage;
        ModelState.Remove(nameof(dto.BackImageUrl));

        var savedAudio = await SaveUploadedAudioAsync(audioFile, cancellationToken);
        if (savedAudio is not null)
            dto.AudioUrl = savedAudio;
        ModelState.Remove(nameof(dto.AudioUrl));

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return AdminView("CreateProduct", dto);
        }

        var result = await _productService.CreateProductAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر إضافة الباقة");
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return AdminView("CreateProduct", dto);
        }

        TempData["SuccessMessage"] = $"تمت إضافة باقة «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction(nameof(Products));
    }

    [HttpGet("EditProduct/{id:int?}")]
    public async Task<IActionResult> EditProduct(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null) return NotFound();

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return AdminView("EditProduct", new UpdateProductDto
        {
            Id = product.Id,
            Name = product.Name,
            NameAr = product.NameAr,
            Slug = product.Slug,
            Description = product.Description,
            DescriptionAr = product.DescriptionAr,
            TargetAudience = product.TargetAudience,
            GradeLevel = product.GradeLevel,
            CardCount = product.CardCount,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            BackImageUrl = product.BackImageUrl,
            AudioUrl = product.AudioUrl,
            BackPreviewText = product.BackPreviewText,
            BackKeyPoints = product.BackKeyPoints,
            BackKeyPointsEn = product.BackKeyPointsEn,
            SampleWords = product.SampleWords,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured
        });
    }

    [HttpPost("EditProduct/{id:int?}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(
        UpdateProductDto dto,
        List<IFormFile>? imageFiles,
        string? existingImages,
        IFormFile? backImageFile,
        string? existingBackImage,
        bool removeBackImage,
        IFormFile? audioFile,
        string? existingAudio,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            dto.Slug = await GenerateUniqueSlugAsync(dto.Name, dto.NameAr, dto.Id, cancellationToken);
            ModelState.Remove(nameof(dto.Slug));
        }

        var imagesList = (existingImages ?? string.Empty)
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        imagesList.AddRange(await SaveUploadedImagesAsync(imageFiles, cancellationToken));

        dto.ImageUrl = imagesList.Count > 0
            ? string.Join(",", imagesList)
            : (string.IsNullOrWhiteSpace(dto.ImageUrl) ? DefaultProductImage : dto.ImageUrl);
        ModelState.Remove(nameof(dto.ImageUrl));

        var savedBackImage = await SaveSingleImageAsync(backImageFile, cancellationToken);
        if (savedBackImage is not null)
            dto.BackImageUrl = savedBackImage;
        else if (removeBackImage)
            dto.BackImageUrl = null;
        else if (!string.IsNullOrWhiteSpace(existingBackImage))
            dto.BackImageUrl = existingBackImage;
        ModelState.Remove(nameof(dto.BackImageUrl));

        var savedAudio = await SaveUploadedAudioAsync(audioFile, cancellationToken);
        if (savedAudio is not null)
            dto.AudioUrl = savedAudio;
        else if (!string.IsNullOrWhiteSpace(existingAudio))
            dto.AudioUrl = existingAudio;
        ModelState.Remove(nameof(dto.AudioUrl));

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return AdminView("EditProduct", dto);
        }

        var result = await _productService.UpdateProductAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر تحديث الباقة");
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return AdminView("EditProduct", dto);
        }

        TempData["SuccessMessage"] = $"تم تحديث باقة «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction(nameof(Products));
    }

    [HttpPost("DeleteProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteProductAsync(id, cancellationToken);
        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Error ?? "تعذر حذف الباقة";
        else
            TempData["SuccessMessage"] = "تم حذف باقة الفلاش كاردز بنجاح ✓";

        return RedirectToAction(nameof(Products));
    }

    [HttpGet("ToggleProductStatus")]
    public IActionResult ToggleProductStatusRedirect() => RedirectToAction(nameof(Products));

    [HttpPost("ToggleProductStatus")]
    public async Task<IActionResult> ToggleProductStatus(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null) return NotFound();

        var updateDto = new UpdateProductDto
        {
            Id = product.Id,
            Name = product.Name,
            NameAr = product.NameAr,
            Slug = product.Slug,
            Description = product.Description,
            DescriptionAr = product.DescriptionAr,
            TargetAudience = product.TargetAudience,
            GradeLevel = product.GradeLevel,
            CardCount = product.CardCount,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            BackImageUrl = product.BackImageUrl,
            AudioUrl = product.AudioUrl,
            BackPreviewText = product.BackPreviewText,
            BackKeyPoints = product.BackKeyPoints,
            BackKeyPointsEn = product.BackKeyPointsEn,
            SampleWords = product.SampleWords,
            CategoryId = product.CategoryId,
            IsActive = !product.IsActive,
            IsFeatured = product.IsFeatured
        };

        var result = await _productService.UpdateProductAsync(updateDto, cancellationToken);
        if (result.Succeeded)
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString(updateDto.IsActive ? "تم تفعيل الباقة بالمتجر ✓" : "تم إيقاف عرض الباقة بالمتجر"));

        return AdminPartial("_ProductStatusBadge", updateDto.IsActive);
    }

    private async Task<List<string>> SaveUploadedImagesAsync(List<IFormFile>? imageFiles, CancellationToken ct)
    {
        var uploaded = new List<string>();
        if (imageFiles is null || imageFiles.Count == 0) return uploaded;

        foreach (var file in imageFiles)
        {
            if (file.Length <= 0 || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                continue;

            await using var stream = file.OpenReadStream();
            uploaded.Add(await _fileStorageService.SaveFileAsync(stream, file.FileName, "products", ct));
        }
        return uploaded;
    }

    private async Task<string?> SaveSingleImageAsync(IFormFile? imageFile, CancellationToken ct)
    {
        if (imageFile is null || imageFile.Length <= 0 || !imageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return null;

        await using var stream = imageFile.OpenReadStream();
        return await _fileStorageService.SaveFileAsync(stream, imageFile.FileName, "products", ct);
    }

    private async Task<string?> SaveUploadedAudioAsync(IFormFile? audioFile, CancellationToken ct)
    {
        if (audioFile is null || audioFile.Length <= 0) return null;

        var ext = Path.GetExtension(audioFile.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".webm";
        var fileName = audioFile.FileName.Contains('.') ? audioFile.FileName : $"{Guid.NewGuid():N}{ext}";

        await using var audioStream = audioFile.OpenReadStream();
        return await _fileStorageService.SaveFileAsync(audioStream, fileName, "audio", ct);
    }

    private async Task<string> GenerateUniqueSlugAsync(string? name, string? nameAr, int? currentProductId, CancellationToken ct)
    {
        var raw = !string.IsNullOrWhiteSpace(name) ? name : nameAr ?? "pack";
        var clean = Regex.Replace(raw, @"[^a-zA-Z0-9\s-]", "").Trim();
        clean = Regex.Replace(clean, @"\s+", "-").ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(clean)) clean = "pack";

        var candidate = clean;
        var counter = 1;
        while (true)
        {
            if (!await _productService.IsSlugTakenAsync(candidate, currentProductId, ct))
                return candidate;

            candidate = $"{clean}-{counter++}";
        }
    }
}
