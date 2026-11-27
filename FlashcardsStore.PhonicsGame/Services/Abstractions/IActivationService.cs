using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IActivationService
{
    event Action? ActivationChanged;
    bool IsActivated { get; }
    string ActivatedCode { get; }
    string BundleName { get; }
    string GetOrCreateDeviceId();
    Task<ActivationResult> ActivateAsync(string rawCode, CancellationToken ct = default);
    Task<bool> VerifyActivationAsync(CancellationToken ct = default);
}
