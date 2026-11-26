namespace FlashcardsStore.Mobile.Models.Api;

public class MyOrderSummary
{
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalItems { get; set; }
    public bool HasPaymentProof { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
    public string StatusDescriptionAr { get; set; } = string.Empty;
}
