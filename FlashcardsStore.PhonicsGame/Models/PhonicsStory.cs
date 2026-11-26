namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsStory(
    int Id,
    string TitleAr,
    string TitleEn,
    string SubtitleAr,
    string SubtitleEn,
    string CoverArtworkKey,
    string AccentColor,
    bool IsFree,
    IReadOnlyList<StoryScene> Scenes,
    string QuizQuestionAr,
    string QuizQuestionEn,
    string QuizCorrectWord,
    IReadOnlyList<string> QuizOptions);
