using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Settings;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IStoreSettingsService
{
    Task<StoreSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default);
    StoreSettingsDto GetCachedSettings();
    Task<Result<StoreSettingsDto>> UpdateSettingsAsync(StoreSettingsDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);
    decimal CalculateShippingFee(string? governorate, decimal subtotal);
    string GetWhatsAppLink(string? customMessage = null);
    string GetWhatsAppNumberInternational();
}
