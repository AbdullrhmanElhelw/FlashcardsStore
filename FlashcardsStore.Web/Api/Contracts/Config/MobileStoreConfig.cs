namespace FlashcardsStore.Web.Api.Contracts.Config;

/// <summary>Public (non-sensitive) subset of store settings for the mobile app.</summary>
public sealed class MobileStoreConfig
{
    public string StoreNameAr { get; set; } = string.Empty;
    public string StoreNameEn { get; set; } = string.Empty;
    public string WhatsAppNumberInternational { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string VodafoneCashNumber { get; set; } = string.Empty;
    public string InstaPayAddress { get; set; } = string.Empty;
    public string InstaPayAccountName { get; set; } = string.Empty;
    public string? PaymentInstructionsAr { get; set; }
    public decimal ShippingFeeCairoGiza { get; set; }
    public decimal ShippingFeeDeltaCanal { get; set; }
    public decimal ShippingFeeUpperEgypt { get; set; }
    public decimal FreeShippingThreshold { get; set; }
    public string EstimatedDeliveryTimeAr { get; set; } = string.Empty;
    public bool AnnouncementBarEnabled { get; set; }
    public string AnnouncementTextAr { get; set; } = string.Empty;
    public string AnnouncementTextEn { get; set; } = string.Empty;
    public string? AnnouncementLink { get; set; }
}
