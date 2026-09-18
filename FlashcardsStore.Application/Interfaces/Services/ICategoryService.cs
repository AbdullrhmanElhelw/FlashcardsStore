using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Categories;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
}
