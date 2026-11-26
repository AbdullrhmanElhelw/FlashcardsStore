namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IGameProgressService
{
    int TotalStars { get; }
    int TotalXp { get; }
    int BestStreak { get; }
    int DailyStreakDays { get; }
    bool IsDailyChallengeCompletedToday { get; }
    HashSet<string> GetCompletedWords();
    int GetWordMastery(string word);
    bool IsStoryCompleted(int storyId);
    void RecordWordCompleted(string word, int starsEarned, int currentStreak);
    void RecordStoryCompleted(int storyId, int starsEarned);
    void MarkDailyChallengeCompleted();
}
