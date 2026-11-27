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
    private readonly ParentReportViewModel _parentReport;

    [ObservableProperty] private string _selectedCategory = "letters";

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
        ParentReportViewModel parentReport,
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
        _parentReport = parentReport;

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

    public IReadOnlyList<PhonicsStage> GetFilteredStages()
        => Stages.Where(s => string.Equals(s.Category, SelectedCategory, StringComparison.OrdinalIgnoreCase)).ToList();

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
    public Task StartGrammarTopicAsync(string topic, bool isFree) => _arena.StartGrammarChallengeAsync(topic, isFree);
    public Task StartListenChooseChallengeAsync() => _arena.StartListenChooseChallengeAsync();
    public Task StartSpeakRepeatChallengeAsync() => _arena.StartSpeakRepeatChallengeAsync();
    public Task StartListenCompleteChallengeAsync() => _arena.StartListenCompleteChallengeAsync();
    public Task StartBalloonPopAsync() => _arena.StartBalloonPopAsync();
    public Task StartSoundSegmentAsync() => _arena.StartSoundSegmentAsync();
    public Task StartSoundPositionAsync() => _arena.StartSoundPositionAsync();
    public Task StartDecodableReaderAsync() => _arena.StartDecodableReaderAsync();
    public void OpenParentReport() => _parentReport.Open();

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

    [RelayCommand]
    public async Task OpenStoreCoursesAsync()
    {
        try
        {
            await Launcher.Default.OpenAsync(new Uri("https://card-store.runasp.net/"));
        }
        catch
        {
            // Ignore if browser cannot be opened on device
        }
    }
}
