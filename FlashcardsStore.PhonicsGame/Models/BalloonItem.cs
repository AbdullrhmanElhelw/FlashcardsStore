namespace FlashcardsStore.PhonicsGame.Models;

public sealed class BalloonItem
{
    public string Sound { get; set; } = string.Empty;
    public string Color { get; set; } = "#7C3AED";
    public int DelayMs { get; set; }
    public bool IsPopped { get; set; }
    public bool IsWrong { get; set; }
}
