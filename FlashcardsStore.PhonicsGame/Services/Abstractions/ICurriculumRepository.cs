using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface ICurriculumRepository
{
    IReadOnlyList<PhonicsStage> GetStages();
    IReadOnlyList<PhonicsStory> GetStories();
    int TotalWordsCount { get; }
}
