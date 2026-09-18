using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Products;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IProductService
{
    Task<PagedResult<ProductCardViewModel>> GetCatalogCardsAsync(ProductFilterDto filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> GetAllProductsPagedAsync(int pageIndex, int pageSize, int? categoryId = null, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> GetProductDetailBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> GetProductDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductCardViewModel>> GetFeaturedCardsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductCardViewModel>> GetCardsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
