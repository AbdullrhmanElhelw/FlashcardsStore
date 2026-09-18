using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Categories;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetCategoriesWithProductsAsync(cancellationToken);
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            NameAr = c.NameAr,
            Slug = c.Slug,
            Description = c.Description,
            DisplayOrder = c.DisplayOrder,
            ProductCount = c.Products?.Count(p => !p.IsDeleted) ?? 0
        }).OrderBy(c => c.DisplayOrder).ToList();
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetBySlugAsync(slug, cancellationToken);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            ProductCount = category.Products?.Count(p => !p.IsDeleted) ?? 0
        };
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            ProductCount = 0
        };
    }

    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetBySlugAsync(dto.Slug, cancellationToken);
        if (existing != null)
        {
            return Result<CategoryDto>.Failure("الرابط المخصص (Slug) مستخدم بالفعل لقسم آخر");
        }

        var category = new Category
        {
            Name = dto.Name.Trim(),
            NameAr = dto.NameAr.Trim(),
            Slug = dto.Slug.Trim().ToLowerInvariant(),
            Description = dto.Description?.Trim() ?? string.Empty,
            DisplayOrder = dto.DisplayOrder
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            ProductCount = 0
        });
    }

    public async Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (category == null)
        {
            return Result<CategoryDto>.Failure("القسم غير موجود");
        }

        var existingSlug = await _categoryRepository.GetBySlugAsync(dto.Slug, cancellationToken);
        if (existingSlug != null && existingSlug.Id != dto.Id)
        {
            return Result<CategoryDto>.Failure("الرابط المخصص (Slug) مستخدم بالفعل لقسم آخر");
        }

        category.Name = dto.Name.Trim();
        category.NameAr = dto.NameAr.Trim();
        category.Slug = dto.Slug.Trim().ToLowerInvariant();
        category.Description = dto.Description?.Trim() ?? string.Empty;
        category.DisplayOrder = dto.DisplayOrder;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            ProductCount = 0
        });
    }

    public async Task<Result<bool>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return Result<bool>.Failure("القسم غير موجود");
        }

        var withProducts = await _categoryRepository.GetCategoriesWithProductsAsync(cancellationToken);
        var target = withProducts.FirstOrDefault(c => c.Id == id);
        if (target != null && target.Products.Any(p => !p.IsDeleted))
        {
            return Result<bool>.Failure("لا يمكن حذف القسم لوجود بطاقات فلاش كاردز مرتبطة به. يرجى نقل البطاقات أولاً.");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
