using System.ComponentModel.DataAnnotations;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Web.Api;

/// <summary>Order line sent by the mobile app. Price is NEVER accepted from the client.</summary>
public class MobileOrderItem
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 50)]
    public int Quantity { get; set; }
}

public class MobileCreateOrderRequest
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(20)]
    public string? WhatsAppNumber { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [Required, StringLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Governorate { get; set; } = string.Empty;

    public CustomerType CustomerType { get; set; } = CustomerType.Parent;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WalletTransfer;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required, MinLength(1), MaxLength(50)]
    public List<MobileOrderItem> Items { get; set; } = new();
}

/// <summary>Public (non-sensitive) subset of store settings for the app.</summary>
public class MobileStoreConfig
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

public class MobileShippingQuote
{
    public decimal ShippingFee { get; set; }
    public decimal Subtotal { get; set; }
    public decimal GrandTotal { get; set; }
    public bool IsFreeShipping { get; set; }
}
