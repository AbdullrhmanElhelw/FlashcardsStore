using FlashcardsStore.Mobile.Services.Abstractions;
#if ANDROID
using Android.Media;
#endif

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiAudioPlayerService : IAudioPlayerService, IDisposable
{
#if ANDROID
    private MediaPlayer? _player;
    private string? _currentUrl;
#endif

    public bool IsPlaying { get; private set; }
    public event EventHandler<bool>? PlaybackStateChanged;

    public async Task TogglePlayAsync(string audioUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(audioUrl)) return;

#if ANDROID
        if (IsPlaying && _currentUrl == audioUrl)
        {
            Stop();
            return;
        }

        Stop();
        _currentUrl = audioUrl;

        var tcs = new TaskCompletionSource<bool>();
        var player = new MediaPlayer();
        _player = player;

        player.SetAudioAttributes(new AudioAttributes.Builder()
            .SetContentType(AudioContentType.Speech)!
            .SetUsage(AudioUsageKind.Media)!
            .Build());

        player.Prepared += (_, _) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetResult(false);
                return;
            }
            player.Start();
            SetPlaying(true);
            tcs.TrySetResult(true);
        };

        player.Completion += (_, _) =>
        {
            SetPlaying(false);
        };

        player.Error += (_, _) =>
        {
            SetPlaying(false);
            tcs.TrySetException(new InvalidOperationException("تعذر تشغيل الملف الصوتي"));
        };

        await player.SetDataSourceAsync(audioUrl);
        player.PrepareAsync();
        await tcs.Task;
#else
        await Launcher.Default.OpenAsync(audioUrl);
#endif
    }

    public void Stop()
    {
#if ANDROID
        if (_player is not null)
        {
            try
            {
                if (_player.IsPlaying) _player.Stop();
            }
            catch { }
            _player.Release();
            _player.Dispose();
            _player = null;
        }
        _currentUrl = null;
#endif
        SetPlaying(false);
    }

    public void Dispose() => Stop();

    private void SetPlaying(bool playing)
    {
        if (IsPlaying == playing) return;
        IsPlaying = playing;
        MainThread.BeginInvokeOnMainThread(() => PlaybackStateChanged?.Invoke(this, playing));
    }
}
