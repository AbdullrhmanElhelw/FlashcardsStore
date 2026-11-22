namespace FlashcardsStore.Mobile.Models.Api;

public class PronunciationItem
{
    public string Word { get; set; } = string.Empty;
    public string? Phonetic { get; set; }
    public string? IconEmoji { get; set; }
    public string? ExampleSentence { get; set; }
}
