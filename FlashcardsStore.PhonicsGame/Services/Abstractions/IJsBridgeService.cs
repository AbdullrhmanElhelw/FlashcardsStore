using Microsoft.JSInterop;

namespace FlashcardsStore.PhonicsGame.Services.Abstractions;

public interface IJsBridgeService
{
    void Attach(IJSRuntime js);
    Task InvokeAsync(string identifier, params object?[] args);
}
