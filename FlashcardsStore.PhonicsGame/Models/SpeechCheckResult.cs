namespace FlashcardsStore.PhonicsGame.Models;

public sealed record SpeechCheckResult(bool IsSupported, bool IsMatch, string RecognizedText);
