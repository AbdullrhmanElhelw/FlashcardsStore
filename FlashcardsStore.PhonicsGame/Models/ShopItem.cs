namespace FlashcardsStore.PhonicsGame.Models;

public sealed record ShopItem(
    string Id,
    string Kind,
    string Key,
    string TitleAr,
    string TitleEn,
    int CostStars,
    string PreviewColor);
