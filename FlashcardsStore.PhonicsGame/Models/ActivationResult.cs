namespace FlashcardsStore.PhonicsGame.Models;

public sealed record ActivationResult(
    bool Success,
    string MessageAr,
    string MessageEn,
    string? BundleName = null);
