using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> GetCatalogAsync(
        int? categoryId = null,
        string? searchTerm = null,
        string? gradeLevel = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(gradeLevel) && !gradeLevel.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            var gl = gradeLevel.Trim().ToLower();
            if (gl == "kg")
            {
                query = query.Where(p =>
                    p.GradeLevel.ToLower() == "all" ||
                    p.GradeLevel.ToLower().Contains("kg") ||
                    p.GradeLevel.ToLower().Contains("pre-k") ||
                    p.TargetAudience.ToLower().Contains("kg") ||
                    p.TargetAudience.ToLower().Contains("toddler") ||
                    p.TargetAudience.ToLower().Contains("روضة") ||
                    p.TargetAudience.ToLower().Contains("تأسيس"));
            }
            else if (gl == "primary-1-3" || gl == "primary-early")
            {
                query = query.Where(p =>
                    p.GradeLevel.ToLower() == "all" ||
                    p.GradeLevel.ToLower().Contains("primary-1") ||
                    p.GradeLevel.ToLower().Contains("primary-2") ||
                    p.GradeLevel.ToLower().Contains("primary-3") ||
                    p.GradeLevel.ToLower().Contains("early") ||
                    p.GradeLevel.ToLower().Contains("primary 1") ||
                    p.GradeLevel.ToLower().Contains("primary 2") ||
                    p.GradeLevel.ToLower().Contains("primary 3") ||
                    p.GradeLevel.ToLower().Contains("pre-k to primary 3") ||
                    p.TargetAudience.ToLower().Contains("الصفوف 1-3") ||
                    p.TargetAudience.ToLower().Contains("primary 1") ||
                    p.TargetAudience.ToLower().Contains("primary 2") ||
                    p.TargetAudience.ToLower().Contains("primary 3"));
            }
            else if (gl == "primary-4-6" || gl == "primary-upper")
            {
                query = query.Where(p =>
                    p.GradeLevel.ToLower() == "all" ||
                    p.GradeLevel.ToLower().Contains("primary-4-6") ||
                    p.GradeLevel.ToLower().Contains("4") ||
                    p.GradeLevel.ToLower().Contains("5") ||
                    p.GradeLevel.ToLower().Contains("6") ||
                    p.GradeLevel.ToLower().Contains("primary 1 - 4") ||
                    p.GradeLevel.ToLower().Contains("primary 2 - 6") ||
                    p.GradeLevel.ToLower().Contains("primary 3 - prep 3") ||
                    p.TargetAudience.ToLower().Contains("الصفوف 4-6") ||
                    p.TargetAudience.ToLower().Contains("primary 4") ||
                    p.TargetAudience.ToLower().Contains("primary 5") ||
                    p.TargetAudience.ToLower().Contains("primary 6"));
            }
            else if (gl == "prep")
            {
                query = query.Where(p =>
                    p.GradeLevel.ToLower() == "all" ||
                    p.GradeLevel.ToLower().Contains("prep") ||
                    p.TargetAudience.ToLower().Contains("prep") ||
                    p.TargetAudience.ToLower().Contains("إعدادي"));
            }
            else
            {
                query = query.Where(p =>
                    p.GradeLevel.ToLower().Contains(gl) ||
                    p.TargetAudience.ToLower().Contains(gl));
            }
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.NameAr.ToLower().Contains(term) ||
                p.Description.ToLower().Contains(term) ||
                p.DescriptionAr.ToLower().Contains(term) ||
                p.TargetAudience.ToLower().Contains(term));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.IsFeatured)
            .Take(8)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return excludeId.HasValue
            ? await _dbSet.AnyAsync(p => p.Slug == slug && p.Id != excludeId.Value, cancellationToken)
            : await _dbSet.AnyAsync(p => p.Slug == slug, cancellationToken);
    }
}
