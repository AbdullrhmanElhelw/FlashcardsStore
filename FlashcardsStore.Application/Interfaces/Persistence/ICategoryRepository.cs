using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default);
}
