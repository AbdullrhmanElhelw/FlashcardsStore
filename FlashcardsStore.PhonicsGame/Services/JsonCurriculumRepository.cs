using System.Reflection;
using System.Text.Json;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Loads curriculum packs from local OTA cache first, falling back to embedded JSON resources.</summary>
public sealed class JsonCurriculumRepository : ICurriculumRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private IReadOnlyList<PhonicsStage> _stages = [];
    private IReadOnlyList<PhonicsStory> _stories = [];
    private IReadOnlyList<SentenceChallenge> _sentences = [];
    private IReadOnlyList<RhymeChallenge> _rhymes = [];
    private IReadOnlyList<MinimalPairChallenge> _minimalPairs = [];
    private IReadOnlyList<PhonicsLetterItem> _alphabet = [];
    private IReadOnlyList<ShopItem> _shopItems = [];

    public event Action? ContentReloaded;

    public JsonCurriculumRepository()
    {
        LoadAll();
    }

    public IReadOnlyList<PhonicsStage> GetStages() => _stages;
    public IReadOnlyList<PhonicsStory> GetStories() => _stories;
    public IReadOnlyList<SentenceChallenge> GetSentences() => _sentences;
    public IReadOnlyList<RhymeChallenge> GetRhymes() => _rhymes;
    public IReadOnlyList<MinimalPairChallenge> GetMinimalPairs() => _minimalPairs;
    public IReadOnlyList<PhonicsLetterItem> GetAlphabet() => _alphabet;
    public IReadOnlyList<ShopItem> GetShopItems() => _shopItems;
    public int TotalWordsCount { get; private set; }

    public void Reload()
    {
        LoadAll();
        ContentReloaded?.Invoke();
    }

    private void LoadAll()
    {
        _stages = LoadPack<CurriculumPack>("curriculum.json")?.Stages ?? [];
        _stories = LoadPack<StoriesPack>("stories.json")?.Stories ?? [];
        _sentences = LoadPack<SentencesPack>("sentences.json")?.Sentences ?? [];
        _rhymes = LoadPack<RhymesPack>("rhymes.json")?.Rhymes ?? [];
        _minimalPairs = LoadPack<MinimalPairsPack>("minimal_pairs.json")?.Pairs ?? [];
        _alphabet = LoadPack<AlphabetPack>("alphabet.json")?.Letters ?? [];
        _shopItems = LoadPack<ShopPack>("shop.json")?.Items ?? [];
        TotalWordsCount = _stages.Sum(s => s.Words.Count);
    }

    private static T? LoadPack<T>(string fileName)
    {
        // 1. Prefer OTA cached file if valid
        var otaPath = Path.Combine(FileSystem.AppDataDirectory, "phonics_ota", fileName);
        if (File.Exists(otaPath))
        {
            try
            {
                var cachedJson = File.ReadAllText(otaPath);
                var parsed = JsonSerializer.Deserialize<T>(cachedJson, JsonOptions);
                if (parsed is not null) return parsed;
            }
            catch
            {
                // Fall back to embedded pack if cached file is corrupted
            }
        }

        // 2. Fallback to embedded JSON bundled in APK
        var asm = Assembly.GetExecutingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null) return default;
        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream is null) return default;
        return JsonSerializer.Deserialize<T>(stream, JsonOptions);
    }
}
