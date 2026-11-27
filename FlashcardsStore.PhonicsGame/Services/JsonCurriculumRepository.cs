using System.Reflection;
using System.Text.Json;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Data-driven repository loading all educational content packs from embedded JSON resources.</summary>
public sealed class JsonCurriculumRepository : ICurriculumRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IReadOnlyList<PhonicsStage> _stages;
    private readonly IReadOnlyList<PhonicsStory> _stories;
    private readonly IReadOnlyList<SentenceChallenge> _sentences;
    private readonly IReadOnlyList<RhymeChallenge> _rhymes;
    private readonly IReadOnlyList<PhonicsLetterItem> _alphabet;

    public JsonCurriculumRepository()
    {
        _stages = LoadResource<CurriculumPack>("curriculum.json")?.Stages ?? [];
        _stories = LoadResource<StoriesPack>("stories.json")?.Stories ?? [];
        _sentences = LoadResource<SentencesPack>("sentences.json")?.Sentences ?? [];
        _rhymes = LoadResource<RhymesPack>("rhymes.json")?.Rhymes ?? [];
        _alphabet = LoadResource<AlphabetPack>("alphabet.json")?.Letters ?? [];
        TotalWordsCount = _stages.Sum(s => s.Words.Count);
    }

    public IReadOnlyList<PhonicsStage> GetStages() => _stages;
    public IReadOnlyList<PhonicsStory> GetStories() => _stories;
    public IReadOnlyList<SentenceChallenge> GetSentences() => _sentences;
    public IReadOnlyList<RhymeChallenge> GetRhymes() => _rhymes;
    public IReadOnlyList<PhonicsLetterItem> GetAlphabet() => _alphabet;
    public int TotalWordsCount { get; }

    private static T? LoadResource<T>(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null) return default;
        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream is null) return default;
        return JsonSerializer.Deserialize<T>(stream, JsonOptions);
    }
}
