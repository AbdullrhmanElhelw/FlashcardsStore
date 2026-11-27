using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Offline-first English Text-to-Speech helper that speaks Jolly Phonics phoneme sounds, not letter names.</summary>
public sealed class SpeechService : ISpeechService
{
    private static readonly Dictionary<string, string> PhonemeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["s"] = "ssss",
        ["a"] = "ah",
        ["t"] = "tuh",
        ["i"] = "ih",
        ["p"] = "puh",
        ["n"] = "nnnn",
        ["c"] = "kuh",
        ["k"] = "kuh",
        ["ck"] = "kuh",
        ["e"] = "eh",
        ["h"] = "huh",
        ["r"] = "rrrr",
        ["m"] = "mmmm",
        ["d"] = "duh",
        ["g"] = "guh",
        ["o"] = "aw",
        ["u"] = "uh",
        ["l"] = "llll",
        ["f"] = "ffff",
        ["b"] = "buh",
        ["ai"] = "ay",
        ["j"] = "juh",
        ["oa"] = "oh",
        ["ie"] = "eye",
        ["ee"] = "eee",
        ["or"] = "or",
        ["z"] = "zzzz",
        ["w"] = "wuh",
        ["ng"] = "nng",
        ["v"] = "vvvv",
        ["oo"] = "ooo",
        ["y"] = "yuh",
        ["x"] = "ksss",
        ["ch"] = "chuh",
        ["sh"] = "shhh",
        ["th"] = "thhh",
        ["qu"] = "kwuh",
        ["ou"] = "oww",
        ["oi"] = "oy",
        ["ue"] = "yoo",
        ["er"] = "err",
        ["ar"] = "arrr"
    };

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
        => SpeakPhonemeAsync(letter.ToString());

    public Task SpeakPhonemeAsync(string sound)
    {
        var key = sound.Trim().ToLowerInvariant();
        var spoken = PhonemeMap.TryGetValue(key, out var phoneme) ? phoneme : key;
        return SpeakAsync(spoken, pitch: 1.12f, rate: 0.82f);
    }
}
