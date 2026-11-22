namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface IAudioPlayerService
{
    bool IsPlaying { get; }
    event EventHandler<bool>? PlaybackStateChanged;
    Task TogglePlayAsync(string audioUrl, CancellationToken cancellationToken = default);
    void Stop();
}
