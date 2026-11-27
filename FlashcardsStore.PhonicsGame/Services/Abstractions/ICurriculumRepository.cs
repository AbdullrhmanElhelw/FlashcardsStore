using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface ICurriculumRepository
{
    event Action? ContentReloaded;
    IReadOnlyList<PhonicsStage> GetStages();
    IReadOnlyList<PhonicsStory> GetStories();
    IReadOnlyList<SentenceChallenge> GetSentences();
    IReadOnlyList<RhymeChallenge> GetRhymes();
    IReadOnlyList<PhonicsLetterItem> GetAlphabet();
    int TotalWordsCount { get; }
    void Reload();
}
