using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Products)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
