using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Persists the child's display name and selected vector avatar.</summary>
public sealed class ChildProfileService : IChildProfileService
{
    private const string KeyName = "phonics_child_name";
    private const string KeyAvatar = "phonics_child_avatar";

    public ChildProfile GetProfile()
    {
        var name = Preferences.Default.Get(KeyName, string.Empty);
        var avatar = Preferences.Default.Get(KeyAvatar, "lion");
        return new ChildProfile(name, avatar);
    }

    public void SaveProfile(string name, string avatarKey)
    {
        Preferences.Default.Set(KeyName, (name ?? string.Empty).Trim());
        Preferences.Default.Set(KeyAvatar, string.IsNullOrWhiteSpace(avatarKey) ? "lion" : avatarKey);
    }
}
