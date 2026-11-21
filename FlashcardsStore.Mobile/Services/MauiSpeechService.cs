using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiSpeechService : ISpeechService
{
    private CancellationTokenSource? _activeSpeech;

    public async Task SpeakAsync(string text, bool slowForKids = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        _activeSpeech?.Cancel();
        _activeSpeech?.Dispose();
        _activeSpeech = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var englishLocale = locales.FirstOrDefault(l => l.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));

        var options = new SpeechOptions
        {
            Locale = englishLocale,
            Pitch = slowForKids ? 1.05f : 1.0f,
            Volume = 1.0f
        };

        // Note: On .NET MAUI TextToSpeech, Pitch/Volume are cross-platform; slow mode adds gentle spacing between words if needed.
        var spokenText = slowForKids
            ? string.Join(", ", text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            : text;

        try
        {
            await TextToSpeech.Default.SpeakAsync(spokenText, options, _activeSpeech.Token);
        }
        catch (OperationCanceledException)
        {
            // User triggered another pronunciation before the previous one finished.
        }
    }
}
