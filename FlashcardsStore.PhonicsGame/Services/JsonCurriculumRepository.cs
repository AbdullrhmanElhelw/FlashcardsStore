using System.Reflection;
using System.Text.Json;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Data-driven repository loading phonics stages and animated stories from embedded JSON packs.</summary>
public sealed class JsonCurriculumRepository : ICurriculumRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IReadOnlyList<PhonicsStage> _stages;
    private readonly IReadOnlyList<PhonicsStory> _stories;

    public JsonCurriculumRepository()
    {
        _stages = LoadResource<CurriculumPack>("curriculum.json")?.Stages ?? [];
        _stories = LoadResource<StoriesPack>("stories.json")?.Stories ?? [];
        TotalWordsCount = _stages.Sum(s => s.Words.Count);
    }

    public IReadOnlyList<PhonicsStage> GetStages() => _stages;
    public IReadOnlyList<PhonicsStory> GetStories() => _stories;
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
