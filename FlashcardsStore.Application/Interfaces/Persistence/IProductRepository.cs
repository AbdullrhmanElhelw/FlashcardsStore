using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetCatalogAsync(int? categoryId = null, string? searchTerm = null, string? gradeLevel = null, CancellationToken cancellationToken = default);
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}
