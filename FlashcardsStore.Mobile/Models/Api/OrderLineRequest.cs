namespace FlashcardsStore.Mobile.Models.Api;

/// <summary>Only product id + quantity are sent; the server owns pricing.</summary>
public class OrderLineRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
