using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Offline-first English Text-to-Speech helper for phonics letters and words.</summary>
public sealed class SpeechService : ISpeechService
{
    private CancellationTokenSource? _cts;
    private Locale? _englishLocale;
    private bool _localeResolved;

    public async Task SpeakAsync(string text, float pitch = 1.05f, float rate = 0.88f)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            if (!_localeResolved)
            {
                _localeResolved = true;
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                _englishLocale = locales.FirstOrDefault(l =>
                    l.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            }

            var options = new SpeechOptions
            {
                Pitch = pitch,
                Volume = 1.0f,
                Locale = _englishLocale
            };

            await TextToSpeech.Default.SpeakAsync(text, options, token);
        }
        catch (OperationCanceledException)
        {
            // Expected when the child taps another tile quickly.
        }
        catch
        {
            // Ignore TTS device errors gracefully so gameplay never crashes.
        }
    }

    public Task SpeakLetterAsync(char letter)
        => SpeakAsync(letter.ToString().ToLowerInvariant(), pitch: 1.15f);
}
