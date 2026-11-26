namespace FlashcardsStore.PhonicsGame.Models;

public sealed class MemoryCardItem
{
    public int Index { get; init; }
    public string WordKey { get; init; } = string.Empty;
    public bool IsPictureCard { get; init; }
    public string ArtworkKey { get; init; } = string.Empty;
    public string AccentColor { get; init; } = "#2B071C";
    public string BgColor { get; init; } = "#FFF9F2";
    public bool IsFlipped { get; set; }
    public bool IsMatched { get; set; }
}
