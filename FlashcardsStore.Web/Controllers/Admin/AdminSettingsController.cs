using FlashcardsStore.Application.DTOs.Settings;
using FlashcardsStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Manages global store settings shared by the website and mobile app.</summary>
public sealed class AdminSettingsController : AdminControllerBase
{
    private readonly IStoreSettingsService _storeSettingsService;

    public AdminSettingsController(IStoreSettingsService storeSettingsService)
    {
        _storeSettingsService = storeSettingsService;
    }

    [HttpGet("Settings")]
    public async Task<IActionResult> Settings(CancellationToken cancellationToken)
    {
        var settings = await _storeSettingsService.GetSettingsAsync(cancellationToken);
        return AdminView("Settings", settings);
    }

    [HttpPost("Settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(StoreSettingsDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "يرجى مراجعة الحقول المطلوبة والتأكد من صحة البيانات.";
            return AdminView("Settings", dto);
        }

        var result = await _storeSettingsService.UpdateSettingsAsync(dto, CurrentAdminUser, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "حدث خطأ أثناء حفظ الإعدادات.";
            return AdminView("Settings", dto);
        }

        TempData["SuccessMessage"] = "تم حفظ وتحديث إعدادات المتجر وبيانات التواصل والدفع بنجاح! ✓";
        return RedirectToAction(nameof(Settings));
    }
}
