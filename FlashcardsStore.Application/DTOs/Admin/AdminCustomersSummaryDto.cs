namespace FlashcardsStore.Application.DTOs.Admin;

public class AdminCustomersSummaryDto
{
    public IReadOnlyList<AdminCustomerListItemDto> Customers { get; set; } = Array.Empty<AdminCustomerListItemDto>();
    public int TotalCustomers { get; set; }
    public int MobileAccountCustomers { get; set; }
    public int GuestCustomers { get; set; }
    public string? SearchTerm { get; set; }
    public string FilterType { get; set; } = "all";
}
