using FlashcardsStore.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>
/// Shared base controller for all Admin CMS controllers.
/// Enforces Administrator authorization, /Admin routing, and shared view/pagination helpers.
/// </summary>
[Authorize(Roles = "Administrator")]
[Route("Admin")]
public abstract class AdminControllerBase : Controller
{
    protected string CurrentAdminUser => User.Identity?.Name ?? "admin";

    protected ViewResult AdminView(string viewName, object? model = null)
        => View($"~/Views/Admin/{viewName}.cshtml", model);

    protected PartialViewResult AdminPartial(string partialName, object? model = null)
        => PartialView($"~/Views/Admin/{partialName}.cshtml", model);

    protected static PagedResult<T> BuildPagedResult<T>(
        IReadOnlyCollection<T> allItems,
        int page,
        int pageSize,
        out List<T> pagedItems)
    {
        var safeSize = Math.Clamp(pageSize, 5, 100);
        var safePage = Math.Max(1, page);
        pagedItems = allItems.Skip((safePage - 1) * safeSize).Take(safeSize).ToList();
        return new PagedResult<T>
        {
            Items = pagedItems,
            TotalCount = allItems.Count,
            PageIndex = safePage,
            PageSize = safeSize
        };
    }
}
