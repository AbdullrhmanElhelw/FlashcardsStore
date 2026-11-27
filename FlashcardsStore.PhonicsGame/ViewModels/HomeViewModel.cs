using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages Home dashboard state: categories, stages, stories, achievements, and navigation.</summary>
public sealed partial class HomeViewModel : BaseViewModel
{
    private readonly ICurriculumRepository _curriculum;
    private readonly IGameProgressService _progress;
    private readonly IActivationService _activation;
    private readonly AppShellViewModel _shell;
    private readonly GameArenaViewModel _arena;
    private readonly StoryPlayerViewModel _storyPlayer;
    private readonly SoundboardViewModel _soundboard;
    private readonly DictionaryViewModel _dictionary;
    private readonly ModalsViewModel _modals;

    [ObservableProperty] private string _selectedCategory = "all";

    public HomeViewModel(
        ICurriculumRepository curriculum,
        IGameProgressService progress,
        IActivationService activation,
        AppShellViewModel shell,
        GameArenaViewModel arena,
        StoryPlayerViewModel storyPlayer,
        SoundboardViewModel soundboard,
        DictionaryViewModel dictionary,
        ModalsViewModel modals,
        OnboardingViewModel onboarding)
    {
        _curriculum = curriculum;
        _progress = progress;
        _activation = activation;
        _shell = shell;
        _arena = arena;
        _storyPlayer = storyPlayer;
        _soundboard = soundboard;
        _dictionary = dictionary;
        _modals = modals;

        _curriculum.ContentReloaded += NotifyAll;
        onboarding.OnRequestStoriesStart = () =>
        {
            var first = Stories.FirstOrDefault(s => s.IsFree);
            if (first is not null) _ = _storyPlayer.StartAsync(first);
            else _shell.NavigateTo(AppScreen.Home);
        };
        onboarding.OnRequestSentencesStart = () => _ = _arena.StartSentenceChallengeAsync();
    }

    public IReadOnlyList<PhonicsStage> Stages => _curriculum.GetStages();
    public IReadOnlyList<PhonicsStory> Stories => _curriculum.GetStories();
    public int TotalWordsCount => _curriculum.TotalWordsCount;
    public int TotalXp => _progress.TotalXp;
    public int TotalStars => _progress.TotalStars;
    public int BestStreak => _progress.BestStreak;
    public bool IsDailyDoneToday => _progress.IsDailyChallengeCompletedToday;
    public IReadOnlyList<string> WeakWords => _progress.GetWeakWords();
    public HashSet<string> CompletedWords => _progress.GetCompletedWords();
    public bool IsActivated => _activation.IsActivated;
    public ChildProfile Profile => _modals.Profile;
    public GameMode SelectedMode => _arena.SelectedMode;

    public bool IsStoryCompleted(int storyId) => _progress.IsStoryCompleted(storyId);

    public void SetMode(GameMode mode)
    {
        _arena.SelectedMode = mode;
        NotifyAll();
    }

    [RelayCommand]
    public void SetCategory(string category)
        => SelectedCategory = string.IsNullOrWhiteSpace(category) ? "all" : category;

    public IReadOnlyList<PhonicsStage> GetFilteredStages() => SelectedCategory switch
    {
        "short" => Stages.Where(s => s.Id is >= 1 and <= 4).ToList(),
        "blends" => Stages.Where(s => s.Id is 5 or 6).ToList(),
        "sight" => Stages.Where(s => s.Id is 7 or 8).ToList(),
        "magice" => Stages.Where(s => s.Id is 9 or 10).ToList(),
        "advanced" => Stages.Where(s => s.Id is 11 or 12).ToList(),
        _ => Stages
    };

    public IReadOnlyList<AchievementItem> GetAchievements() =>
    [
        new("first_word", "أول كلمة", "First Word", CompletedWords.Count >= 1),
        new("ten_stars", "10 نجوم", "10 Stars", TotalStars >= 10),
        new("streak_five", "سلسلة 5", "5 Streak", BestStreak >= 5),
        new("twenty_words", "20 كلمة", "20 Words", CompletedWords.Count >= 20)
    ];

    public Task StartStageAsync(PhonicsStage stage) => _arena.StartStageAsync(stage);
    public Task StartStoryAsync(PhonicsStory story) => _storyPlayer.StartAsync(story);
    public Task StartDailyChallengeAsync() => _arena.StartDailyChallengeAsync();
    public Task StartWeakWordsReviewAsync() => _arena.StartWeakWordsReviewAsync();
    public Task StartSentenceChallengeAsync() => _arena.StartSentenceChallengeAsync();
    public Task StartRhymingChallengeAsync() => _arena.StartRhymingChallengeAsync();
    public Task StartMinimalPairsChallengeAsync() => _arena.StartMinimalPairsChallengeAsync();

    public void OpenSoundboard()
    {
        _soundboard.Reset();
        _shell.NavigateTo(AppScreen.Soundboard);
    }

    public void OpenDictionary()
    {
        _dictionary.Reset();
        _shell.NavigateTo(AppScreen.Dictionary);
    }
}
