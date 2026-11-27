using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Parent-gated progress dashboard showing Jolly Phonics mastery and weak areas.</summary>
public sealed partial class ParentReportViewModel : BaseViewModel
{
    private readonly Random _rng = new();
    private readonly ICurriculumRepository _curriculum;
    private readonly IGameProgressService _progress;
    private readonly AppShellViewModel _shell;
    private readonly GameArenaViewModel _arena;

    [ObservableProperty] private bool _isGateUnlocked;
    [ObservableProperty] private int _gateA;
    [ObservableProperty] private int _gateB;
    [ObservableProperty] private string _gateAnswer = string.Empty;
    [ObservableProperty] private bool _gateWrong;

    public ParentReportViewModel(
        ICurriculumRepository curriculum,
        IGameProgressService progress,
        AppShellViewModel shell,
        GameArenaViewModel arena)
    {
        _curriculum = curriculum;
        _progress = progress;
        _shell = shell;
        _arena = arena;
    }

    public int TotalWords => _curriculum.TotalWordsCount;
    public int MasteredCount => _progress.GetCompletedWords().Count;
    public int TotalStars => _progress.TotalStars;
    public int TotalXp => _progress.TotalXp;
    public int BestStreak => _progress.BestStreak;
    public IReadOnlyList<string> WeakWords => _progress.GetWeakWords();
    public IReadOnlyList<PhonicsStage> Stages => _curriculum.GetStages();
    public HashSet<string> CompletedWords => _progress.GetCompletedWords();

    public void Open()
    {
        IsGateUnlocked = false;
        GateAnswer = string.Empty;
        GateWrong = false;
        GateA = _rng.Next(4, 10);
        GateB = _rng.Next(3, 9);
        _shell.NavigateTo(AppScreen.ParentReport);
    }

    [RelayCommand]
    public void VerifyGate()
    {
        if (int.TryParse(GateAnswer.Trim(), out var ans) && ans == GateA * GateB)
        {
            GateWrong = false;
            IsGateUnlocked = true;
        }
        else
        {
            GateWrong = true;
            GateAnswer = string.Empty;
        }
    }

    [RelayCommand]
    public Task PracticeWeakWordsAsync() => _arena.StartWeakWordsReviewAsync();
}
