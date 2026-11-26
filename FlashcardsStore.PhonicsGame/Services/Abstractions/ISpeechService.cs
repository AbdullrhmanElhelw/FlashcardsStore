namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface ISpeechService
{
    Task SpeakAsync(string text, float pitch = 1.05f, float rate = 0.88f);
    Task SpeakLetterAsync(char letter);
}
