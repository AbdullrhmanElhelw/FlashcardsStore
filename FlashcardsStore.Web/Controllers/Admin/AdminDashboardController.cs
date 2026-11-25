using FlashcardsStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Displays the Admin KPI dashboard and overview metrics.</summary>
public sealed class AdminDashboardController : AdminControllerBase
{
    private readonly IOrderService _orderService;

    public AdminDashboardController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => RedirectToAction(nameof(Dashboard));

    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var metrics = await _orderService.GetAdminDashboardMetricsAsync(cancellationToken);
        return AdminView("Dashboard", metrics);
    }
}
