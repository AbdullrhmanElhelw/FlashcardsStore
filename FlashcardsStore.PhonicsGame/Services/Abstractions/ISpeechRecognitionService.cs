using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface ISpeechRecognitionService
{
    Task<SpeechCheckResult> ListenAndCheckWordAsync(string expectedWord, string prompt);
}
