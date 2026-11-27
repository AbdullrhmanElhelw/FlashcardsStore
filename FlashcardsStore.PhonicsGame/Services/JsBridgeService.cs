using FlashcardsStore.PhonicsGame.Services.Abstractions;
using Microsoft.JSInterop;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Thread-safe wrapper around Blazor IJSRuntime shared across all ViewModels.</summary>
public sealed class JsBridgeService : IJsBridgeService
{
    private IJSRuntime? _js;

    public void Attach(IJSRuntime js) => _js = js;

    public async Task InvokeAsync(string identifier, params object?[] args)
    {
        if (_js is null) return;
        try
        {
            await _js.InvokeVoidAsync(identifier, args);
        }
        catch
        {
            // Ignore transient WebView JS interop errors
        }
    }
}
