using System.Net;
using System.Text.RegularExpressions;
using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Settings;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Services;

public class StoreSettingsService : IStoreSettingsService
{
    private readonly IStoreSettingsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    // Fast memory cache shared across requests for near-zero latency in views
    private static StoreSettingsDto? _cachedSettings;
    private static readonly object _cacheLock = new();

    public StoreSettingsService(IStoreSettingsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StoreSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        var entity = await _repository.GetCurrentSettingsAsync(cancellationToken);
        if (entity == null)
        {
            var fallback = new StoreSettingsDto();
            lock (_cacheLock)
            {
                _cachedSettings = fallback;
            }
            return fallback;
        }

        var dto = MapToDto(entity);
        lock (_cacheLock)
        {
            _cachedSettings = dto;
        }
        return dto;
    }

    public StoreSettingsDto GetCachedSettings()
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        // Synchronous fallback if not yet loaded
        try
        {
            var entity = _repository.GetCurrentSettingsAsync().GetAwaiter().GetResult();
            if (entity != null)
            {
                var dto = MapToDto(entity);
                lock (_cacheLock)
                {
                    _cachedSettings = dto;
                }
                return dto;
            }
        }
        catch
        {
            // Fallback to defaults on database startup delay
        }

        var defaults = new StoreSettingsDto();
        lock (_cacheLock)
        {
            _cachedSettings ??= defaults;
        }
        return _cachedSettings;
    }

    public async Task<Result<StoreSettingsDto>> UpdateSettingsAsync(StoreSettingsDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            return Result<StoreSettingsDto>.Failure("بيانات الإعدادات غير صالحة");
        }

        var entity = await _repository.GetCurrentSettingsAsync(cancellationToken);
        if (entity == null)
        {
            entity = new StoreSettings();
            MapFromDto(dto, entity);
            entity.CreatedBy = updatedBy;
            entity.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(entity, cancellationToken);
        }
        else
        {
            MapFromDto(dto, entity);
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedDto = MapToDto(entity);
        lock (_cacheLock)
        {
            _cachedSettings = updatedDto;
        }

        return Result<StoreSettingsDto>.Success(updatedDto);
    }

    public decimal CalculateShippingFee(string? governorate, decimal subtotal)
    {
        var settings = GetCachedSettings();

        // Free shipping condition
        if (settings.FreeShippingThreshold > 0 && subtotal >= settings.FreeShippingThreshold)
        {
            return 0m;
        }

        var gov = governorate?.Trim() ?? "";

        // Cairo & Giza
        if (gov.Contains("القاهرة") || gov.Contains("الجيزة") ||
            gov.Equals("Cairo", StringComparison.OrdinalIgnoreCase) ||
            gov.Equals("Giza", StringComparison.OrdinalIgnoreCase))
        {
            return settings.ShippingFeeCairoGiza;
        }

        // Delta, Alexandria & Canal Zone
        if (gov.Contains("الإسكندرية") || gov.Contains("القليوبية") || gov.Contains("الشرقية") ||
            gov.Contains("الدقهلية") || gov.Contains("الغربية") || gov.Contains("المنوفية") ||
            gov.Contains("البحيرة") || gov.Contains("دمياط") || gov.Contains("كفر الشيخ") ||
            gov.Contains("بورسعيد") || gov.Contains("الإسماعيلية") || gov.Contains("السويس") ||
            gov.Equals("Alexandria", StringComparison.OrdinalIgnoreCase) ||
            gov.Equals("Port Said", StringComparison.OrdinalIgnoreCase) ||
            gov.Equals("Ismailia", StringComparison.OrdinalIgnoreCase) ||
            gov.Equals("Suez", StringComparison.OrdinalIgnoreCase))
        {
            return settings.ShippingFeeDeltaCanal;
        }

        // Upper Egypt & Border Governorates
        return settings.ShippingFeeUpperEgypt;
    }

    public string GetWhatsAppNumberInternational()
    {
        var raw = GetCachedSettings().WhatsAppNumber?.Trim() ?? "01099887766";
        var digits = Regex.Replace(raw, @"\D", "");

        if (digits.StartsWith("20"))
        {
            return digits;
        }

        if (digits.StartsWith("01"))
        {
            return "2" + digits;
        }

        if (digits.StartsWith("1"))
        {
            return "20" + digits;
        }

        return !string.IsNullOrEmpty(digits) ? digits : "201099887766";
    }

    public string GetWhatsAppLink(string? customMessage = null)
    {
        var intlNumber = GetWhatsAppNumberInternational();
        var msg = customMessage ?? "مرحباً مس خلود، لدي استفسار بخصوص بطاقات الفلاش كاردز التعليمية للأطفال من الأكاديمية.";
        var encoded = WebUtility.UrlEncode(msg);
        return $"https://wa.me/{intlNumber}?text={encoded}";
    }

    private static StoreSettingsDto MapToDto(StoreSettings s) => new()
    {
        Id = s.Id,
        StoreNameAr = s.StoreNameAr,
        StoreNameEn = s.StoreNameEn,
        WhatsAppNumber = s.WhatsAppNumber,
        ContactPhone = s.ContactPhone,
        ContactEmail = s.ContactEmail,
        StoreAddressAr = s.StoreAddressAr,
        StoreAddressEn = s.StoreAddressEn,
        VodafoneCashNumber = s.VodafoneCashNumber,
        InstaPayAddress = s.InstaPayAddress,
        InstaPayAccountName = s.InstaPayAccountName,
        PaymentInstructionsAr = s.PaymentInstructionsAr,
        ShippingFeeCairoGiza = s.ShippingFeeCairoGiza,
        ShippingFeeDeltaCanal = s.ShippingFeeDeltaCanal,
        ShippingFeeUpperEgypt = s.ShippingFeeUpperEgypt,
        FreeShippingThreshold = s.FreeShippingThreshold,
        EstimatedDeliveryTimeAr = s.EstimatedDeliveryTimeAr,
        AnnouncementBarEnabled = s.AnnouncementBarEnabled,
        AnnouncementTextAr = s.AnnouncementTextAr,
        AnnouncementTextEn = s.AnnouncementTextEn,
        AnnouncementLink = s.AnnouncementLink
    };

    private static void MapFromDto(StoreSettingsDto dto, StoreSettings s)
    {
        s.StoreNameAr = dto.StoreNameAr;
        s.StoreNameEn = dto.StoreNameEn;
        s.WhatsAppNumber = dto.WhatsAppNumber;
        s.ContactPhone = dto.ContactPhone;
        s.ContactEmail = dto.ContactEmail;
        s.StoreAddressAr = dto.StoreAddressAr;
        s.StoreAddressEn = dto.StoreAddressEn;
        s.VodafoneCashNumber = dto.VodafoneCashNumber;
        s.InstaPayAddress = dto.InstaPayAddress;
        s.InstaPayAccountName = dto.InstaPayAccountName;
        s.PaymentInstructionsAr = dto.PaymentInstructionsAr;
        s.ShippingFeeCairoGiza = dto.ShippingFeeCairoGiza;
        s.ShippingFeeDeltaCanal = dto.ShippingFeeDeltaCanal;
        s.ShippingFeeUpperEgypt = dto.ShippingFeeUpperEgypt;
        s.FreeShippingThreshold = dto.FreeShippingThreshold;
        s.EstimatedDeliveryTimeAr = dto.EstimatedDeliveryTimeAr;
        s.AnnouncementBarEnabled = dto.AnnouncementBarEnabled;
        s.AnnouncementTextAr = dto.AnnouncementTextAr;
        s.AnnouncementTextEn = dto.AnnouncementTextEn;
        s.AnnouncementLink = dto.AnnouncementLink;
    }
}
