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
    private static readonly string[] ContentPacks = ["curriculum", "alphabet", "stories", "sentences", "rhymes", "minimal_pairs", "shop"];
    private static readonly JsonSerializerOptions PrettyJson = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
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

    private string OtaDir => Path.Combine(_env.ContentRootPath, "App_Data", "PhonicsContent");

    private string? ResolveAppDefaultPackPath(string pack)
    {
        var candidate = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "FlashcardsStore.PhonicsGame", "Resources", "Content", $"{pack}.json"));
        return System.IO.File.Exists(candidate) ? candidate : null;
    }

    private async Task<string> ReadPackJsonAsync(string pack, CancellationToken ct)
    {
        var otaPath = Path.Combine(OtaDir, $"{pack}.json");
        if (System.IO.File.Exists(otaPath))
            return await System.IO.File.ReadAllTextAsync(otaPath, ct);

        var defaultPath = ResolveAppDefaultPackPath(pack);
        if (defaultPath is not null)
            return await System.IO.File.ReadAllTextAsync(defaultPath, ct);

        return "{}";
    }

    private async Task<int> WritePackAndBumpVersionAsync(string pack, string jsonContent, CancellationToken ct)
    {
        Directory.CreateDirectory(OtaDir);
        var packPath = Path.Combine(OtaDir, $"{pack}.json");
        await System.IO.File.WriteAllTextAsync(packPath, jsonContent, ct);

        var manifestPath = Path.Combine(OtaDir, "manifest.json");
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
        return nextVersion;
    }

    private async Task<int> GetCurrentManifestVersionAsync(CancellationToken ct)
    {
        var manifestPath = Path.Combine(OtaDir, "manifest.json");
        if (!System.IO.File.Exists(manifestPath)) return 1;
        try
        {
            using var doc = JsonDocument.Parse(await System.IO.File.ReadAllTextAsync(manifestPath, ct));
            if (doc.RootElement.TryGetProperty("version", out var v))
                return v.GetInt32();
        }
        catch { }
        return 1;
    }

    [HttpGet("PhonicsContent")]
    public async Task<IActionResult> PhonicsContent(string pack = "curriculum", CancellationToken ct = default)
    {
        var selectedPack = ContentPacks.Contains(pack, StringComparer.OrdinalIgnoreCase)
            ? pack.ToLowerInvariant()
            : "curriculum";

        var jsonContent = await ReadPackJsonAsync(selectedPack, ct);
        var version = await GetCurrentManifestVersionAsync(ct);

        ViewBag.SelectedPack = selectedPack;
        ViewBag.AvailablePacks = ContentPacks;
        ViewBag.JsonContent = jsonContent;
        ViewBag.ManifestVersion = version;
        ViewBag.IsOtaActive = System.IO.File.Exists(Path.Combine(OtaDir, $"{selectedPack}.json"));
        ViewBag.HasAppDefaults = ResolveAppDefaultPackPath(selectedPack) is not null;

        return AdminView("PhonicsContent");
    }

    [HttpGet("PhonicsContent/Download")]
    public async Task<IActionResult> DownloadPack(string pack = "curriculum", CancellationToken ct = default)
    {
        var selectedPack = ContentPacks.Contains(pack, StringComparer.OrdinalIgnoreCase)
            ? pack.ToLowerInvariant()
            : "curriculum";

        var json = await ReadPackJsonAsync(selectedPack, ct);
        return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"{selectedPack}.json");
    }

    [HttpPost("PhonicsContent/SyncDefaults")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SyncFromAppDefaults(string pack = "curriculum", CancellationToken ct = default)
    {
        var selectedPack = ContentPacks.Contains(pack, StringComparer.OrdinalIgnoreCase)
            ? pack.ToLowerInvariant()
            : "curriculum";

        var synced = 0;
        var nextVersion = await GetCurrentManifestVersionAsync(ct);
        foreach (var p in ContentPacks)
        {
            var defaultPath = ResolveAppDefaultPackPath(p);
            if (defaultPath is null) continue;
            var content = await System.IO.File.ReadAllTextAsync(defaultPath, ct);
            nextVersion = await WritePackAndBumpVersionAsync(p, content, ct);
            synced++;
        }

        if (synced == 0)
            TempData["ErrorMessage"] = "تعذر العثور على ملفات المنهج الافتراضية على السيرفر.";
        else
            TempData["SuccessMessage"] = $"تم استيراد {synced} حزمة منهج من أحدث نسخة للتطبيق ونشر الإصدار v{nextVersion}.";

        return RedirectToAction(nameof(PhonicsContent), new { pack = selectedPack });
    }

    [HttpPost("PhonicsContent/Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePhonicsContent(string pack, string jsonContent, CancellationToken ct = default)
    {
        var selectedPack = ContentPacks.Contains(pack, StringComparer.OrdinalIgnoreCase)
            ? pack.ToLowerInvariant()
            : "curriculum";

        string formatted;
        try
        {
            using var parsed = JsonDocument.Parse(jsonContent ?? string.Empty);
            formatted = JsonSerializer.Serialize(parsed.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException ex)
        {
            TempData["ErrorMessage"] = $"صيغة JSON غير صحيحة: {ex.Message}";
            return RedirectToAction(nameof(PhonicsContent), new { pack = selectedPack });
        }

        var nextVersion = await WritePackAndBumpVersionAsync(selectedPack, formatted, ct);
        TempData["SuccessMessage"] = $"تم حفظ {selectedPack}.json وتحديث إصدار المنهج إلى v{nextVersion}. سيتم المزامنة تلقائياً مع تطبيقات الموبايل.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = selectedPack });
    }

    public sealed class StageFormDto
    {
        public int? Id { get; set; }
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public string SubtitleAr { get; set; } = "";
        public string SubtitleEn { get; set; } = "";
        public string Category { get; set; } = "letters";
        public string BadgeKey { get; set; } = "sun";
        public string GradientStart { get; set; } = "#F97316";
        public string GradientEnd { get; set; } = "#EA580C";
        public bool IsFree { get; set; }
    }

    public sealed class WordFormDto
    {
        public int StageId { get; set; }
        public string? OriginalWord { get; set; }
        public string Word { get; set; } = "";
        public string ArabicMeaning { get; set; } = "";
        public string ArtworkKey { get; set; } = "";
        public string BgColor { get; set; } = "#FEF3C7";
        public string AccentColor { get; set; } = "#D97706";
        public bool IsSightWord { get; set; }
    }

    public sealed class LetterFormDto
    {
        public string? OriginalSound { get; set; }
        public string Letter { get; set; } = "S";
        public string Sound { get; set; } = "s";
        public int Group { get; set; } = 1;
        public string ExampleWord { get; set; } = "";
        public string ArabicWord { get; set; } = "";
        public string ArtworkKey { get; set; } = "";
        public bool IsVowel { get; set; }
        public string ActionAr { get; set; } = "";
        public string ActionEn { get; set; } = "";
    }

    public sealed class CurriculumDoc
    {
        public List<StageDoc> Stages { get; set; } = [];
    }

    public sealed class StageDoc
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public string SubtitleAr { get; set; } = "";
        public string SubtitleEn { get; set; } = "";
        public string BadgeKey { get; set; } = "sun";
        public string GradientStart { get; set; } = "#F97316";
        public string GradientEnd { get; set; } = "#EA580C";
        public bool IsFree { get; set; }
        public string Category { get; set; } = "letters";
        public List<WordDoc> Words { get; set; } = [];
    }

    public sealed class WordDoc
    {
        public string Word { get; set; } = "";
        public string ArabicMeaning { get; set; } = "";
        public string ArtworkKey { get; set; } = "";
        public string BgColor { get; set; } = "#FEF3C7";
        public string AccentColor { get; set; } = "#D97706";
        public bool IsSightWord { get; set; }
    }

    public sealed class AlphabetDoc
    {
        public List<LetterDoc> Letters { get; set; } = [];
    }

    public sealed class LetterDoc
    {
        public string Letter { get; set; } = "S";
        public string Sound { get; set; } = "s";
        public int Group { get; set; } = 1;
        public string ExampleWord { get; set; } = "";
        public string ArabicWord { get; set; } = "";
        public string ArtworkKey { get; set; } = "";
        public bool IsVowel { get; set; }
        public string ActionAr { get; set; } = "";
        public string ActionEn { get; set; } = "";
    }

    [HttpPost("PhonicsContent/Stage/Upsert")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertStage(StageFormDto form, CancellationToken ct = default)
    {
        var json = await ReadPackJsonAsync("curriculum", ct);
        var doc = JsonSerializer.Deserialize<CurriculumDoc>(json, PrettyJson) ?? new CurriculumDoc();

        if (form.Id is > 0)
        {
            var existing = doc.Stages.FirstOrDefault(s => s.Id == form.Id.Value);
            if (existing is null)
            {
                TempData["ErrorMessage"] = "المرحلة غير موجودة.";
                return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
            }
            existing.TitleAr = form.TitleAr.Trim();
            existing.TitleEn = form.TitleEn.Trim();
            existing.SubtitleAr = form.SubtitleAr.Trim();
            existing.SubtitleEn = form.SubtitleEn.Trim();
            existing.Category = string.IsNullOrWhiteSpace(form.Category) ? "letters" : form.Category.Trim();
            existing.BadgeKey = string.IsNullOrWhiteSpace(form.BadgeKey) ? "sun" : form.BadgeKey.Trim();
            existing.GradientStart = string.IsNullOrWhiteSpace(form.GradientStart) ? "#F97316" : form.GradientStart.Trim();
            existing.GradientEnd = string.IsNullOrWhiteSpace(form.GradientEnd) ? "#EA580C" : form.GradientEnd.Trim();
            existing.IsFree = form.IsFree;
        }
        else
        {
            var nextId = doc.Stages.Count == 0 ? 1 : doc.Stages.Max(s => s.Id) + 1;
            doc.Stages.Add(new StageDoc
            {
                Id = nextId,
                TitleAr = form.TitleAr.Trim(),
                TitleEn = form.TitleEn.Trim(),
                SubtitleAr = form.SubtitleAr.Trim(),
                SubtitleEn = form.SubtitleEn.Trim(),
                Category = string.IsNullOrWhiteSpace(form.Category) ? "letters" : form.Category.Trim(),
                BadgeKey = string.IsNullOrWhiteSpace(form.BadgeKey) ? "sun" : form.BadgeKey.Trim(),
                GradientStart = string.IsNullOrWhiteSpace(form.GradientStart) ? "#F97316" : form.GradientStart.Trim(),
                GradientEnd = string.IsNullOrWhiteSpace(form.GradientEnd) ? "#EA580C" : form.GradientEnd.Trim(),
                IsFree = form.IsFree
            });
        }

        var v = await WritePackAndBumpVersionAsync("curriculum", JsonSerializer.Serialize(doc, PrettyJson), ct);
        TempData["SuccessMessage"] = $"تم حفظ المرحلة بنجاح ونشر الإصدار v{v}.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
    }

    [HttpPost("PhonicsContent/Stage/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStage(int id, CancellationToken ct = default)
    {
        var json = await ReadPackJsonAsync("curriculum", ct);
        var doc = JsonSerializer.Deserialize<CurriculumDoc>(json, PrettyJson) ?? new CurriculumDoc();
        var removed = doc.Stages.RemoveAll(s => s.Id == id);
        if (removed == 0)
        {
            TempData["ErrorMessage"] = "المرحلة غير موجودة.";
            return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
        }

        var v = await WritePackAndBumpVersionAsync("curriculum", JsonSerializer.Serialize(doc, PrettyJson), ct);
        TempData["SuccessMessage"] = $"تم حذف المرحلة وتحديث المنهج إلى v{v}.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
    }

    [HttpPost("PhonicsContent/Word/Upsert")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertWord(WordFormDto form, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(form.Word))
        {
            TempData["ErrorMessage"] = "يجب إدخال الكلمة بالإنجليزية.";
            return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
        }

        var json = await ReadPackJsonAsync("curriculum", ct);
        var doc = JsonSerializer.Deserialize<CurriculumDoc>(json, PrettyJson) ?? new CurriculumDoc();
        var stage = doc.Stages.FirstOrDefault(s => s.Id == form.StageId);
        if (stage is null)
        {
            TempData["ErrorMessage"] = "المرحلة المحددة غير موجودة.";
            return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
        }

        var cleanWord = form.Word.Trim().ToLowerInvariant();
        var artKey = string.IsNullOrWhiteSpace(form.ArtworkKey) ? cleanWord : form.ArtworkKey.Trim().ToLowerInvariant();
        var existing = !string.IsNullOrWhiteSpace(form.OriginalWord)
            ? stage.Words.FirstOrDefault(w => string.Equals(w.Word, form.OriginalWord.Trim(), StringComparison.OrdinalIgnoreCase))
            : null;

        if (existing is not null)
        {
            existing.Word = cleanWord;
            existing.ArabicMeaning = form.ArabicMeaning.Trim();
            existing.ArtworkKey = artKey;
            existing.BgColor = string.IsNullOrWhiteSpace(form.BgColor) ? "#FEF3C7" : form.BgColor.Trim();
            existing.AccentColor = string.IsNullOrWhiteSpace(form.AccentColor) ? "#D97706" : form.AccentColor.Trim();
            existing.IsSightWord = form.IsSightWord;
        }
        else
        {
            stage.Words.Add(new WordDoc
            {
                Word = cleanWord,
                ArabicMeaning = form.ArabicMeaning.Trim(),
                ArtworkKey = artKey,
                BgColor = string.IsNullOrWhiteSpace(form.BgColor) ? "#FEF3C7" : form.BgColor.Trim(),
                AccentColor = string.IsNullOrWhiteSpace(form.AccentColor) ? "#D97706" : form.AccentColor.Trim(),
                IsSightWord = form.IsSightWord
            });
        }

        var v = await WritePackAndBumpVersionAsync("curriculum", JsonSerializer.Serialize(doc, PrettyJson), ct);
        TempData["SuccessMessage"] = $"تم حفظ الكلمة ({cleanWord}) ونشر الإصدار v{v}.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
    }

    [HttpPost("PhonicsContent/Word/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteWord(int stageId, string word, CancellationToken ct = default)
    {
        var json = await ReadPackJsonAsync("curriculum", ct);
        var doc = JsonSerializer.Deserialize<CurriculumDoc>(json, PrettyJson) ?? new CurriculumDoc();
        var stage = doc.Stages.FirstOrDefault(s => s.Id == stageId);
        if (stage is null || stage.Words.RemoveAll(w => string.Equals(w.Word, word, StringComparison.OrdinalIgnoreCase)) == 0)
        {
            TempData["ErrorMessage"] = "الكلمة غير موجودة.";
            return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
        }

        var v = await WritePackAndBumpVersionAsync("curriculum", JsonSerializer.Serialize(doc, PrettyJson), ct);
        TempData["SuccessMessage"] = $"تم حذف الكلمة ({word}) ونشر الإصدار v{v}.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = "curriculum" });
    }

    [HttpPost("PhonicsContent/Letter/Upsert")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertLetter(LetterFormDto form, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(form.Sound) || string.IsNullOrWhiteSpace(form.Letter))
        {
            TempData["ErrorMessage"] = "يجب إدخال الحرف والصوت.";
            return RedirectToAction(nameof(PhonicsContent), new { pack = "alphabet" });
        }

        var json = await ReadPackJsonAsync("alphabet", ct);
        var doc = JsonSerializer.Deserialize<AlphabetDoc>(json, PrettyJson) ?? new AlphabetDoc();
        var cleanSound = form.Sound.Trim().ToLowerInvariant();
        var cleanLetter = form.Letter.Trim()[..1].ToUpperInvariant();
        var artKey = string.IsNullOrWhiteSpace(form.ArtworkKey) ? form.ExampleWord.Trim().ToLowerInvariant() : form.ArtworkKey.Trim().ToLowerInvariant();

        var existing = !string.IsNullOrWhiteSpace(form.OriginalSound)
            ? doc.Letters.FirstOrDefault(l => string.Equals(l.Sound, form.OriginalSound.Trim(), StringComparison.OrdinalIgnoreCase))
            : null;

        if (existing is not null)
        {
            existing.Letter = cleanLetter;
            existing.Sound = cleanSound;
            existing.Group = Math.Clamp(form.Group, 1, 7);
            existing.ExampleWord = form.ExampleWord.Trim();
            existing.ArabicWord = form.ArabicWord.Trim();
            existing.ArtworkKey = artKey;
            existing.IsVowel = form.IsVowel;
            existing.ActionAr = form.ActionAr.Trim();
            existing.ActionEn = form.ActionEn.Trim();
        }
        else
        {
            doc.Letters.Add(new LetterDoc
            {
                Letter = cleanLetter,
                Sound = cleanSound,
                Group = Math.Clamp(form.Group, 1, 7),
                ExampleWord = form.ExampleWord.Trim(),
                ArabicWord = form.ArabicWord.Trim(),
                ArtworkKey = artKey,
                IsVowel = form.IsVowel,
                ActionAr = form.ActionAr.Trim(),
                ActionEn = form.ActionEn.Trim()
            });
        }

        var v = await WritePackAndBumpVersionAsync("alphabet", JsonSerializer.Serialize(doc, PrettyJson), ct);
        TempData["SuccessMessage"] = $"تم حفظ كارت الصوت ({cleanSound}) ونشر الإصدار v{v}.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = "alphabet" });
    }

    [HttpPost("PhonicsContent/Letter/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLetter(string sound, CancellationToken ct = default)
    {
        var json = await ReadPackJsonAsync("alphabet", ct);
        var doc = JsonSerializer.Deserialize<AlphabetDoc>(json, PrettyJson) ?? new AlphabetDoc();
        if (doc.Letters.RemoveAll(l => string.Equals(l.Sound, sound, StringComparison.OrdinalIgnoreCase)) == 0)
        {
            TempData["ErrorMessage"] = "كارت الصوت غير موجود.";
            return RedirectToAction(nameof(PhonicsContent), new { pack = "alphabet" });
        }

        var v = await WritePackAndBumpVersionAsync("alphabet", JsonSerializer.Serialize(doc, PrettyJson), ct);
        TempData["SuccessMessage"] = $"تم حذف كارت الصوت ({sound}) ونشر الإصدار v{v}.";
        return RedirectToAction(nameof(PhonicsContent), new { pack = "alphabet" });
    }
}
