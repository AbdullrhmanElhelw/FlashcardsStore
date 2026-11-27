namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IGameProgressService
{
    int TotalStars { get; }
    int AvailableStars { get; }
    int TotalXp { get; }
    int BestStreak { get; }
    int DailyStreakDays { get; }
    bool IsDailyChallengeCompletedToday { get; }
    string EquippedTheme { get; }
    HashSet<string> GetCompletedWords();
    IReadOnlyList<string> GetWeakWords();
    int GetWordMastery(string word);
    bool IsStoryCompleted(int storyId);
    bool IsShopItemUnlocked(string itemId);
    void RecordWordCompleted(string word, int starsEarned, int currentStreak);
    void RecordWordMistake(string word);
    void ClearWordMistake(string word);
    void RecordStoryCompleted(int storyId, int starsEarned);
    void MarkDailyChallengeCompleted();
    bool TryUnlockShopItem(string itemId, int costStars);
    void SetEquippedTheme(string themeKey);
}
