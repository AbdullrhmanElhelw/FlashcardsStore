using FlashcardsStore.Application.DTOs.Reviews;

namespace FlashcardsStore.Web.Api.Contracts.Reviews;

public sealed record MobileReviewCreatedResponse(
    bool Success,
    ReviewDto? Review);
