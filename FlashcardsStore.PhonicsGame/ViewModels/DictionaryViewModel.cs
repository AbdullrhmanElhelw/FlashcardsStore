using CommunityToolkit.Mvvm.ComponentModel;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages the 120-word Picture Dictionary with search and stage filtering.</summary>
public sealed partial class DictionaryViewModel : BaseViewModel
{
    private readonly ICurriculumRepository _curriculum;
    private readonly IGameProgressService _progress;
    private readonly IActivationService _activation;
    private readonly ISpeechService _speech;
    private readonly IJsBridgeService _js;
    private readonly ModalsViewModel _modals;

    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private int _selectedStageId;

    public DictionaryViewModel(
        ICurriculumRepository curriculum,
        IGameProgressService progress,
        IActivationService activation,
        ISpeechService speech,
        IJsBridgeService js,
        ModalsViewModel modals)
    {
        _curriculum = curriculum;
        _progress = progress;
        _activation = activation;
        _speech = speech;
        _js = js;
        _modals = modals;
        _curriculum.ContentReloaded += NotifyAll;
    }

    public IReadOnlyList<PhonicsStage> Stages => _curriculum.GetStages();

    public void Reset()
    {
        SearchQuery = string.Empty;
        SelectedStageId = 0;
    }

    public IReadOnlyList<(PhonicsWord Word, bool Unlocked, int Mastery)> GetFilteredWords()
    {
        var term = (SearchQuery ?? string.Empty).Trim();
        var results = new List<(PhonicsWord, bool, int)>();
        foreach (var stage in Stages)
        {
            if (SelectedStageId > 0 && stage.Id != SelectedStageId)
                continue;

            var unlocked = stage.IsFree || _activation.IsActivated;
            foreach (var w in stage.Words)
            {
                if (!string.IsNullOrEmpty(term) &&
                    !w.Word.Contains(term, StringComparison.OrdinalIgnoreCase) &&
                    !w.ArabicMeaning.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                results.Add((w, unlocked, _progress.GetWordMastery(w.Word)));
            }
        }
        return results;
    }

    public async Task HandleCardTapAsync(string word, bool unlocked)
    {
        if (!unlocked)
        {
            _modals.OpenActivationModal();
            return;
        }
        await _js.InvokeAsync("phonicsAudio.pop");
        await _speech.SpeakAsync(word, pitch: 1.08f);
    }
}
