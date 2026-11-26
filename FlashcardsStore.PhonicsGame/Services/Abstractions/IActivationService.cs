using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IActivationService
{
    bool IsActivated { get; }
    string ActivatedCode { get; }
    string BundleName { get; }
    string GetOrCreateDeviceId();
    Task<ActivationResult> ActivateAsync(string rawCode, CancellationToken ct = default);
}
