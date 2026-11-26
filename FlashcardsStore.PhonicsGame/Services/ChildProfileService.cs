using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Persists the child's display name, selected vector avatar, and daily goal.</summary>
public sealed class ChildProfileService : IChildProfileService
{
    private const string KeyName = "phonics_child_name";
    private const string KeyAvatar = "phonics_child_avatar";
    private const string KeyDailyGoal = "phonics_daily_goal";

    public ChildProfile GetProfile()
    {
        var name = Preferences.Default.Get(KeyName, string.Empty);
        var avatar = Preferences.Default.Get(KeyAvatar, "lion");
        var goal = Preferences.Default.Get(KeyDailyGoal, 10);
        return new ChildProfile(name, avatar, goal);
    }

    public void SaveProfile(string name, string avatarKey, int dailyGoalMinutes = 10)
    {
        Preferences.Default.Set(KeyName, (name ?? string.Empty).Trim());
        Preferences.Default.Set(KeyAvatar, string.IsNullOrWhiteSpace(avatarKey) ? "lion" : avatarKey);
        Preferences.Default.Set(KeyDailyGoal, Math.Clamp(dailyGoalMinutes, 5, 30));
    }
}
