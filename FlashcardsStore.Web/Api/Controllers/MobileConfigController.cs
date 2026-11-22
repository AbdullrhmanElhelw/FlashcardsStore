using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Web.Api.Common;
using FlashcardsStore.Web.Api.Contracts.Config;
using FlashcardsStore.Web.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Provides public store configuration and dynamic shipping quotes for the mobile app.</summary>
public sealed class MobileConfigController : MobileApiControllerBase
{
    private readonly IStoreSettingsService _settings;

    public MobileConfigController(IStoreSettingsService settings)
    {
        _settings = settings;
    }

    [HttpGet("config")]
    public async Task<ActionResult<MobileStoreConfig>> GetConfig(CancellationToken ct)
    {
        var storeSettings = await _settings.GetSettingsAsync(ct);
        var whatsApp = _settings.GetWhatsAppNumberInternational();
        return Ok(MobileStoreConfigMapper.ToConfig(storeSettings, whatsApp));
    }

    [HttpGet("shipping-fee")]
    public async Task<ActionResult<MobileShippingQuote>> GetShippingFee(
        [FromQuery] string governorate,
        [FromQuery] decimal subtotal,
        CancellationToken ct)
    {
        await _settings.GetSettingsAsync(ct); // warm cache
        var normalizedSubtotal = Math.Max(0, subtotal);
        var fee = _settings.CalculateShippingFee(governorate, normalizedSubtotal);

        return Ok(new MobileShippingQuote
        {
            ShippingFee = fee,
            Subtotal = subtotal,
            GrandTotal = subtotal + fee,
            IsFreeShipping = fee == 0
        });
    }
}
