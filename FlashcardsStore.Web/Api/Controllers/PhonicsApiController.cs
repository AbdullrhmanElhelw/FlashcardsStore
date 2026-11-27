using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Public activation and verification endpoints for the standalone Phonics Game mobile app.</summary>
[ApiController]
[IgnoreAntiforgeryToken]
[Route("api/phonics")]
public sealed class PhonicsApiController : ControllerBase
{
    private static readonly HashSet<string> AllowedPacks = new(StringComparer.OrdinalIgnoreCase)
    {
        "curriculum", "stories", "sentences", "rhymes", "alphabet"
    };

    private readonly FlashcardsStoreDbContext _db;
    private readonly IWebHostEnvironment _env;

    public PhonicsApiController(FlashcardsStoreDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public sealed record PhonicsActivateRequest(string Code, string DeviceId, string? DeviceName);

    public sealed record PhonicsVerifyRequest(string Code, string DeviceId);

    public sealed record PhonicsActivationResponse(
        bool IsActivated,
        string Code,
        string? BundleName,
        int MaxDevices,
        int UsedDevices,
        int RemainingDevices,
        string MessageAr,
        string MessageEn);

    [HttpPost("activate")]
    public async Task<ActionResult<PhonicsActivationResponse>> Activate(
        [FromBody] PhonicsActivateRequest request,
        CancellationToken ct)
    {
        var normalizedCode = (request.Code ?? string.Empty).Trim().ToUpperInvariant();
        var deviceId = (request.DeviceId ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return BadRequest(new PhonicsActivationResponse(
                false, string.Empty, null, 0, 0, 0,
                "من فضلك أدخل كود التفعيل.",
                "Please enter an activation code."));
        }

        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return BadRequest(new PhonicsActivationResponse(
                false, normalizedCode, null, 0, 0, 0,
                "معرّف الجهاز غير صالح.",
                "Invalid device identifier."));
        }

        var entity = await _db.PhonicsActivationCodes
            .Include(c => c.Devices)
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, ct);

        if (entity is null)
        {
            return NotFound(new PhonicsActivationResponse(
                false, normalizedCode, null, 0, 0, 0,
                "كود التفعيل غير صحيح. تأكد من الحروف والأرقام وحاول مرة أخرى.",
                "Invalid activation code. Please check and try again."));
        }

        if (!entity.IsActive)
        {
            return BadRequest(new PhonicsActivationResponse(
                false, entity.Code, entity.BundleName, entity.MaxDevices, entity.Devices.Count, 0,
                "تم إيقاف هذا الكود حالياً. تواصل معنا عبر واتساب.",
                "This activation code is currently disabled."));
        }

        if (entity.ExpiresAtUtc.HasValue && entity.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            return BadRequest(new PhonicsActivationResponse(
                false, entity.Code, entity.BundleName, entity.MaxDevices, entity.Devices.Count, 0,
                "انتهت صلاحية هذا الكود.",
                "This activation code has expired."));
        }

        var existingDevice = entity.Devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));

        if (existingDevice is not null)
        {
            existingDevice.LastVerifiedAtUtc = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.DeviceName))
                existingDevice.DeviceName = request.DeviceName.Trim();

            await _db.SaveChangesAsync(ct);

            var remaining = Math.Max(0, entity.MaxDevices - entity.Devices.Count);
            return Ok(new PhonicsActivationResponse(
                true, entity.Code, entity.BundleName, entity.MaxDevices, entity.Devices.Count, remaining,
                "تم تفعيل التطبيق بنجاح على هذا الجهاز! 🎉",
                "App activated successfully on this device! 🎉"));
        }

        if (entity.Devices.Count >= entity.MaxDevices)
        {
            return BadRequest(new PhonicsActivationResponse(
                false, entity.Code, entity.BundleName, entity.MaxDevices, entity.Devices.Count, 0,
                $"تم استخدام هذا الكود على الحد الأقصى من الأجهزة ({entity.MaxDevices}).",
                $"This code has reached its maximum device limit ({entity.MaxDevices})."));
        }

        entity.Devices.Add(new PhonicsDeviceActivation
        {
            DeviceId = deviceId,
            DeviceName = string.IsNullOrWhiteSpace(request.DeviceName) ? "Android Device" : request.DeviceName.Trim(),
            ActivatedAtUtc = DateTime.UtcNow,
            LastVerifiedAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);

        var usedCount = entity.Devices.Count;
        var remainingCount = Math.Max(0, entity.MaxDevices - usedCount);

        return Ok(new PhonicsActivationResponse(
            true, entity.Code, entity.BundleName, entity.MaxDevices, usedCount, remainingCount,
            "تم تفعيل جميع المراحل بنجاح! استمتعوا باللعب 🎉",
            "All stages unlocked! Have fun learning 🎉"));
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] PhonicsVerifyRequest request, CancellationToken ct)
    {
        var normalizedCode = (request.Code ?? string.Empty).Trim().ToUpperInvariant();
        var deviceId = (request.DeviceId ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedCode) || string.IsNullOrWhiteSpace(deviceId))
            return Ok(new { isValid = false, reason = "missing_fields" });

        var entity = await _db.PhonicsActivationCodes
            .Include(c => c.Devices)
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, ct);

        if (entity is null || !entity.IsActive ||
            (entity.ExpiresAtUtc.HasValue && entity.ExpiresAtUtc.Value <= DateTime.UtcNow))
        {
            return Ok(new { isValid = false, reason = "inactive_or_expired" });
        }

        var device = entity.Devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));

        if (device is null)
            return Ok(new { isValid = false, reason = "device_not_bound" });

        device.LastVerifiedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Ok(new { isValid = true, bundleName = entity.BundleName });
    }

    [HttpGet("content/manifest")]
    public async Task<IActionResult> GetContentManifest(CancellationToken ct)
    {
        var manifestPath = Path.Combine(_env.ContentRootPath, "App_Data", "PhonicsContent", "manifest.json");
        if (!System.IO.File.Exists(manifestPath))
            return Ok(new { version = 1, updatedAtUtc = DateTime.UtcNow });

        var json = await System.IO.File.ReadAllTextAsync(manifestPath, ct);
        return Content(json, "application/json");
    }

    [HttpGet("content/{packName}")]
    public async Task<IActionResult> GetContentPack(string packName, CancellationToken ct)
    {
        var normalized = (packName ?? string.Empty).Trim().ToLowerInvariant();
        if (!AllowedPacks.Contains(normalized))
            return NotFound(new { error = "Unknown content pack." });

        var packPath = Path.Combine(_env.ContentRootPath, "App_Data", "PhonicsContent", $"{normalized}.json");
        if (!System.IO.File.Exists(packPath))
            return NotFound(new { error = "Content pack not found." });

        var json = await System.IO.File.ReadAllTextAsync(packPath, ct);
        return Content(json, "application/json");
    }
}
