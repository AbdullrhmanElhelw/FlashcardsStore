using System.Text.Json;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Duolingo-style JSON progress storage tracking XP, stars, weak words, shop unlocks, and daily streaks.</summary>
public sealed class GameProgressService : IGameProgressService
{
    private const string KeyProgressJson = "phonics_progress_json_v2";
    private PlayerProgressState _state;

    public GameProgressService()
    {
        _state = LoadState();
    }

    public int TotalStars => _state.TotalStars;
    public int AvailableStars => Math.Max(0, _state.TotalStars - _state.StarsSpent);
    public int TotalXp => _state.TotalXp;
    public int BestStreak => _state.BestStreak;
    public int DailyStreakDays => _state.DailyStreakDays;
    public string EquippedTheme => string.IsNullOrWhiteSpace(_state.EquippedTheme) ? "default" : _state.EquippedTheme;

    public bool IsDailyChallengeCompletedToday
        => _state.LastDailyChallengeDate == DateTime.Today.ToString("yyyy-MM-dd");

    public HashSet<string> GetCompletedWords()
        => _state.WordMastery.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> GetWeakWords()
        => _state.WeakWords
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

    public int GetWordMastery(string word)
        => _state.WordMastery.TryGetValue(word, out var level) ? level : 0;

    public bool IsStoryCompleted(int storyId)
        => _state.CompletedStoryIds.Contains(storyId);

    public bool IsShopItemUnlocked(string itemId)
        => _state.UnlockedShopIds.Contains(itemId);

    public void RecordWordCompleted(string word, int starsEarned, int currentStreak)
    {
        var stars = Math.Max(1, starsEarned);
        _state.TotalStars += stars;
        _state.TotalXp += stars * 10;

        if (currentStreak > _state.BestStreak)
            _state.BestStreak = currentStreak;

        _state.WordMastery[word] = Math.Min(3, GetWordMastery(word) + 1);
        _state.WeakWords.Remove(word);
        TouchDailyActivity();
        SaveState();
    }

    public void RecordWordMistake(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        _state.WeakWords[word] = _state.WeakWords.TryGetValue(word, out var count) ? count + 1 : 1;
        SaveState();
    }

    public void ClearWordMistake(string word)
    {
        if (_state.WeakWords.Remove(word))
            SaveState();
    }

    public void RecordStoryCompleted(int storyId, int starsEarned)
    {
        var stars = Math.Max(1, starsEarned);
        _state.TotalStars += stars;
        _state.TotalXp += stars * 15;
        _state.CompletedStoryIds.Add(storyId);
        TouchDailyActivity();
        SaveState();
    }

    public void MarkDailyChallengeCompleted()
    {
        _state.LastDailyChallengeDate = DateTime.Today.ToString("yyyy-MM-dd");
        TouchDailyActivity();
        SaveState();
    }

    public bool TryUnlockShopItem(string itemId, int costStars)
    {
        if (IsShopItemUnlocked(itemId))
            return true;

        if (AvailableStars < costStars)
            return false;

        _state.StarsSpent += costStars;
        _state.UnlockedShopIds.Add(itemId);
        SaveState();
        return true;
    }

    public void SetEquippedTheme(string themeKey)
    {
        _state.EquippedTheme = string.IsNullOrWhiteSpace(themeKey) ? "default" : themeKey;
        SaveState();
    }

    private void TouchDailyActivity()
    {
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        if (_state.LastActiveDate == today)
            return;

        var yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
        _state.DailyStreakDays = _state.LastActiveDate == yesterday
            ? _state.DailyStreakDays + 1
            : 1;
        _state.LastActiveDate = today;
    }

    private static PlayerProgressState LoadState()
    {
        var json = Preferences.Default.Get(KeyProgressJson, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
            return new PlayerProgressState();

        try
        {
            var parsed = JsonSerializer.Deserialize<PlayerProgressState>(json);
            if (parsed is not null)
            {
                parsed.WordMastery = new Dictionary<string, int>(parsed.WordMastery, StringComparer.OrdinalIgnoreCase);
                parsed.WeakWords = new Dictionary<string, int>(parsed.WeakWords, StringComparer.OrdinalIgnoreCase);
                parsed.UnlockedShopIds = new HashSet<string>(parsed.UnlockedShopIds, StringComparer.OrdinalIgnoreCase);
                return parsed;
            }
        }
        catch
        {
            // Fallback cleanly on corrupted state
        }
        return new PlayerProgressState();
    }

    private void SaveState()
    {
        var json = JsonSerializer.Serialize(_state);
        Preferences.Default.Set(KeyProgressJson, json);
    }
}
