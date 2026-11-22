namespace FlashcardsStore.Mobile.Models.Api;

public class ShippingQuote
{
    public decimal ShippingFee { get; set; }
    public decimal Subtotal { get; set; }
    public decimal GrandTotal { get; set; }
    public bool IsFreeShipping { get; set; }
}
