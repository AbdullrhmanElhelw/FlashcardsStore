namespace FlashcardsStore.PhonicsGame.Models;

public sealed record MinimalPairChallenge(
    string ShortWord,
    string LongWord,
    string ShortArabic,
    string LongArabic,
    bool IsFree);
