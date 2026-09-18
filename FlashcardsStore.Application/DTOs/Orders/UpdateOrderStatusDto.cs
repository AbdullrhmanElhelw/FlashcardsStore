using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.DTOs.Orders;

public class UpdateOrderStatusDto
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string? ReasonOrNotes { get; set; }
}
