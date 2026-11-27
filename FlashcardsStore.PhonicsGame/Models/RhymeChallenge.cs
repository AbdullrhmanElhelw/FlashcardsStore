namespace FlashcardsStore.PhonicsGame.Models;

public sealed record RhymeChallenge(
    string TargetWord,
    string TargetArtworkKey,
    string TargetAccentColor,
    string CorrectRhyme,
    IReadOnlyList<string> Distractors,
    bool IsFree);
