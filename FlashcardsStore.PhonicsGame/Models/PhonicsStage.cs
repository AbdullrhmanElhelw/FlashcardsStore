namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsStage(
    int Id,
    string TitleAr,
    string TitleEn,
    string SubtitleAr,
    string SubtitleEn,
    string BadgeKey,
    string GradientStart,
    string GradientEnd,
    bool IsFree,
    IReadOnlyList<PhonicsWord> Words);
