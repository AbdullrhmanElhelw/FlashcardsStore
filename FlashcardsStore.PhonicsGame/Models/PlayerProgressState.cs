namespace FlashcardsStore.PhonicsGame.Models;

/// <summary>Structured Duolingo-style player progress serialized as JSON.</summary>
public sealed class PlayerProgressState
{
    public int TotalStars { get; set; }
    public int StarsSpent { get; set; }
    public int TotalXp { get; set; }
    public int BestStreak { get; set; }
    public int DailyStreakDays { get; set; }
    public string LastActiveDate { get; set; } = string.Empty;
    public string LastDailyChallengeDate { get; set; } = string.Empty;
    public Dictionary<string, int> WordMastery { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> WeakWords { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<int> CompletedStoryIds { get; set; } = [];
    public HashSet<string> UnlockedShopIds { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string EquippedTheme { get; set; } = "default";
}
