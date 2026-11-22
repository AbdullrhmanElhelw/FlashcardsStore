using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

public class StoreSettings : BaseEntity
{
    // Contact Info
    public string StoreNameAr { get; set; } = "أكاديمية خلود النادي للغة الإنجليزية";
    public string StoreNameEn { get; set; } = "Kholoud Elnady English Academy";
    public string WhatsAppNumber { get; set; } = "01099887766";
    public string ContactPhone { get; set; } = "01099887766";
    public string ContactEmail { get; set; } = "info@kholoudelnady-academy.com";
    public string StoreAddressAr { get; set; } = "القاهرة - المعادي / الدقي، جمهورية مصر العربية";
    public string StoreAddressEn { get; set; } = "Cairo, Egypt";

    // Payment Details
    public string VodafoneCashNumber { get; set; } = "01099887766";
    public string InstaPayAddress { get; set; } = "kholoud.elnady@instapay";
    public string InstaPayAccountName { get; set; } = "Kholoud Elnady English Academy";
    public string? PaymentInstructionsAr { get; set; }

    // Egyptian Shipping Tiers & Delivery
    public decimal ShippingFeeCairoGiza { get; set; } = 50.00m;
    public decimal ShippingFeeDeltaCanal { get; set; } = 60.00m;
    public decimal ShippingFeeUpperEgypt { get; set; } = 75.00m;
    public decimal FreeShippingThreshold { get; set; } = 500.00m;
    public string EstimatedDeliveryTimeAr { get; set; } = "التوصيل السريع خلال 48 إلى 72 ساعة لجميع محافظات مصر";

    // Top Announcement Bar
    public bool AnnouncementBarEnabled { get; set; } = true;
    public string AnnouncementTextAr { get; set; } = "🎉 عروض التأسيس: شحن مجاني لأي طلب بقيمة 500 جنيه أو أكثر لجميع المحافظات! 🚚";
    public string AnnouncementTextEn { get; set; } = "🎉 Foundation Offer: Free shipping on orders over 500 EGP to all Egyptian governorates! 🚚";
    public string? AnnouncementLink { get; set; } = "/Catalog";
}
