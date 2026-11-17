using System.Text;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Application.Services;

public class WalletMessageGenerator : IWalletMessageGenerator
{
    private readonly IStoreSettingsService _settingsService;

    public WalletMessageGenerator(IStoreSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public string GenerateTransferInstructions(OrderDetailDto order)
    {
        var settings = _settingsService.GetCachedSettings();
        var vodafoneCash = settings.VodafoneCashNumber;
        var instaPay = settings.InstaPayAddress;
        var accountName = settings.InstaPayAccountName;

        var sb = new StringBuilder();
        sb.AppendLine("تعليمات الدفع عبر المحافظ الإلكترونية وإنستاباي (مصر)");
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine($"رقم الطلب: {order.OrderNumber}");
        sb.AppendLine($"المبلغ المطلوب تحويله: {order.GrandTotal:F2} جنيه مصري (شامل مصاريف الشحن)");
        sb.AppendLine();
        sb.AppendLine("طرق الدفع المتاحة:");
        sb.AppendLine($"1. فودافون كاش / أورنج موني / اتصالات كاش / وي باي: {vodafoneCash}");
        sb.AppendLine($"2. تطبيق إنستاباي (InstaPay): {instaPay}");
        sb.AppendLine($"   (اسم الحساب: {accountName})");
        sb.AppendLine();
        sb.AppendLine("ملاحظات هامة:");
        if (!string.IsNullOrWhiteSpace(settings.PaymentInstructionsAr))
        {
            sb.AppendLine(settings.PaymentInstructionsAr);
        }
        else
        {
            sb.AppendLine("• يتم حجز البطاقات مؤقتاً لمدة 24 ساعة في انتظار إرفاق صورة التحويل.");
            sb.AppendLine("• يرجى الاحتفاظ بلقطة الشاشة (Screenshot) أو إيصال التحويل ورفعها عبر صفحة الطلب.");
            sb.AppendLine("• في خانة الملاحظات أثناء التحويل، يرجى كتابة رقم الطلب لسرعة المطابقة والتأكيد.");
        }

        return sb.ToString();
    }

    public string GetVodafoneCashNumber() => _settingsService.GetCachedSettings().VodafoneCashNumber;

    public string GetInstaPayAddress() => _settingsService.GetCachedSettings().InstaPayAddress;
}
