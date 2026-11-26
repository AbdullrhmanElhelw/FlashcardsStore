namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IGameProgressService
{
    int TotalStars { get; }
    int BestStreak { get; }
    HashSet<string> GetCompletedWords();
    void RecordWordCompleted(string word, int starsEarned, int currentStreak);
}
