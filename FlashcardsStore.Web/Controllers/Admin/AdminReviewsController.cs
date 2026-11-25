using FlashcardsStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Moderates customer reviews submitted from the website and mobile app.</summary>
public sealed class AdminReviewsController : AdminControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("Reviews")]
    public async Task<IActionResult> Reviews(
        string? status = "all",
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var allReviews = await _reviewService.GetAllReviewsAsync();
        ViewBag.TotalReviewsCount = allReviews.Count;
        ViewBag.ApprovedReviewsCount = allReviews.Count(r => r.IsApproved);
        ViewBag.PendingReviewsCount = allReviews.Count(r => !r.IsApproved);
        ViewBag.AverageScore = allReviews.Count > 0 ? allReviews.Average(r => r.Rating).ToString("F1") : "5.0";

        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "all" : status.Trim().ToLowerInvariant();
        var query = allReviews.AsEnumerable();

        if (normalizedStatus == "approved") query = query.Where(r => r.IsApproved);
        else if (normalizedStatus == "pending") query = query.Where(r => !r.IsApproved);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r =>
                (!string.IsNullOrEmpty(r.CustomerName) && r.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(r.ProductName) && r.ProductName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(r.Comment) && r.Comment.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var filtered = query.ToList();
        var pagedResult = BuildPagedResult(filtered, page, pageSize, out var pagedReviews);

        ViewBag.SelectedStatus = normalizedStatus;
        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Reviews?status={normalizedStatus}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return AdminView("Reviews", pagedReviews);
    }

    [HttpPost("ToggleReviewApproval")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleReviewApproval(int id)
    {
        var result = await _reviewService.ToggleApprovalAsync(id);
        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.Error;
        else
            TempData["SuccessMessage"] = "تم تحديث حالة اعتماد التقييم بنجاح ✓";

        return RedirectToAction(nameof(Reviews));
    }

    [HttpPost("DeleteReview")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var result = await _reviewService.DeleteReviewAsync(id);
        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.Error;
        else
            TempData["SuccessMessage"] = "تم حذف التقييم بنجاح ✓";

        return RedirectToAction(nameof(Reviews));
    }
}
