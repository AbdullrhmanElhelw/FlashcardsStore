using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages the A-Z Phonics Soundboard and finger-tracing canvas.</summary>
public sealed partial class SoundboardViewModel : BaseViewModel
{
    private readonly ICurriculumRepository _curriculum;
    private readonly ISpeechService _speech;
    private readonly IJsBridgeService _js;

    [ObservableProperty] private char? _activeLetter;

    public SoundboardViewModel(
        ICurriculumRepository curriculum,
        ISpeechService speech,
        IJsBridgeService js)
    {
        _curriculum = curriculum;
        _speech = speech;
        _js = js;
        _curriculum.ContentReloaded += NotifyAll;
    }

    public IReadOnlyList<PhonicsLetterItem> Letters => _curriculum.GetAlphabet();

    public void Reset() => ActiveLetter = null;

    [RelayCommand]
    public async Task SpeakLetterAsync(PhonicsLetterItem item)
    {
        ActiveLetter = item.Letter;
        await _js.InvokeAsync("phonicsAudio.pop");
        await _js.InvokeAsync("phonicsTracing.init", "letterTraceCanvas");
        await _speech.SpeakAsync($"{item.Letter}, {item.ExampleWord}", pitch: 1.1f);
    }

    [RelayCommand]
    public Task ClearCanvasAsync() => _js.InvokeAsync("phonicsTracing.clear");

    public Task SetColorAsync(string color) => _js.InvokeAsync("phonicsTracing.setColor", color);
}
