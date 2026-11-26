namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsWord(
    string Word,
    string ArabicMeaning,
    string ArtworkKey,
    string BgColor,
    string AccentColor,
    bool IsSightWord = false);
