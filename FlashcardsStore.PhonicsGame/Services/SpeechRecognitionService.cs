using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Checks child pronunciation using Android's native speech recognizer.</summary>
public sealed class SpeechRecognitionService : ISpeechRecognitionService
{
    public async Task<SpeechCheckResult> ListenAndCheckWordAsync(string expectedWord, string prompt)
    {
#if ANDROID
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
                return new SpeechCheckResult(false, false, string.Empty);

            var recognized = await MainActivity.RecognizeSpeechAsync(prompt);
            if (recognized.Count == 0)
                return new SpeechCheckResult(true, false, string.Empty);

            var expected = expectedWord.Trim().ToLowerInvariant();
            var matched = recognized.Any(r =>
            {
                var clean = r.Trim().ToLowerInvariant();
                return clean == expected || clean.Split(' ').Contains(expected);
            });

            return new SpeechCheckResult(true, matched, recognized[0]);
        }
        catch
        {
            return new SpeechCheckResult(false, false, string.Empty);
        }
#else
        await Task.CompletedTask;
        return new SpeechCheckResult(false, false, string.Empty);
#endif
    }
}
