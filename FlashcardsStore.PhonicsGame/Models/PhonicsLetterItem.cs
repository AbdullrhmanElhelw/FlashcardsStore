namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsLetterItem(
    char Letter,
    string ExampleWord,
    string ArabicWord,
    bool IsVowel);
