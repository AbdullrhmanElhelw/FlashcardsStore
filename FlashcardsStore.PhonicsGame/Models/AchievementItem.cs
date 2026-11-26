namespace FlashcardsStore.PhonicsGame.Models;

public sealed record AchievementItem(
    string Id,
    string TitleAr,
    string TitleEn,
    bool Unlocked);
