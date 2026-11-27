using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages animated Fill-in-the-Blank Phonics Stories and end-of-story comprehension quizzes.</summary>
public sealed partial class StoryPlayerViewModel : BaseViewModel
{
    private readonly Random _rng = new();
    private readonly IActivationService _activation;
    private readonly IGameProgressService _progress;
    private readonly ISpeechService _speech;
    private readonly IJsBridgeService _js;
    private readonly ModalsViewModel _modals;
    private readonly AppShellViewModel _shell;

    [ObservableProperty] private PhonicsStory? _activeStory;
    [ObservableProperty] private int _sceneIndex;
    [ObservableProperty] private bool _inQuiz;
    [ObservableProperty] private bool _quizSolved;
    [ObservableProperty] private string? _quizWrongOption;
    [ObservableProperty] private bool _blankSolved;
    [ObservableProperty] private List<string> _blankOptions = [];
    [ObservableProperty] private string? _blankWrongOption;

    public StoryPlayerViewModel(
        IActivationService activation,
        IGameProgressService progress,
        ISpeechService speech,
        IJsBridgeService js,
        ModalsViewModel modals,
        AppShellViewModel shell)
    {
        _activation = activation;
        _progress = progress;
        _speech = speech;
        _js = js;
        _modals = modals;
        _shell = shell;
    }

    public async Task StartAsync(PhonicsStory story)
    {
        if (!story.IsFree && !_activation.IsActivated)
        {
            _modals.OpenActivationModal();
            return;
        }

        ActiveStory = story;
        SceneIndex = 0;
        InQuiz = false;
        QuizSolved = false;
        QuizWrongOption = null;
        SetupBlank();
        _shell.NavigateTo(AppScreen.StoryPlayer);
        await SpeakSceneAsync();
    }

    private void SetupBlank()
    {
        BlankSolved = false;
        BlankWrongOption = null;
        if (ActiveStory is null) return;

        var scene = ActiveStory.Scenes[SceneIndex];
        var distractors = ActiveStory.Scenes
            .Select(s => s.FocusWord)
            .Where(w => !string.Equals(w, scene.FocusWord, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(_ => _rng.Next())
            .Take(2)
            .ToList();

        distractors.Add(scene.FocusWord);
        BlankOptions = distractors.OrderBy(_ => _rng.Next()).ToList();
    }

    public async Task SelectBlankOptionAsync(string option)
    {
        if (ActiveStory is null || BlankSolved) return;
        var scene = ActiveStory.Scenes[SceneIndex];
        if (string.Equals(option, scene.FocusWord, StringComparison.OrdinalIgnoreCase))
        {
            BlankSolved = true;
            BlankWrongOption = null;
            await _js.InvokeAsync("phonicsAudio.success");
            await _speech.SpeakAsync(scene.SentenceEn, pitch: 1.08f);
        }
        else
        {
            BlankWrongOption = option;
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            BlankWrongOption = null;
        }
    }

    [RelayCommand]
    public Task SpeakSceneAsync()
    {
        if (ActiveStory is null || InQuiz) return Task.CompletedTask;
        return _speech.SpeakAsync(ActiveStory.Scenes[SceneIndex].SentenceEn, pitch: 1.08f);
    }

    [RelayCommand]
    public async Task NextSceneAsync()
    {
        if (ActiveStory is null) return;
        if (SceneIndex + 1 < ActiveStory.Scenes.Count)
        {
            SceneIndex++;
            SetupBlank();
            await SpeakSceneAsync();
        }
        else
        {
            InQuiz = true;
            await _speech.SpeakAsync(ActiveStory.QuizQuestionEn, pitch: 1.08f);
        }
    }

    [RelayCommand]
    public async Task PrevSceneAsync()
    {
        if (ActiveStory is null) return;
        if (InQuiz)
        {
            InQuiz = false;
            return;
        }
        if (SceneIndex > 0)
        {
            SceneIndex--;
            SetupBlank();
            await SpeakSceneAsync();
        }
    }

    public async Task AnswerQuizAsync(string option)
    {
        if (ActiveStory is null || QuizSolved) return;

        if (string.Equals(option, ActiveStory.QuizCorrectWord, StringComparison.OrdinalIgnoreCase))
        {
            QuizSolved = true;
            QuizWrongOption = null;
            _progress.RecordStoryCompleted(ActiveStory.Id, 3);
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
            await _speech.SpeakAsync($"Great job! {ActiveStory.QuizCorrectWord}", pitch: 1.15f);
        }
        else
        {
            QuizWrongOption = option;
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            QuizWrongOption = null;
        }
    }
}
