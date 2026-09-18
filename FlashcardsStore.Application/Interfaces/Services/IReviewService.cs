using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Reviews;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetReviewsByProductIdAsync(int productId, bool approvedOnly = true);
    Task<ProductReviewSummaryDto> GetProductReviewSummaryAsync(int productId);
    Task<Result<ReviewDto>> AddReviewAsync(CreateReviewDto dto);
    Task<IReadOnlyList<ReviewDto>> GetAllReviewsAsync();
    Task<Result> ToggleApprovalAsync(int reviewId);
    Task<Result> DeleteReviewAsync(int reviewId);
}
