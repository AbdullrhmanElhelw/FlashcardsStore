using FlashcardsStore.Application.DTOs.Settings;
using FlashcardsStore.Web.Api.Contracts.Config;

namespace FlashcardsStore.Web.Api.Mappers;

/// <summary>Maps store settings DTOs to public mobile store configuration contracts.</summary>
public static class MobileStoreConfigMapper
{
    public static MobileStoreConfig ToConfig(StoreSettingsDto settings, string whatsAppNumberInternational)
        => new()
        {
            StoreNameAr = settings.StoreNameAr,
            StoreNameEn = settings.StoreNameEn,
            WhatsAppNumberInternational = whatsAppNumberInternational,
            ContactPhone = settings.ContactPhone,
            ContactEmail = settings.ContactEmail,
            VodafoneCashNumber = settings.VodafoneCashNumber,
            InstaPayAddress = settings.InstaPayAddress,
            InstaPayAccountName = settings.InstaPayAccountName,
            PaymentInstructionsAr = settings.PaymentInstructionsAr,
            ShippingFeeCairoGiza = settings.ShippingFeeCairoGiza,
            ShippingFeeDeltaCanal = settings.ShippingFeeDeltaCanal,
            ShippingFeeUpperEgypt = settings.ShippingFeeUpperEgypt,
            FreeShippingThreshold = settings.FreeShippingThreshold,
            EstimatedDeliveryTimeAr = settings.EstimatedDeliveryTimeAr,
            AnnouncementBarEnabled = settings.AnnouncementBarEnabled,
            AnnouncementTextAr = settings.AnnouncementTextAr,
            AnnouncementTextEn = settings.AnnouncementTextEn,
            AnnouncementLink = settings.AnnouncementLink
        };
}
