using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Persists child stars, streak, and completed words locally.</summary>
public sealed class GameProgressService : IGameProgressService
{
    private const string KeyStars = "phonics_total_stars";
    private const string KeyBestStreak = "phonics_best_streak";
    private const string KeyCompletedWords = "phonics_completed_words";

    public int TotalStars => Preferences.Default.Get(KeyStars, 0);
    public int BestStreak => Preferences.Default.Get(KeyBestStreak, 0);

    public HashSet<string> GetCompletedWords()
    {
        var raw = Preferences.Default.Get(KeyCompletedWords, string.Empty);
        if (string.IsNullOrWhiteSpace(raw))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public void RecordWordCompleted(string word, int starsEarned, int currentStreak)
    {
        Preferences.Default.Set(KeyStars, TotalStars + Math.Max(1, starsEarned));

        if (currentStreak > BestStreak)
            Preferences.Default.Set(KeyBestStreak, currentStreak);

        var set = GetCompletedWords();
        if (set.Add(word))
            Preferences.Default.Set(KeyCompletedWords, string.Join(',', set));
    }
}
