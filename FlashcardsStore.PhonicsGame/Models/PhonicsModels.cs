namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsWord(
    string Word,
    string ArabicMeaning,
    string ArtworkKey,
    string BgColor,
    string AccentColor,
    bool IsSightWord = false);

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

public sealed class LetterTileItem
{
    public char Letter { get; init; }
    public bool Used { get; set; }
}

public sealed record ActivationResult(
    bool Success,
    string MessageAr,
    string MessageEn,
    string? BundleName = null);
