using FlashcardsStore.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Displays registered mobile accounts and guest checkout customers.</summary>
public sealed class AdminCustomersController : AdminControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public AdminCustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet("Customers")]
    public async Task<IActionResult> Customers(
        string? search,
        string? filter = "all",
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var summary = await _customerRepository.GetAdminCustomersSummaryAsync(search, filter, cancellationToken);
        var pagedResult = BuildPagedResult(summary.Customers, page, pageSize, out var pagedCustomers);
        summary.Customers = pagedCustomers;

        ViewBag.PaginationBaseUrl = $"/Admin/Customers?filter={summary.FilterType}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return AdminView("Customers", summary);
    }
}
