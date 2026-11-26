namespace FlashcardsStore.PhonicsGame.Models;

public sealed record StoryScene(
    string SentenceEn,
    string SentenceAr,
    string FocusWord,
    string ArtworkKey,
    string BgColor,
    string AccentColor,
    string AnimationClass);
