using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IChildProfileService
{
    ChildProfile GetProfile();
    void SaveProfile(string name, string avatarKey);
}
