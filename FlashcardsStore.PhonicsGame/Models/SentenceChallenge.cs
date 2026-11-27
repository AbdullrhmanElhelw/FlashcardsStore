namespace FlashcardsStore.PhonicsGame.Models;

public sealed record SentenceChallenge(
    int Id,
    IReadOnlyList<string> Words,
    string ArabicMeaning,
    string ArtworkKey,
    string AccentColor,
    bool IsFree);
