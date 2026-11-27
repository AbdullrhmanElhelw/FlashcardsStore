using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages Jolly Phonics Sound Cards (Groups 1-7 starting with S A T I P N), actions, phoneme audio, and tracing canvas.</summary>
public sealed partial class SoundboardViewModel : BaseViewModel
{
    private readonly ICurriculumRepository _curriculum;
    private readonly ISpeechService _speech;
    private readonly IJsBridgeService _js;

    [ObservableProperty] private int _selectedGroup = 1;
    [ObservableProperty] private PhonicsLetterItem? _activeItem;

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

    public IReadOnlyList<PhonicsLetterItem> FilteredLetters => SelectedGroup == 0
        ? Letters
        : Letters.Where(l => l.Group == SelectedGroup).ToList();

    public char? ActiveLetter => ActiveItem?.Letter;

    public void Reset()
    {
        SelectedGroup = 1;
        ActiveItem = Letters.FirstOrDefault(l => l.Group == 1);
    }

    [RelayCommand]
    public void SetGroup(int group)
    {
        SelectedGroup = group;
        ActiveItem = FilteredLetters.FirstOrDefault();
    }

    [RelayCommand]
    public async Task SpeakLetterAsync(PhonicsLetterItem item)
    {
        ActiveItem = item;
        await _js.InvokeAsync("phonicsAudio.pop");
        await _js.InvokeAsync("phonicsTracing.init", "letterTraceCanvas");
        await _speech.SpeakPhonemeAsync(item.DisplaySound);
    }

    [RelayCommand]
    public Task SpeakExampleWordAsync()
        => ActiveItem is null ? Task.CompletedTask : _speech.SpeakAsync(ActiveItem.ExampleWord, pitch: 1.08f);

    [RelayCommand]
    public Task ClearCanvasAsync() => _js.InvokeAsync("phonicsTracing.clear");

    public Task SetColorAsync(string color) => _js.InvokeAsync("phonicsTracing.setColor", color);
}
