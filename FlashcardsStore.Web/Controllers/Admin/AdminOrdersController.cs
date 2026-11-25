using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Manages store and mobile orders, payment receipt verification, and fulfillment status.</summary>
public sealed class AdminOrdersController : AdminControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("Orders")]
    public async Task<IActionResult> Orders(
        OrderStatus? status,
        string? search,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<OrderSummaryDto> orders = status.HasValue
            ? await _orderService.GetOrdersByStatusAsync(status.Value, cancellationToken)
            : await _orderService.GetRecentOrdersAsync(500, cancellationToken);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            orders = orders.Where(o =>
                (!string.IsNullOrEmpty(o.OrderNumber) && o.OrderNumber.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.CustomerName) && o.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.CustomerPhone) && o.CustomerPhone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.Governorate) && o.Governorate.Contains(term, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        var pagedResult = BuildPagedResult(orders, page, pageSize, out var pagedOrders);

        ViewBag.CurrentStatus = status;
        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Orders?status={status}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return AdminView("Orders", pagedOrders);
    }

    [HttpGet("OrderDetails/{id:int?}")]
    public async Task<IActionResult> OrderDetails(int id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : AdminView("OrderDetails", order);
    }

    [HttpPost("AddOrderNote")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrderNote(int id, string notes, OrderStatus? newStatus, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order is null) return NotFound();

        var statusToSet = newStatus ?? order.Status;
        var result = await _orderService.UpdateOrderStatusAsync(id, statusToSet, CurrentAdminUser, notes, cancellationToken);
        if (result.Succeeded)
            TempData["SuccessMessage"] = "تم تحديث حالة الطلب وحفظ الملاحظات بنجاح ✓";
        else
            TempData["ErrorMessage"] = result.Error ?? "تعذر تحديث الطلب";

        return RedirectToAction(nameof(OrderDetails), new { id });
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        var result = await _orderService.ApproveOrderAsync(id, CurrentAdminUser, cancellationToken);
        if (!result.Succeeded)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString(result.Error ?? "تعذر اعتماد الطلب"));
            Response.Headers.Append("HX-Toast-Type", "error");
        }
        else
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString("تم اعتماد الطلب وخصم الكمية المحجوزة من المخزن بنجاح ✓"));
        }

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return PartialView("_OrderStatusBadge", order?.Status ?? OrderStatus.Approved);
    }

    [HttpPost("Reject")]
    public async Task<IActionResult> Reject(int id, string? reason, CancellationToken cancellationToken)
    {
        var result = await _orderService.RejectOrderAsync(id, CurrentAdminUser, reason, cancellationToken);
        if (!result.Succeeded)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString(result.Error ?? "تعذر رفض الطلب"));
            Response.Headers.Append("HX-Toast-Type", "error");
        }
        else
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString("تم رفض الطلب وإعادة الكمية المحجوزة للمخزن"));
        }

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return PartialView("_OrderStatusBadge", order?.Status ?? OrderStatus.Rejected);
    }

    [HttpPost("UpdateStatus")]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus, string? notes, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, newStatus, CurrentAdminUser, notes, cancellationToken);
        if (result.Succeeded)
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString($"تم تحديث حالة الطلب إلى {newStatus}"));

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return PartialView("_OrderStatusBadge", order?.Status ?? newStatus);
    }
}
