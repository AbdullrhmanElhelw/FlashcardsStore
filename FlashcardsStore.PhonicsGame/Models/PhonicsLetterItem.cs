namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsLetterItem(
    char Letter,
    string ExampleWord,
    string ArabicWord,
    bool IsVowel,
    string Sound = "",
    int Group = 1,
    string ActionAr = "",
    string ActionEn = "",
    string ArtworkKey = "")
{
    public string DisplaySound => string.IsNullOrWhiteSpace(Sound) ? Letter.ToString().ToLowerInvariant() : Sound;
    public string DisplayArtwork => string.IsNullOrWhiteSpace(ArtworkKey) ? ExampleWord.ToLowerInvariant() : ArtworkKey;
}
