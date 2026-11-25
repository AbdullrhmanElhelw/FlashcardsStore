using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Application.DTOs.Settings;

public class StoreSettingsDto
{
    public int Id { get; set; }

    // Contact Information
    [Required(ErrorMessage = "اسم المتجر بالعربية مطلوب")]
    [Display(Name = "اسم المتجر (بالعربية)")]
    public string StoreNameAr { get; set; } = "أكاديمية خلود النادي للغة الإنجليزية";

    [Required(ErrorMessage = "اسم المتجر بالإنجليزية مطلوب")]
    [Display(Name = "اسم المتجر (بالإنجليزية)")]
    public string StoreNameEn { get; set; } = "Kholoud Elnady English Academy";

    [Required(ErrorMessage = "رقم الواتساب مطلوب")]
    [RegularExpression(@"^\+?[0-9\s\-]{8,20}$", ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
    [Display(Name = "رقم الواتساب")]
    public string WhatsAppNumber { get; set; } = "01099887766";

    [Required(ErrorMessage = "رقم خدمة العملاء مطلوب")]
    [RegularExpression(@"^\+?[0-9\s\-]{8,20}$", ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
    [Display(Name = "رقم هاتف الدعم / خدمة العملاء")]
    public string ContactPhone { get; set; } = "01099887766";

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
    [Display(Name = "البريد الإلكتروني للمتجر")]
    public string ContactEmail { get; set; } = "info@kholoudelnady-academy.com";

    [Required(ErrorMessage = "عنوان الأكاديمية بالعربية مطلوب")]
    [Display(Name = "عنوان المقر (بالعربية)")]
    public string StoreAddressAr { get; set; } = "القاهرة - المعادي / الدقي، جمهورية مصر العربية";

    [Display(Name = "عنوان المقر (بالإنجليزية)")]
    public string StoreAddressEn { get; set; } = "Cairo, Egypt";

    // Payment details
    [Required(ErrorMessage = "رقم محفظة فودافون كاش مطلوب")]
    [Display(Name = "رقم محفظة فودافون كاش / المحافظ الإلكترونية")]
    public string VodafoneCashNumber { get; set; } = "01099887766";

    [Required(ErrorMessage = "معرف إنستاباي مطلوب")]
    [Display(Name = "عنوان أو معرف إنستاباي (InstaPay Address)")]
    public string InstaPayAddress { get; set; } = "kholoud.elnady@instapay";

    [Required(ErrorMessage = "اسم صاحب حساب إنستاباي مطلوب")]
    [Display(Name = "اسم صاحب الحساب في إنستاباي")]
    public string InstaPayAccountName { get; set; } = "Kholoud Elnady English Academy";

    [Display(Name = "تعليمات وتوجيهات الدفع الإضافية للعميل")]
    public string? PaymentInstructionsAr { get; set; }

    // Shipping Tiers & Delivery
    [Range(0, 10000, ErrorMessage = "قيمة الشحن يجب أن تكون رقماً موجباً")]
    [Display(Name = "شحن القاهرة والجيزة (ج.م)")]
    public decimal ShippingFeeCairoGiza { get; set; } = 50.00m;

    [Range(0, 10000, ErrorMessage = "قيمة الشحن يجب أن تكون رقماً موجباً")]
    [Display(Name = "شحن الإسكندرية والوجه البحري والقناة (ج.م)")]
    public decimal ShippingFeeDeltaCanal { get; set; } = 60.00m;

    [Range(0, 10000, ErrorMessage = "قيمة الشحن يجب أن تكون رقماً موجباً")]
    [Display(Name = "شحن الصعيد ومحافظات الحدود (ج.م)")]
    public decimal ShippingFeeUpperEgypt { get; set; } = 75.00m;

    [Range(0, 100000, ErrorMessage = "الحد الأدنى للشحن المجاني يجب أن يكون رقماً موجباً")]
    [Display(Name = "الحد الأدنى للشحن المجاني (ج.م) - ضع 0 لإلغاء الشحن المجاني")]
    public decimal FreeShippingThreshold { get; set; } = 500.00m;

    [Display(Name = "مدة التوصيل التقديرية")]
    public string EstimatedDeliveryTimeAr { get; set; } = "التوصيل السريع خلال 48 إلى 72 ساعة لجميع محافظات مصر";

    // Top Announcement Bar
    [Display(Name = "تفعيل شريط الإعلانات العلوي")]
    public bool AnnouncementBarEnabled { get; set; } = true;

    [Display(Name = "نص الإعلان (بالعربية)")]
    public string AnnouncementTextAr { get; set; } = "🎉 عروض التأسيس: شحن مجاني لأي طلب بقيمة 500 جنيه أو أكثر لجميع المحافظات! 🚚";

    [Display(Name = "نص الإعلان (بالإنجليزية)")]
    public string AnnouncementTextEn { get; set; } = "🎉 Foundation Offer: Free shipping on orders over 500 EGP to all Egyptian governorates! 🚚";

    [Display(Name = "رابط التوجيه عند النقر على الإعلان (اختياري)")]
    public string? AnnouncementLink { get; set; } = "/Catalog";
}
