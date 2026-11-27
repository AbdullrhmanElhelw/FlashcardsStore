namespace FlashcardsStore.PhonicsGame.Models;

public sealed record SegmentChallenge(
    string Word,
    string ArabicMeaning,
    string ArtworkKey,
    string AccentColor,
    IReadOnlyList<string> Phonemes);

public sealed record PositionChallenge(
    string Word,
    string TargetSound,
    int PositionIndex, // 0 = First, 1 = Middle, 2 = Last
    string ArabicMeaning,
    string ArtworkKey,
    string AccentColor);

public sealed record DecodableSentence(
    string Text,
    string ArabicMeaning,
    string ArtworkKey,
    string AccentColor);
