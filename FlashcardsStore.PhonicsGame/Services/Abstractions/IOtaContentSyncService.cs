namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IOtaContentSyncService
{
    int CurrentContentVersion { get; }
    Task<bool> SyncIfNeededAsync(CancellationToken ct = default);
}
