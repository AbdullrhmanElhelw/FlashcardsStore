using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Web.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Track(string? orderNumber, string? phone, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(orderNumber) && !string.IsNullOrWhiteSpace(phone))
        {
            var result = await _orderService.TrackOrderAsync(orderNumber, phone, cancellationToken);
            ViewBag.TrackingResult = result;
            ViewBag.Searched = true;
        }

        ViewBag.OrderNumber = orderNumber;
        ViewBag.Phone = phone;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TrackLookup(string orderNumber, string phone, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(phone))
        {
            ViewBag.ErrorMessage = "يرجى إدخال رقم الطلب ورقم الهاتف";
            return PartialView("_OrderTrackResult", null);
        }

        var result = await _orderService.TrackOrderAsync(orderNumber, phone, cancellationToken);
        if (result == null)
        {
            ViewBag.ErrorMessage = "عفواً، لم يتم العثور على طلب يطابق هذا الرقم ورقم الهاتف المسجل";
        }

        return PartialView("_OrderTrackResult", result);
    }
}
