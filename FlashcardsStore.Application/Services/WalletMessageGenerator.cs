using System.Text;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Application.Services;

public class WalletMessageGenerator : IWalletMessageGenerator
{
    public const string VodafoneCashNumber = "01099887766";
    public const string InstaPayAddress = "kholoud.elnady@instapay";
    public const string InstaPayAccountName = "Kholoud Elnady English Academy";

    public string GenerateTransferInstructions(OrderDetailDto order)
    {
        var sb = new StringBuilder();
        sb.AppendLine("تعليمات الدفع عبر المحافظ الإلكترونية وإنستاباي (مصر)");
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine($"رقم الطلب: {order.OrderNumber}");
        sb.AppendLine($"المبلغ المطلوب تحويله: {order.GrandTotal:F2} جنيه مصري (شامل مصاريف الشحن)");
        sb.AppendLine();
        sb.AppendLine("طرق الدفع المتاحة:");
        sb.AppendLine($"1. فودافون كاش / أورنج موني / اتصالات كاش / وي باي: {VodafoneCashNumber}");
        sb.AppendLine($"2. تطبيق إنستاباي (InstaPay): {InstaPayAddress}");
        sb.AppendLine($"   (اسم الحساب: {InstaPayAccountName})");
        sb.AppendLine();
        sb.AppendLine("ملاحظات هامة:");
        sb.AppendLine("• يتم حجز البطاقات مؤقتاً لمدة 24 ساعة في انتظار إرفاق صورة التحويل.");
        sb.AppendLine("• يرجى الاحتفاظ بلقطة الشاشة (Screenshot) أو إيصال التحويل ورفعها عبر صفحة الطلب.");
        sb.AppendLine("• في خانة الملاحظات أثناء التحويل، يرجى كتابة رقم الطلب لسرعة المطابقة والتأكيد.");

        return sb.ToString();
    }

    public string GetVodafoneCashNumber() => VodafoneCashNumber;

    public string GetInstaPayAddress() => InstaPayAddress;
}
