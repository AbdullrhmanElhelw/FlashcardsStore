namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface ISpeechService
{
    Task SpeakAsync(string text, bool slowForKids = false, CancellationToken cancellationToken = default);
}
