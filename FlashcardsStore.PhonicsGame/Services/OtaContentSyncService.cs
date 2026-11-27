using System.Net.Http.Json;
using System.Text.Json;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Checks server manifest and caches updated JSON curriculum packs locally for offline play.</summary>
public sealed class OtaContentSyncService : IOtaContentSyncService
{
    private const string KeyOtaVersion = "phonics_ota_version";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string[] PackNames = ["curriculum", "stories", "sentences", "rhymes", "alphabet"];

    private readonly HttpClient _http;
    private readonly ICurriculumRepository _curriculum;

    public OtaContentSyncService(HttpClient http, ICurriculumRepository curriculum)
    {
        _http = http;
        _curriculum = curriculum;
    }

    public int CurrentContentVersion => Preferences.Default.Get(KeyOtaVersion, 1);

    public async Task<bool> SyncIfNeededAsync(CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var manifest = await _http.GetFromJsonAsync<ContentManifest>(
                "api/phonics/content/manifest", JsonOptions, cts.Token);

            if (manifest is null || manifest.Version <= CurrentContentVersion)
                return false;

            var downloaded = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pack in PackNames)
            {
                var json = await _http.GetStringAsync($"api/phonics/content/{pack}", cts.Token);
                if (!ValidatePackJson(pack, json))
                    return false;
                downloaded[pack] = json;
            }

            var otaDir = Path.Combine(FileSystem.AppDataDirectory, "phonics_ota");
            Directory.CreateDirectory(otaDir);

            foreach (var (pack, json) in downloaded)
            {
                var path = Path.Combine(otaDir, $"{pack}.json");
                await File.WriteAllTextAsync(path, json, cts.Token);
            }

            Preferences.Default.Set(KeyOtaVersion, manifest.Version);
            _curriculum.Reload();
            return true;
        }
        catch
        {
            // Offline or unreachable server: silently continue using cached/embedded content
            return false;
        }
    }

    private static bool ValidatePackJson(string pack, string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            return pack switch
            {
                "curriculum" => JsonSerializer.Deserialize<CurriculumPack>(json, JsonOptions)?.Stages.Count > 0,
                "stories" => JsonSerializer.Deserialize<StoriesPack>(json, JsonOptions)?.Stories.Count > 0,
                "sentences" => JsonSerializer.Deserialize<SentencesPack>(json, JsonOptions)?.Sentences.Count > 0,
                "rhymes" => JsonSerializer.Deserialize<RhymesPack>(json, JsonOptions)?.Rhymes.Count > 0,
                "alphabet" => JsonSerializer.Deserialize<AlphabetPack>(json, JsonOptions)?.Letters.Count > 0,
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }
}
