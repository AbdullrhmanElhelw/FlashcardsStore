using FlashcardsStore.Application.DTOs.Reviews;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Web.Api.Common;
using FlashcardsStore.Web.Api.Contracts.Reviews;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Manages product review listing and submission for the mobile app.</summary>
public sealed class MobileReviewsController : MobileApiControllerBase
{
    private readonly IReviewService _reviews;

    public MobileReviewsController(IReviewService reviews)
    {
        _reviews = reviews;
    }

    [HttpGet("products/{productId:int}/reviews")]
    public async Task<ActionResult<MobileProductReviewsResponse>> GetReviews(int productId)
    {
        var reviews = await _reviews.GetReviewsByProductIdAsync(productId);
        var summary = await _reviews.GetProductReviewSummaryAsync(productId);
        return Ok(new MobileProductReviewsResponse(reviews, summary));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("products/{productId:int}/reviews")]
    public async Task<IActionResult> AddReview(int productId, [FromBody] CreateReviewDto request)
    {
        if (request.ProductId != productId)
            return Fail("المنتج المحدد غير صحيح");

        var result = await _reviews.AddReviewAsync(request);
        return result.IsSuccess
            ? Ok(new MobileReviewCreatedResponse(true, result.Value))
            : Fail(result.Error ?? "تعذر إضافة التقييم");
    }
}
