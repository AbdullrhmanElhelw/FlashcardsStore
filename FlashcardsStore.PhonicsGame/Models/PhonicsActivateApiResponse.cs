namespace FlashcardsStore.PhonicsGame.Models;

public sealed record PhonicsActivateApiResponse(
    bool IsActivated,
    string? Code,
    string? BundleName,
    int MaxDevices,
    int UsedDevices,
    int RemainingDevices,
    string? MessageAr,
    string? MessageEn);
