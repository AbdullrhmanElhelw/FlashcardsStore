using System.Net;
using System.Text;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.Services;

public class WhatsAppLinkBuilder : IWhatsAppLinkBuilder
{
    private readonly IStoreSettingsService _settingsService;

    public WhatsAppLinkBuilder(IStoreSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public string BuildOrderLink(OrderDetailDto order)
    {
        var settings = _settingsService.GetCachedSettings();
        var teacherWhatsAppEgypt = _settingsService.GetWhatsAppNumberInternational();

        var sb = new StringBuilder();
        sb.AppendLine("أهلاً بحضرتك يا مس خلود،");
        sb.AppendLine($"أود تأكيد طلب بطاقات الفلاش كاردز التعليمية من {settings.StoreNameAr}:");
        sb.AppendLine();
        sb.AppendLine($"[رقم الطلب]: {order.OrderNumber}");
        sb.AppendLine($"[الاسم]: {order.CustomerName}");
        sb.AppendLine($"[الهاتف]: {order.CustomerPhone}");
        sb.AppendLine($"[العنوان]: {order.Governorate} - {order.ShippingAddress}");

        var typeStr = order.CustomerType switch
        {
            CustomerType.Parent => "ولي أمر (Parent)",
            CustomerType.Teacher => "معلم / معلمة (Teacher)",
            CustomerType.School => "مدرسة / حضانة (School/Nursery)",
            _ => "عميل"
        };
        sb.AppendLine($"[نوع المشتري]: {typeStr}");
        sb.AppendLine();
        sb.AppendLine("[المنتجات المطلوبة]:");

        foreach (var item in order.Items)
        {
            sb.AppendLine($"- {item.ProductName} ({item.ProductNameAr}) x {item.Quantity} = {item.SubTotal:F2} ج.م");
        }

        sb.AppendLine();
        sb.AppendLine($"[الشحن]: {order.ShippingFee:F2} ج.م");
        sb.AppendLine($"[الإجمالي النهائي]: {order.GrandTotal:F2} ج.م");
        sb.AppendLine();
        sb.AppendLine("برجاء إفادتي بميعاد التوصيل وتأكيد استلام الطلب. شكراً جزيلاً.");

        var encoded = WebUtility.UrlEncode(sb.ToString());
        return $"https://wa.me/{teacherWhatsAppEgypt}?text={encoded}";
    }

    public string BuildSupportLink(string? customMessage = null)
    {
        return _settingsService.GetWhatsAppLink(customMessage);
    }
}
