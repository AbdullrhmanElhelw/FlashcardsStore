namespace FlashcardsStore.Application.Common;

/// <summary>
/// Represents an educational word or sentence item available for interactive audio pronunciation preview.
/// </summary>
public class PronunciationItem
{
    public string Word { get; set; } = string.Empty;
    public string? Phonetic { get; set; }
    public string? IconEmoji { get; set; }
    public string? ExampleSentence { get; set; }
}
