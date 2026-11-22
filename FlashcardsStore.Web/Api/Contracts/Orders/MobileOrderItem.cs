using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Web.Api.Contracts.Orders;

/// <summary>Order line sent by the mobile app. Price is NEVER accepted from the client.</summary>
public sealed class MobileOrderItem
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 50)]
    public int Quantity { get; set; }
}
