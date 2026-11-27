using System.Security.Cryptography;
using System.Text.Json;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Manages Phonics Game activation codes sold with bundles or subscriptions.</summary>
public sealed class AdminPhonicsController : AdminControllerBase
{
    private const string CodeAlphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
    private static readonly string[] ContentPacks = ["curriculum", "stories", "sentences", "rhymes", "minimal_pairs", "alphabet"];
    private readonly FlashcardsStoreDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AdminPhonicsController(FlashcardsStoreDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("PhonicsCodes")]
    public async Task<IActionResult> PhonicsCodes(
        string? search,
        string? status = "all",
        int page = 1,
        int pageSize = 15,
        CancellationToken ct = default)
    {
        var query = _db.PhonicsActivationCodes
            .Include(c => c.Devices)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c =>
                c.Code.Contains(term) ||
                (c.BundleName != null && c.BundleName.Contains(term)) ||
                (c.Notes != null && c.Notes.Contains(term)));
        }

        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            query = query.Where(c => c.IsActive);
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            query = query.Where(c => !c.IsActive);

        var allCodes = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

        var pagedInfo = BuildPagedResult(allCodes, page, pageSize, out var pagedCodes);

        ViewBag.SearchTerm = search;
        ViewBag.StatusFilter = status ?? "all";
        ViewBag.PagedInfo = pagedInfo;
        ViewBag.PaginationBaseUrl = $"/Admin/PhonicsCodes?search={Uri.EscapeDataString(search ?? "")}&status={Uri.EscapeDataString(status ?? "all")}";
        ViewBag.TotalCodes = await _db.PhonicsActivationCodes.CountAsync(ct);
        ViewBag.ActiveCodes = await _db.PhonicsActivationCodes.CountAsync(c => c.IsActive, ct);
        ViewBag.TotalDevices = await _db.PhonicsDeviceActivations.CountAsync(ct);

        return AdminView("PhonicsCodes", pagedCodes);
    }

    [HttpPost("PhonicsCodes/Generate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(
        int count = 5,
        string? bundleName = "باقة تأسيس الفونكس",
        int maxDevices = 2,
        int? expiresInDays = null,
        string? notes = null,
        CancellationToken ct = default)
    {
        var safeCount = Math.Clamp(count, 1, 50);
        var safeMaxDevices = Math.Clamp(maxDevices, 1, 10);
        DateTime? expiresAt = expiresInDays is > 0
            ? DateTime.UtcNow.AddDays(expiresInDays.Value)
            : null;

        var existingCodes = (await _db.PhonicsActivationCodes
            .IgnoreQueryFilters()
            .Select(c => c.Code)
            .ToListAsync(ct))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var createdCodes = new List<PhonicsActivationCode>(safeCount);
        for (var i = 0; i < safeCount; i++)
        {
            string code;
            do
            {
                code = GenerateFormattedCode();
            } while (!existingCodes.Add(code));

            createdCodes.Add(new PhonicsActivationCode
            {
                Code = code,
                BundleName = string.IsNullOrWhiteSpace(bundleName) ? "باقة تأسيس الفونكس" : bundleName.Trim(),
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
                MaxDevices = safeMaxDevices,
                IsActive = true,
                ExpiresAtUtc = expiresAt
            });
        }

        _db.PhonicsActivationCodes.AddRange(createdCodes);
        await _db.SaveChangesAsync(ct);

        TempData["SuccessMessage"] = $"تم توليد {safeCount} كود تفعيل بنجاح.";
        return RedirectToAction(nameof(PhonicsCodes));
    }

    [HttpPost("PhonicsCodes/Toggle/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, CancellationToken ct = default)
    {
        var entity = await _db.PhonicsActivationCodes.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
        {
            TempData["ErrorMessage"] = "الكود غير موجود.";
            return RedirectToAction(nameof(PhonicsCodes));
        }

        entity.IsActive = !entity.IsActive;
        await _db.SaveChangesAsync(ct);

        TempData["SuccessMessage"] = entity.IsActive
            ? $"تم تفعيل الكود {entity.Code}."
            : $"تم إيقاف الكود {entity.Code}.";
        return RedirectToAction(nameof(PhonicsCodes));
    }

    [HttpPost("PhonicsCodes/ResetDevices/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetDevices(int id, CancellationToken ct = default)
    {
        var entity = await _db.PhonicsActivationCodes
            .Include(c => c.Devices)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (entity is null)
        {
            TempData["ErrorMessage"] = "الكود غير موجود.";
            return RedirectToAction(nameof(PhonicsCodes));
        }

        _db.PhonicsDeviceActivations.RemoveRange(entity.Devices);
        await _db.SaveChangesAsync(ct);

        TempData["SuccessMessage"] = $"تم تصفير الأجهزة المرتبطة بالكود {entity.Code}.";
        return RedirectToAction(nameof(PhonicsCodes));
    }

    [HttpPost("PhonicsCodes/Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var entity = await _db.PhonicsActivationCodes.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
        {
            TempData["ErrorMessage"] = "الكود غير موجود.";
            return RedirectToAction(nameof(PhonicsCodes));
        }

        _db.PhonicsActivationCodes.Remove(entity);
        await _db.SaveChangesAsync(ct);

        TempData["SuccessMessage"] = $"تم حذف الكود {entity.Code}.";
        return RedirectToAction(nameof(PhonicsCodes));
    }

    private static string GenerateFormattedCode()
    {
        Span<char> part1 = stackalloc char[4];
        Span<char> part2 = stackalloc char[4];
        for (var i = 0; i < 4; i++)
        {
            part1[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
            part2[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
        }
        return $"KHO-{new string(part1)}-{new string(part2)}";
    }

    [HttpGet("PhonicsContent")]
    public async Task<IActionResult> PhonicsContent(string pack = "curriculum", CancellationToken ct = default)
    {
        var selectedPack = ContentPacks.Contains(pack, StringComparer.OrdinalIgnoreCase)
            ? pack.ToLowerInvariant()
            : "curriculum";

        var dir = Path.Combine(_env.ContentRootPath, "App_Data", "PhonicsContent");
        var packPath = Path.Combine(dir, $"{selectedPack}.json");
        var manifestPath = Path.Combine(dir, "manifest.json");

        var jsonContent = System.IO.File.Exists(packPath)
            ? await System.IO.File.ReadAllTextAsync(packPath, ct)
            : "{}";

        var version = 1;
        if (System.IO.File.Exists(manifestPath))
        {
            try
            {
                using var doc = JsonDocument.Parse(await System.IO.File.ReadAllTextAsync(manifestPath, ct));
                if (doc.RootElement.TryGetProperty("version", out var v))
                    version = v.GetInt32();
            }
            catch { }
        }

        ViewBag.SelectedPack = selectedPack;
        ViewBag.AvailablePacks = ContentPacks;
        ViewBag.JsonContent = jsonContent;
        ViewBag.ManifestVersion = version;

        return AdminView("PhonicsContent");
    }

    [HttpPost("PhonicsContent/Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePhonicsContent(string pack, string jsonContent, CancellationToken ct = default)
    {
        var selectedPack = ContentPacks.Contains(pack, StringComparer.OrdinalIgnoreCase)
            ? pack.ToLowerInvariant()
            : "curriculum";

        try
        {
            using var parsed = JsonDocument.Parse(jsonContent ?? string.Empty);
        }
        catch (JsonException ex)
        {
            TempData["ErrorMessage"] = $"صيغة JSON غير صحيحة: {ex.Message}";
            return RedirectToAction(nameof(PhonicsContent), new { pack = selectedPack });
        }

        var dir = Path.Combine(_env.ContentRootPath, "App_Data", "PhonicsContent");
        Directory.CreateDirectory(dir);

        var packPath = Path.Combine(dir, $"{selectedPack}.json");
        await System.IO.File.WriteAllTextAsync(packPath, jsonContent, ct);

        var manifestPath = Path.Combine(dir, "manifest.json");
        var nextVersion = 1;
        if (System.IO.File.Exists(manifestPath))
        {
            try
            {
                using var doc = JsonDocument.Parse(await System.IO.File.ReadAllTextAsync(manifestPath, ct));
                if (doc.RootElement.TryGetProperty("version", out var v))
                    nextVersion = v.GetInt32() + 1;
            }
            catch { }
        }

        var newManifest = JsonSerializer.Serialize(new { version = nextVersion, updatedAtUtc = DateTime.UtcNow }, new JsonSerializerOptions { WriteIndented = true });
        await System.IO.File.WriteAllTextAsync(manifestPath, newManifest, ct);

        TempData["SuccessMessage"] = $"تم حفظ {selectedPack}.json وتحديث إصدار المنهج إلى v{nextVersion}. سيتم المزامنة تلقائياً مع تطبيقات الموبايل.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = selectedPack });
    }
}
