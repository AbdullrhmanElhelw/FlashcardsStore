using FlashcardsStore.Application.DTOs.Reviews;

namespace FlashcardsStore.Web.Api.Contracts.Reviews;

public sealed record MobileProductReviewsResponse(
    IEnumerable<ReviewDto> Reviews,
    ProductReviewSummaryDto Summary);
