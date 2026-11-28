using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductCardViewModel>> GetCatalogCardsAsync(ProductFilterDto filter, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetCatalogAsync(
            filter.CategoryId,
            filter.SearchTerm,
            filter.GradeLevel,
            cancellationToken);

        var query = products.AsEnumerable();

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        var isRtl = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        query = filter.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name" => isRtl ? query.OrderBy(p => p.NameAr) : query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = query.Count();
        var pageIndex = Math.Max(1, filter.PageIndex);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var pagedItems = query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToCardViewModel)
            .ToList();

        return new PagedResult<ProductCardViewModel>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllWithCategoryAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<ProductDto>> GetAllProductsPagedAsync(int pageIndex, int pageSize, int? categoryId = null, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllWithCategoryAsync(cancellationToken);
        var query = products.AsEnumerable();

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var totalCount = query.Count();
        pageIndex = Math.Max(1, pageIndex);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var pagedItems = query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<ProductDetailDto?> GetProductDetailBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetBySlugAsync(slug, cancellationToken);
        if (product == null) return null;

        return MapToDetailDto(product);
    }

    public async Task<ProductDetailDto?> GetProductDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null) return null;

        return MapToDetailDto(product);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var descAr = dto.DescriptionAr?.Trim() ?? "";
        var descEn = dto.Description?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(descAr) && string.IsNullOrWhiteSpace(descEn))
        {
            return Result<ProductDto>.Failure("يرجى إدخال وصف للباقة باللغة العربية أو الإنجليزية على الأقل");
        }

        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            dto.Slug = !string.IsNullOrWhiteSpace(dto.Name)
                ? dto.Name.ToLowerInvariant().Replace(" ", "-").Replace("/", "-")
                : "pack-" + Guid.NewGuid().ToString("N")[..8];
        }

        if (await _productRepository.ExistsBySlugAsync(dto.Slug, null, cancellationToken))
        {
            return Result<ProductDto>.Failure("الرابط المخصص (Slug) مستخدم بالفعل لمنتج آخر");
        }

        var product = new Product
        {
            Name = string.IsNullOrWhiteSpace(dto.Name) ? (dto.NameAr ?? "") : dto.Name,
            NameAr = string.IsNullOrWhiteSpace(dto.NameAr) ? (dto.Name ?? "") : dto.NameAr,
            Slug = dto.Slug ?? string.Empty,
            Description = descEn,
            DescriptionAr = descAr,
            TargetAudience = dto.TargetAudience ?? string.Empty,
            GradeLevel = dto.GradeLevel ?? string.Empty,
            CardCount = dto.CardCount,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl ?? string.Empty,
            AudioUrl = dto.AudioUrl,
            BackPreviewText = dto.BackPreviewText ?? string.Empty,
            BackImageUrl = dto.BackImageUrl,
            BackKeyPoints = dto.BackKeyPoints ?? string.Empty,
            BackKeyPointsEn = dto.BackKeyPointsEn ?? string.Empty,
            SampleWords = dto.SampleWords ?? string.Empty,
            CategoryId = dto.CategoryId,
            IsActive = dto.IsActive,
            IsFeatured = dto.IsFeatured
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(MapToDto(product));
    }

    public async Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (product == null)
            return Result<ProductDto>.Failure("المنتج غير موجود");

        var descAr = dto.DescriptionAr?.Trim() ?? "";
        var descEn = dto.Description?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(descAr) && string.IsNullOrWhiteSpace(descEn))
        {
            return Result<ProductDto>.Failure("يرجى إدخال وصف للباقة باللغة العربية أو الإنجليزية على الأقل");
        }

        if (await _productRepository.ExistsBySlugAsync(dto.Slug, dto.Id, cancellationToken))
            return Result<ProductDto>.Failure("الرابط المخصص مستخدم بالفعل لمنتج آخر");

        product.Name = string.IsNullOrWhiteSpace(dto.Name) ? (dto.NameAr ?? product.Name) : dto.Name;
        product.NameAr = string.IsNullOrWhiteSpace(dto.NameAr) ? (dto.Name ?? product.NameAr) : dto.NameAr;
        product.Slug = dto.Slug ?? product.Slug;
        product.Description = descEn;
        product.DescriptionAr = descAr;
        product.TargetAudience = dto.TargetAudience ?? string.Empty;
        product.GradeLevel = dto.GradeLevel ?? string.Empty;
        product.CardCount = dto.CardCount;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.ImageUrl = dto.ImageUrl ?? product.ImageUrl;
        product.AudioUrl = dto.AudioUrl;
        product.BackPreviewText = dto.BackPreviewText ?? string.Empty;
        product.BackImageUrl = dto.BackImageUrl;
        product.BackKeyPoints = dto.BackKeyPoints ?? string.Empty;
        product.BackKeyPointsEn = dto.BackKeyPointsEn ?? string.Empty;
        product.SampleWords = dto.SampleWords ?? string.Empty;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;
        product.IsFeatured = dto.IsFeatured;

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(MapToDto(product));
    }

    public async Task<Result<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result<bool>.Failure("المنتج غير موجود");

        await _productRepository.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public Task<bool> IsSlugTakenAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
        => _productRepository.ExistsBySlugAsync(slug, excludeId, cancellationToken);

    public async Task<IReadOnlyList<ProductCardViewModel>> GetFeaturedCardsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetFeaturedProductsAsync(cancellationToken);
        return products.Select(MapToCardViewModel).ToList();
    }

    public async Task<IReadOnlyList<ProductCardViewModel>> GetCardsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (!idList.Any())
            return Array.Empty<ProductCardViewModel>();

        var all = await _productRepository.GetAllWithCategoryAsync(cancellationToken);
        return all
            .Where(p => idList.Contains(p.Id) && p.IsActive)
            .Select(MapToCardViewModel)
            .ToList();
    }

    private static ProductCardViewModel MapToCardViewModel(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        NameAr = p.NameAr,
        Slug = p.Slug,
        TargetAudience = p.TargetAudience,
        GradeLevel = p.GradeLevel,
        CardCount = p.CardCount,
        Price = p.Price,
        AvailableStockQuantity = p.AvailableStockQuantity,
        ImageUrl = p.ImageUrl,
        AudioUrl = p.AudioUrl,
        BackPreviewText = p.BackPreviewText,
        BackImageUrl = p.BackImageUrl,
        BackKeyPoints = p.BackKeyPoints,
        BackKeyPointsEn = p.BackKeyPointsEn,
        CategoryName = p.Category?.Name ?? string.Empty,
        CategoryNameAr = p.Category?.NameAr ?? string.Empty,
        SampleWords = PronunciationWordExtractor.GetWordsForProduct(p.Slug, p.Name, p.BackPreviewText, p.Category?.Name, p.SampleWords)
    };

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        NameAr = p.NameAr,
        Slug = p.Slug,
        Description = p.Description,
        DescriptionAr = p.DescriptionAr,
        TargetAudience = p.TargetAudience,
        GradeLevel = p.GradeLevel,
        CardCount = p.CardCount,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        ReservedStockQuantity = p.ReservedStockQuantity,
        AvailableStockQuantity = p.AvailableStockQuantity,
        ImageUrl = p.ImageUrl,
        AudioUrl = p.AudioUrl,
        BackPreviewText = p.BackPreviewText,
        BackImageUrl = p.BackImageUrl,
        BackKeyPoints = p.BackKeyPoints,
        BackKeyPointsEn = p.BackKeyPointsEn,
        SampleWords = p.SampleWords,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        CategoryNameAr = p.Category?.NameAr ?? string.Empty,
        IsActive = p.IsActive,
        IsFeatured = p.IsFeatured
    };

    private static ProductDetailDto MapToDetailDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        NameAr = p.NameAr,
        Slug = p.Slug,
        Description = p.Description,
        DescriptionAr = p.DescriptionAr,
        TargetAudience = p.TargetAudience,
        GradeLevel = p.GradeLevel,
        CardCount = p.CardCount,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        AvailableStockQuantity = p.AvailableStockQuantity,
        ImageUrl = p.ImageUrl,
        AudioUrl = p.AudioUrl,
        BackPreviewText = p.BackPreviewText,
        BackImageUrl = p.BackImageUrl,
        BackKeyPoints = p.BackKeyPoints,
        BackKeyPointsEn = p.BackKeyPointsEn,
        SampleWordsText = p.SampleWords,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        CategoryNameAr = p.Category?.NameAr ?? string.Empty,
        SampleWords = PronunciationWordExtractor.GetWordsForProduct(p.Slug, p.Name, p.BackPreviewText, p.Category?.Name, p.SampleWords)
    };
}
