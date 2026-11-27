using FlashcardsStore.PhonicsGame.Data;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;
using Microsoft.JSInterop;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Encapsulates all game state, navigation, drag-and-drop, and audio orchestration away from Razor UI views.</summary>
public sealed class PhonicsGameViewModel : IDisposable
{
    private const string KeyOnboardingDone = "phonics_onboarding_done";
    private readonly Random _rng = new();
    private readonly ICurriculumRepository _curriculum;
    private readonly IOtaContentSyncService _otaSync;
    private readonly ISpeechService _speech;
    private readonly ISpeechRecognitionService _speechRec;
    private readonly IChildProfileService _profileService;
    private readonly IActivationService _activation;
    private readonly IGameProgressService _progress;
    private readonly ILocalizationService _loc;
    private IJSRuntime? _js;
    private DotNetObjectReference<PhonicsGameViewModel>? _dotNetRef;

    public event Action? StateChanged;

    public PhonicsGameViewModel(
        ICurriculumRepository curriculum,
        IOtaContentSyncService otaSync,
        ISpeechService speech,
        ISpeechRecognitionService speechRec,
        IChildProfileService profileService,
        IActivationService activation,
        IGameProgressService progress,
        ILocalizationService loc)
    {
        _curriculum = curriculum;
        _otaSync = otaSync;
        _speech = speech;
        _speechRec = speechRec;
        _profileService = profileService;
        _activation = activation;
        _progress = progress;
        _loc = loc;
        _loc.Changed += Notify;
        _curriculum.ContentReloaded += Notify;
        CompletedWords = _progress.GetCompletedWords();
        Profile = _profileService.GetProfile();
        ProfileNameInput = Profile.Name;
        ProfileAvatarInput = Profile.AvatarKey;
        OnboardingGoalMinutes = Profile.DailyGoalMinutes;
    }

    public IReadOnlyList<PhonicsStage> Stages => _curriculum.GetStages();
    public IReadOnlyList<PhonicsStory> Stories => _curriculum.GetStories();
    public IReadOnlyList<PhonicsLetterItem> Alphabet => _curriculum.GetAlphabet();
    public IReadOnlyList<ShopItem> ShopItems => _curriculum.GetShopItems();
    public int TotalWordsCount => _curriculum.TotalWordsCount;
    public int AvailableStars => _progress.AvailableStars;
    public IReadOnlyList<string> WeakWords => _progress.GetWeakWords();
    public string EquippedTheme => _progress.EquippedTheme;
    public string ShopFeedback { get; private set; } = string.Empty;
    public int TotalXp => _progress.TotalXp;
    public int DailyStreakDays => _progress.DailyStreakDays;
    public int OnboardingGoalMinutes { get; set; } = 10;
    public bool IsStoryCompleted(int storyId) => _progress.IsStoryCompleted(storyId);

    public AppScreen CurrentScreen { get; private set; } = AppScreen.Splash;
    public int OnboardingSlide { get; private set; }
    public GameMode SelectedMode { get; private set; } = GameMode.WordBuilder;
    public HashSet<string> CompletedWords { get; private set; }

    public PhonicsStage? ActiveStage { get; private set; }
    public int WordIndex { get; private set; }
    public int StageStars { get; private set; }
    public int Streak { get; private set; }
    public bool StageFinished { get; private set; }
    public bool WordSolved { get; private set; }

    public int[] Slots { get; private set; } = [];
    public List<LetterTileItem> Tiles { get; private set; } = [];
    public bool BuilderWrongFlash { get; private set; }

    public List<PhonicsWord> MatchOptions { get; private set; } = [];
    public string? WrongMatchWord { get; private set; }

    public List<MemoryCardItem> MemoryCards { get; private set; } = [];
    private MemoryCardItem? _firstFlippedCard;
    private bool _memoryBusy;

    public char? ActiveSoundboardLetter { get; private set; }

    public ChildProfile Profile { get; private set; }
    public bool ShowProfileModal { get; private set; }
    public string ProfileNameInput { get; set; } = string.Empty;
    public string ProfileAvatarInput { get; set; } = "lion";

    public bool IsDailyChallengeMode { get; private set; }
    public bool IsDailyChallengeDoneToday => _progress.IsDailyChallengeCompletedToday;

    public string SpeechFeedback { get; private set; } = string.Empty;
    public bool SpeechSuccess { get; private set; }
    public bool IsListeningSpeech { get; private set; }

    public PhonicsStory? ActiveStory { get; private set; }
    public int StorySceneIndex { get; private set; }
    public bool InStoryQuiz { get; private set; }
    public bool StoryQuizSolved { get; private set; }
    public string? StoryQuizWrongOption { get; private set; }

    // Sentence Builder state
    public IReadOnlyList<SentenceChallenge> SentenceList { get; private set; } = [];
    public int SentenceIndex { get; private set; }
    public SentenceChallenge? CurrentSentence => SentenceIndex < SentenceList.Count ? SentenceList[SentenceIndex] : null;
    public int[] SentenceSlots { get; private set; } = [];
    public List<(string Word, bool Used)> SentenceBank { get; private set; } = [];
    public bool SentenceWrongFlash { get; private set; }

    // Rhyming Pairs state
    public IReadOnlyList<RhymeChallenge> RhymeList { get; private set; } = [];
    public int RhymeIndex { get; private set; }
    public RhymeChallenge? CurrentRhyme => RhymeIndex < RhymeList.Count ? RhymeList[RhymeIndex] : null;
    public List<string> RhymeOptions { get; private set; } = [];
    public string? RhymeWrongOption { get; private set; }

    // Minimal Pairs state
    public IReadOnlyList<MinimalPairChallenge> MinimalPairList { get; private set; } = [];
    public int MinimalPairIndex { get; private set; }
    public MinimalPairChallenge? CurrentMinimalPair => MinimalPairIndex < MinimalPairList.Count ? MinimalPairList[MinimalPairIndex] : null;
    public bool MinimalTargetIsLong { get; private set; }
    public string? MinimalWrongWord { get; private set; }

    // Home Category filter
    public string SelectedCategory { get; private set; } = "all";

    public bool ShowActivationModal { get; private set; }
    public string ActivationCodeInput { get; set; } = string.Empty;
    public string ActivationFeedback { get; private set; } = string.Empty;
    public bool ActivationSuccess { get; private set; }
    public bool IsActivating { get; private set; }

    public PhonicsWord CurrentWord => ActiveStage!.Words[WordIndex];
    public int TotalStars => _progress.TotalStars;
    public int BestStreak => _progress.BestStreak;
    public bool IsActivated => _activation.IsActivated;

    public async Task InitializeAsync(IJSRuntime js)
    {
        _js = js;
        _dotNetRef ??= DotNetObjectReference.Create(this);
        await SafeJsAsync("phonicsDrag.register", _dotNetRef);
        await SyncDocumentDirectionAsync();

        // Trigger non-blocking OTA content check in background
        _ = _otaSync.SyncIfNeededAsync();

        if (CurrentScreen == AppScreen.Splash)
        {
            await Task.Delay(1700);
            var seenOnboarding = Preferences.Default.Get(KeyOnboardingDone, false);
            CurrentScreen = seenOnboarding ? AppScreen.Home : AppScreen.Onboarding;
            Notify();
        }
    }

    public async Task ToggleLanguageAsync()
    {
        _loc.ToggleLanguage();
        await SyncDocumentDirectionAsync();
        Notify();
    }

    private Task SyncDocumentDirectionAsync()
        => SafeJsAsync("phonicsDom.setLocale", _loc.CurrentLang, _loc.Direction);

    public void SetMode(GameMode mode)
    {
        SelectedMode = mode;
        Notify();
    }

    public void SetCategory(string category)
    {
        SelectedCategory = string.IsNullOrWhiteSpace(category) ? "all" : category;
        Notify();
    }

    public IReadOnlyList<PhonicsStage> GetFilteredStages() => SelectedCategory switch
    {
        "short" => Stages.Where(s => s.Id is >= 1 and <= 4).ToList(),
        "blends" => Stages.Where(s => s.Id is 5 or 6).ToList(),
        "sight" => Stages.Where(s => s.Id is 7 or 8).ToList(),
        "magice" => Stages.Where(s => s.Id is 9 or 10).ToList(),
        "advanced" => Stages.Where(s => s.Id is 11 or 12).ToList(),
        _ => Stages
    };

    public void NextOnboardingSlide()
    {
        if (OnboardingSlide < 3)
        {
            OnboardingSlide++;
            Notify();
        }
        else
        {
            CompleteOnboarding();
        }
    }

    public void SetOnboardingSlide(int index)
    {
        OnboardingSlide = Math.Clamp(index, 0, 3);
        Notify();
    }

    public void CompleteOnboarding()
    {
        _profileService.SaveProfile(ProfileNameInput, ProfileAvatarInput, OnboardingGoalMinutes);
        Profile = _profileService.GetProfile();
        Preferences.Default.Set(KeyOnboardingDone, true);
        CurrentScreen = AppScreen.Home;
        Notify();
    }

    public async Task StartStageAsync(PhonicsStage stage)
    {
        if (!stage.IsFree && !_activation.IsActivated)
        {
            OpenActivationModal();
            return;
        }

        IsDailyChallengeMode = false;
        ActiveStage = stage;
        WordIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        CurrentScreen = AppScreen.Playing;
        await SetupCurrentWordAsync();
    }

    public async Task StartDailyChallengeAsync()
    {
        var eligibleWords = Stages
            .Where(s => s.IsFree || _activation.IsActivated)
            .SelectMany(s => s.Words)
            .ToList();

        var seed = DateTime.Today.DayOfYear + DateTime.Today.Year * 1000;
        var dailyRng = new Random(seed);
        var dailyWords = eligibleWords.OrderBy(_ => dailyRng.Next()).Take(5).ToList();

        IsDailyChallengeMode = true;
        SelectedMode = GameMode.WordBuilder;
        ActiveStage = new PhonicsStage(
            Id: 99,
            TitleAr: "التحدي اليومي (x2)",
            TitleEn: "Daily Challenge (2x)",
            SubtitleAr: "5 كلمات يومية",
            SubtitleEn: "5 Daily Words",
            BadgeKey: "star_badge",
            GradientStart: "#F59E0B",
            GradientEnd: "#D97706",
            IsFree: true,
            Words: dailyWords);
        WordIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        CurrentScreen = AppScreen.Playing;
        await SetupCurrentWordAsync();
    }

    public void ExitStage()
    {
        ActiveStage = null;
        ActiveStory = null;
        StageFinished = false;
        CompletedWords = _progress.GetCompletedWords();
        CurrentScreen = AppScreen.Home;
        Notify();
    }

    public async Task StartStoryAsync(PhonicsStory story)
    {
        if (!story.IsFree && !_activation.IsActivated)
        {
            OpenActivationModal();
            return;
        }

        ActiveStory = story;
        StorySceneIndex = 0;
        InStoryQuiz = false;
        StoryQuizSolved = false;
        StoryQuizWrongOption = null;
        CurrentScreen = AppScreen.StoryPlayer;
        Notify();
        await SpeakCurrentStorySceneAsync();
    }

    public Task SpeakCurrentStorySceneAsync()
    {
        if (ActiveStory is null || InStoryQuiz) return Task.CompletedTask;
        var scene = ActiveStory.Scenes[StorySceneIndex];
        return _speech.SpeakAsync(scene.SentenceEn, pitch: 1.08f);
    }

    public async Task NextStorySceneAsync()
    {
        if (ActiveStory is null) return;
        if (StorySceneIndex + 1 < ActiveStory.Scenes.Count)
        {
            StorySceneIndex++;
            Notify();
            await SpeakCurrentStorySceneAsync();
        }
        else
        {
            InStoryQuiz = true;
            Notify();
            await _speech.SpeakAsync(ActiveStory.QuizQuestionEn, pitch: 1.08f);
        }
    }

    public async Task PrevStorySceneAsync()
    {
        if (ActiveStory is null) return;
        if (InStoryQuiz)
        {
            InStoryQuiz = false;
            Notify();
            return;
        }
        if (StorySceneIndex > 0)
        {
            StorySceneIndex--;
            Notify();
            await SpeakCurrentStorySceneAsync();
        }
    }

    public async Task AnswerStoryQuizAsync(string option)
    {
        if (ActiveStory is null || StoryQuizSolved) return;

        if (string.Equals(option, ActiveStory.QuizCorrectWord, StringComparison.OrdinalIgnoreCase))
        {
            StoryQuizSolved = true;
            StoryQuizWrongOption = null;
            _progress.RecordStoryCompleted(ActiveStory.Id, 3);
            CompletedWords = _progress.GetCompletedWords();
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
            await _speech.SpeakAsync($"Great job! {ActiveStory.QuizCorrectWord}", pitch: 1.15f);
            Notify();
        }
        else
        {
            StoryQuizWrongOption = option;
            await SafeJsAsync("phonicsAudio.wrong");
            Notify();
            await Task.Delay(450);
            StoryQuizWrongOption = null;
            Notify();
        }
    }

    public void OpenShop()
    {
        ShopFeedback = string.Empty;
        CurrentScreen = AppScreen.Shop;
        Notify();
    }

    public bool IsShopItemUnlocked(string itemId) => _progress.IsShopItemUnlocked(itemId);

    public bool IsShopItemEquipped(ShopItem item) => item.Kind switch
    {
        "avatar" => string.Equals(Profile.AvatarKey, item.Key, StringComparison.OrdinalIgnoreCase),
        "theme" => string.Equals(EquippedTheme, item.Key, StringComparison.OrdinalIgnoreCase),
        _ => false
    };

    public async Task BuyOrEquipShopItemAsync(ShopItem item)
    {
        ShopFeedback = string.Empty;
        if (!IsShopItemUnlocked(item.Id))
        {
            if (!_progress.TryUnlockShopItem(item.Id, item.CostStars))
            {
                ShopFeedback = _loc.T("ShopNotEnough");
                await SafeJsAsync("phonicsAudio.wrong");
                Notify();
                return;
            }
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
        }
        else
        {
            await SafeJsAsync("phonicsAudio.pop");
        }

        if (item.Kind == "avatar")
        {
            _profileService.SaveProfile(Profile.Name, item.Key, Profile.DailyGoalMinutes);
            Profile = _profileService.GetProfile();
        }
        else if (item.Kind == "theme")
        {
            _progress.SetEquippedTheme(item.Key);
        }

        Notify();
    }

    public IReadOnlyList<(string Key, string LabelAr, string LabelEn)> GetAvailableAvatars()
    {
        var list = AvatarArtwork.Avatars.ToList();
        foreach (var sa in AvatarArtwork.ShopAvatars)
        {
            var shopItem = ShopItems.FirstOrDefault(i => i.Kind == "avatar" && i.Key == sa.Key);
            if (shopItem is not null && IsShopItemUnlocked(shopItem.Id))
                list.Add(sa);
        }
        return list;
    }

    public async Task StartWeakWordsReviewAsync()
    {
        var weakKeys = WeakWords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (weakKeys.Count == 0) return;

        var reviewWords = Stages
            .SelectMany(s => s.Words)
            .Where(w => weakKeys.Contains(w.Word))
            .DistinctBy(w => w.Word, StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();

        if (reviewWords.Count == 0) return;

        IsDailyChallengeMode = false;
        SelectedMode = GameMode.WordBuilder;
        ActiveStage = new PhonicsStage(
            Id: 96,
            TitleAr: "مراجعة الكلمات الصعبة",
            TitleEn: "Weak Words Review",
            SubtitleAr: "",
            SubtitleEn: "",
            BadgeKey: "star_badge",
            GradientStart: "#E11D48",
            GradientEnd: "#BE123C",
            IsFree: true,
            Words: reviewWords);
        WordIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        CurrentScreen = AppScreen.Playing;
        await SetupCurrentWordAsync();
    }

    public void OpenSoundboard()
    {
        ActiveSoundboardLetter = null;
        CurrentScreen = AppScreen.Soundboard;
        Notify();
    }

    public async Task SpeakAlphabetLetterAsync(PhonicsLetterItem item)
    {
        ActiveSoundboardLetter = item.Letter;
        Notify();
        await SafeJsAsync("phonicsAudio.pop");
        await SafeJsAsync("phonicsTracing.init", "letterTraceCanvas");
        await _speech.SpeakAsync($"{item.Letter}, {item.ExampleWord}", pitch: 1.1f);
    }

    public Task ClearTracingCanvasAsync() => SafeJsAsync("phonicsTracing.clear");
    public Task SetTracingColorAsync(string color) => SafeJsAsync("phonicsTracing.setColor", color);

    public async Task StartSentenceChallengeAsync()
    {
        SelectedMode = GameMode.SentenceBuilder;
        SentenceList = _curriculum.GetSentences()
            .Where(s => s.IsFree || _activation.IsActivated)
            .ToList();
        SentenceIndex = 0;
        StageStars = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(97, "تركيب الجمل", "Sentence Builder", "", "", "star_badge", "#0284C7", "#2563EB", true, []);
        CurrentScreen = AppScreen.Playing;
        await SetupCurrentSentenceAsync();
    }

    private async Task SetupCurrentSentenceAsync()
    {
        WordSolved = false;
        SentenceWrongFlash = false;
        if (CurrentSentence is null) return;

        SentenceSlots = Enumerable.Repeat(-1, CurrentSentence.Words.Count).ToArray();
        SentenceBank = CurrentSentence.Words
            .OrderBy(_ => _rng.Next())
            .Select(w => (w, false))
            .ToList();

        Notify();
        await SpeakCurrentSentenceAsync();
    }

    public Task SpeakCurrentSentenceAsync()
        => CurrentSentence is null ? Task.CompletedTask : _speech.SpeakAsync(string.Join(' ', CurrentSentence.Words), pitch: 1.05f);

    public async Task SelectSentenceWordAsync(int bankIdx)
    {
        if (WordSolved || bankIdx < 0 || bankIdx >= SentenceBank.Count || SentenceBank[bankIdx].Used)
            return;

        var emptySlot = Array.FindIndex(SentenceSlots, s => s < 0);
        if (emptySlot < 0) return;

        SentenceSlots[emptySlot] = bankIdx;
        SentenceBank[bankIdx] = (SentenceBank[bankIdx].Word, true);
        await SafeJsAsync("phonicsAudio.pop");
        _ = _speech.SpeakAsync(SentenceBank[bankIdx].Word, pitch: 1.1f);

        if (SentenceSlots.All(s => s >= 0) && CurrentSentence is not null)
        {
            var built = string.Join(' ', SentenceSlots.Select(i => SentenceBank[i].Word));
            var target = string.Join(' ', CurrentSentence.Words);
            if (string.Equals(built, target, StringComparison.OrdinalIgnoreCase))
            {
                WordSolved = true;
                Streak++;
                StageStars += 2;
                _progress.RecordWordCompleted(CurrentSentence.Words[0], 2, Streak);
                CompletedWords = _progress.GetCompletedWords();
                await SafeJsAsync("phonicsAudio.success");
                await SafeJsAsync("phonicsAudio.confetti");
                await _speech.SpeakAsync(target, pitch: 1.1f);
            }
            else
            {
                SentenceWrongFlash = true;
                Streak = 0;
                await SafeJsAsync("phonicsAudio.wrong");
                Notify();
                await Task.Delay(520);
                for (var i = 0; i < SentenceSlots.Length; i++)
                {
                    if (SentenceSlots[i] >= 0)
                        SentenceBank[SentenceSlots[i]] = (SentenceBank[SentenceSlots[i]].Word, false);
                    SentenceSlots[i] = -1;
                }
                SentenceWrongFlash = false;
            }
        }
        Notify();
    }

    public async Task ClearSentenceSlotAsync(int slotIdx)
    {
        if (WordSolved || slotIdx < 0 || slotIdx >= SentenceSlots.Length || SentenceSlots[slotIdx] < 0)
            return;
        var bIdx = SentenceSlots[slotIdx];
        SentenceBank[bIdx] = (SentenceBank[bIdx].Word, false);
        SentenceSlots[slotIdx] = -1;
        await SafeJsAsync("phonicsAudio.pop");
        Notify();
    }

    public async Task NextSentenceAsync()
    {
        if (SentenceIndex + 1 < SentenceList.Count)
        {
            SentenceIndex++;
            await SetupCurrentSentenceAsync();
        }
        else
        {
            StageFinished = true;
            await SafeJsAsync("phonicsAudio.confetti");
            Notify();
        }
    }

    public async Task StartRhymingChallengeAsync()
    {
        SelectedMode = GameMode.RhymingMatch;
        RhymeList = _curriculum.GetRhymes()
            .Where(r => r.IsFree || _activation.IsActivated)
            .ToList();
        RhymeIndex = 0;
        StageStars = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(98, "لعبة القافية", "Rhyming Pairs", "", "", "star_badge", "#7C3AED", "#EC4899", true, []);
        CurrentScreen = AppScreen.Playing;
        await SetupCurrentRhymeAsync();
    }

    private async Task SetupCurrentRhymeAsync()
    {
        WordSolved = false;
        RhymeWrongOption = null;
        if (CurrentRhyme is null) return;

        RhymeOptions = CurrentRhyme.Distractors
            .Append(CurrentRhyme.CorrectRhyme)
            .OrderBy(_ => _rng.Next())
            .ToList();

        Notify();
        await SpeakCurrentRhymeTargetAsync();
    }

    public Task SpeakCurrentRhymeTargetAsync()
        => CurrentRhyme is null ? Task.CompletedTask : _speech.SpeakAsync(CurrentRhyme.TargetWord, pitch: 1.05f);

    public async Task SelectRhymeOptionAsync(string option)
    {
        if (WordSolved || CurrentRhyme is null) return;
        if (string.Equals(option, CurrentRhyme.CorrectRhyme, StringComparison.OrdinalIgnoreCase))
        {
            WordSolved = true;
            Streak++;
            StageStars++;
            _progress.RecordWordCompleted(CurrentRhyme.CorrectRhyme, 1, Streak);
            CompletedWords = _progress.GetCompletedWords();
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
            await _speech.SpeakAsync($"{CurrentRhyme.TargetWord}, {CurrentRhyme.CorrectRhyme}", pitch: 1.12f);
            Notify();
        }
        else
        {
            RhymeWrongOption = option;
            Streak = 0;
            _progress.RecordWordMistake(CurrentRhyme.TargetWord);
            await SafeJsAsync("phonicsAudio.wrong");
            Notify();
            await Task.Delay(450);
            RhymeWrongOption = null;
            Notify();
        }
    }

    public async Task NextRhymeAsync()
    {
        if (RhymeIndex + 1 < RhymeList.Count)
        {
            RhymeIndex++;
            await SetupCurrentRhymeAsync();
        }
        else
        {
            StageFinished = true;
            await SafeJsAsync("phonicsAudio.confetti");
            Notify();
        }
    }

    public async Task StartMinimalPairsChallengeAsync()
    {
        SelectedMode = GameMode.MinimalPairs;
        MinimalPairList = _curriculum.GetMinimalPairs()
            .Where(p => p.IsFree || _activation.IsActivated)
            .ToList();
        MinimalPairIndex = 0;
        StageStars = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(95, "الفرق الصوتي", "Minimal Pairs", "", "", "star_badge", "#0D9488", "#0284C7", true, []);
        CurrentScreen = AppScreen.Playing;
        await SetupCurrentMinimalPairAsync();
    }

    private async Task SetupCurrentMinimalPairAsync()
    {
        WordSolved = false;
        MinimalWrongWord = null;
        if (CurrentMinimalPair is null) return;

        MinimalTargetIsLong = _rng.Next(2) == 1;
        Notify();
        await SpeakCurrentMinimalPairAsync();
    }

    public Task SpeakCurrentMinimalPairAsync()
    {
        if (CurrentMinimalPair is null) return Task.CompletedTask;
        var word = MinimalTargetIsLong ? CurrentMinimalPair.LongWord : CurrentMinimalPair.ShortWord;
        return _speech.SpeakAsync(word, pitch: 1.05f);
    }

    public async Task SelectMinimalOptionAsync(bool pickedLong)
    {
        if (WordSolved || CurrentMinimalPair is null) return;
        var targetWord = MinimalTargetIsLong ? CurrentMinimalPair.LongWord : CurrentMinimalPair.ShortWord;

        if (pickedLong == MinimalTargetIsLong)
        {
            WordSolved = true;
            Streak++;
            StageStars++;
            _progress.RecordWordCompleted(targetWord, 1, Streak);
            CompletedWords = _progress.GetCompletedWords();
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
            await _speech.SpeakAsync($"Great! {targetWord}", pitch: 1.12f);
            Notify();
        }
        else
        {
            MinimalWrongWord = pickedLong ? CurrentMinimalPair.LongWord : CurrentMinimalPair.ShortWord;
            Streak = 0;
            _progress.RecordWordMistake(targetWord);
            await SafeJsAsync("phonicsAudio.wrong");
            Notify();
            await Task.Delay(450);
            MinimalWrongWord = null;
            Notify();
        }
    }

    public async Task NextMinimalPairAsync()
    {
        if (MinimalPairIndex + 1 < MinimalPairList.Count)
        {
            MinimalPairIndex++;
            await SetupCurrentMinimalPairAsync();
        }
        else
        {
            StageFinished = true;
            await SafeJsAsync("phonicsAudio.confetti");
            Notify();
        }
    }

    public IReadOnlyList<AchievementItem> GetAchievements() =>
    [
        new("first_word", "أول كلمة", "First Word", CompletedWords.Count >= 1),
        new("ten_stars", "10 نجوم", "10 Stars", TotalStars >= 10),
        new("streak_five", "سلسلة 5", "5 Streak", BestStreak >= 5),
        new("twenty_words", "20 كلمة", "20 Words", CompletedWords.Count >= 20)
    ];

    private async Task SetupCurrentWordAsync()
    {
        WordSolved = false;
        BuilderWrongFlash = false;
        WrongMatchWord = null;
        SpeechFeedback = string.Empty;
        SpeechSuccess = false;

        var word = CurrentWord.Word.ToLowerInvariant();

        if (SelectedMode == GameMode.WordBuilder)
        {
            Slots = Enumerable.Repeat(-1, word.Length).ToArray();
            var letters = word.ToCharArray().ToList();

            const string distractors = "bcdfghlmnprst";
            while (letters.Count < word.Length + 2)
            {
                var candidate = distractors[_rng.Next(distractors.Length)];
                if (!letters.Contains(candidate))
                    letters.Add(candidate);
            }

            Tiles = letters
                .OrderBy(_ => _rng.Next())
                .Select(c => new LetterTileItem { Letter = char.ToUpperInvariant(c), Used = false })
                .ToList();
        }
        else if (SelectedMode == GameMode.SoundMatch)
        {
            var pool = Stages
                .SelectMany(s => s.Words)
                .Where(w => !string.Equals(w.Word, CurrentWord.Word, StringComparison.OrdinalIgnoreCase))
                .OrderBy(_ => _rng.Next())
                .Take(3)
                .ToList();

            pool.Add(CurrentWord);
            MatchOptions = pool.OrderBy(_ => _rng.Next()).ToList();
        }
        else
        {
            SetupMemoryDeck();
            Notify();
            return;
        }

        Notify();
        await SpeakCurrentWordAsync();
    }

    private void SetupMemoryDeck()
    {
        _firstFlippedCard = null;
        _memoryBusy = false;

        var chosen = ActiveStage!.Words
            .OrderBy(_ => _rng.Next())
            .Take(4)
            .ToList();

        var raw = new List<MemoryCardItem>(8);
        foreach (var w in chosen)
        {
            raw.Add(new MemoryCardItem
            {
                WordKey = w.Word,
                IsPictureCard = true,
                ArtworkKey = w.ArtworkKey,
                AccentColor = w.AccentColor,
                BgColor = w.BgColor
            });
            raw.Add(new MemoryCardItem
            {
                WordKey = w.Word,
                IsPictureCard = false,
                ArtworkKey = w.ArtworkKey,
                AccentColor = w.AccentColor,
                BgColor = w.BgColor
            });
        }

        MemoryCards = raw
            .OrderBy(_ => _rng.Next())
            .Select((c, i) => new MemoryCardItem
            {
                Index = i,
                WordKey = c.WordKey,
                IsPictureCard = c.IsPictureCard,
                ArtworkKey = c.ArtworkKey,
                AccentColor = c.AccentColor,
                BgColor = c.BgColor
            })
            .ToList();
    }

    public async Task FlipMemoryCardAsync(MemoryCardItem card)
    {
        if (_memoryBusy || card.IsFlipped || card.IsMatched)
            return;

        card.IsFlipped = true;
        await SafeJsAsync("phonicsAudio.pop");
        _ = _speech.SpeakAsync(card.WordKey, pitch: 1.08f);
        Notify();

        if (_firstFlippedCard is null)
        {
            _firstFlippedCard = card;
            return;
        }

        _memoryBusy = true;
        var first = _firstFlippedCard;
        _firstFlippedCard = null;

        if (string.Equals(first.WordKey, card.WordKey, StringComparison.OrdinalIgnoreCase))
        {
            first.IsMatched = true;
            card.IsMatched = true;
            Streak++;
            StageStars++;
            _progress.RecordWordCompleted(card.WordKey, 1, Streak);
            CompletedWords = _progress.GetCompletedWords();

            await SafeJsAsync("phonicsAudio.success");
            _memoryBusy = false;
            Notify();

            if (MemoryCards.All(c => c.IsMatched))
            {
                await SafeJsAsync("phonicsAudio.confetti");
                await Task.Delay(500);
                StageFinished = true;
                Notify();
            }
        }
        else
        {
            Streak = 0;
            await SafeJsAsync("phonicsAudio.wrong");
            await Task.Delay(650);
            first.IsFlipped = false;
            card.IsFlipped = false;
            _memoryBusy = false;
            Notify();
        }
    }

    public async Task UseHintAsync()
    {
        if (WordSolved || SelectedMode != GameMode.WordBuilder)
            return;

        var target = CurrentWord.Word.ToUpperInvariant();
        for (var slotIdx = 0; slotIdx < target.Length; slotIdx++)
        {
            var currentTileIdx = Slots[slotIdx];
            if (currentTileIdx >= 0 && Tiles[currentTileIdx].Letter == target[slotIdx])
                continue;

            var neededTileIdx = Tiles.FindIndex(t => !t.Used && t.Letter == target[slotIdx]);
            if (neededTileIdx >= 0)
            {
                await PlaceTileInSlotAsync(neededTileIdx, slotIdx);
                return;
            }
        }
    }

    public Task StartTileDragAsync(int tileIdx, double clientX, double clientY)
    {
        if (WordSolved || tileIdx < 0 || tileIdx >= Tiles.Count || Tiles[tileIdx].Used)
            return Task.CompletedTask;

        var letter = Tiles[tileIdx].Letter;
        return SafeJsAsync("phonicsDrag.startDrag", tileIdx, letter.ToString(), IsVowel(letter), clientX, clientY);
    }

    [JSInvokable]
    public async Task OnTileTapped(int tileIdx)
    {
        if (WordSolved || tileIdx < 0 || tileIdx >= Tiles.Count || Tiles[tileIdx].Used)
            return;

        var emptySlot = Array.FindIndex(Slots, s => s < 0);
        if (emptySlot >= 0)
            await PlaceTileInSlotAsync(tileIdx, emptySlot);
    }

    [JSInvokable]
    public async Task OnTileDroppedOnSlot(int tileIdx, int slotIdx)
    {
        if (WordSolved || tileIdx < 0 || tileIdx >= Tiles.Count || Tiles[tileIdx].Used)
            return;

        if (slotIdx < 0 || slotIdx >= Slots.Length)
            return;

        await PlaceTileInSlotAsync(tileIdx, slotIdx);
    }

    private async Task PlaceTileInSlotAsync(int tileIdx, int slotIdx)
    {
        if (Slots[slotIdx] >= 0)
            Tiles[Slots[slotIdx]].Used = false;

        Slots[slotIdx] = tileIdx;
        Tiles[tileIdx].Used = true;

        await SafeJsAsync("phonicsAudio.pop");
        _ = _speech.SpeakLetterAsync(Tiles[tileIdx].Letter);

        if (Slots.All(s => s >= 0))
        {
            var built = new string(Slots.Select(idx => char.ToLowerInvariant(Tiles[idx].Letter)).ToArray());
            if (string.Equals(built, CurrentWord.Word, StringComparison.OrdinalIgnoreCase))
            {
                await MarkWordSolvedAsync();
            }
            else
            {
                BuilderWrongFlash = true;
                Streak = 0;
                _progress.RecordWordMistake(CurrentWord.Word);
                await SafeJsAsync("phonicsAudio.wrong");
                Notify();
                await Task.Delay(520);

                for (var i = 0; i < Slots.Length; i++)
                {
                    if (Slots[i] >= 0)
                        Tiles[Slots[i]].Used = false;
                    Slots[i] = -1;
                }
                BuilderWrongFlash = false;
            }
        }

        Notify();
    }

    public async Task ClearSlotAsync(int slotIdx)
    {
        if (WordSolved || slotIdx < 0 || slotIdx >= Slots.Length || Slots[slotIdx] < 0)
            return;

        Tiles[Slots[slotIdx]].Used = false;
        Slots[slotIdx] = -1;
        await SafeJsAsync("phonicsAudio.pop");
        Notify();
    }

    public async Task SelectMatchOptionAsync(PhonicsWord option)
    {
        if (WordSolved) return;

        if (string.Equals(option.Word, CurrentWord.Word, StringComparison.OrdinalIgnoreCase))
        {
            await MarkWordSolvedAsync();
            Notify();
        }
        else
        {
            WrongMatchWord = option.Word;
            Streak = 0;
            _progress.RecordWordMistake(CurrentWord.Word);
            await SafeJsAsync("phonicsAudio.wrong");
            Notify();
            await Task.Delay(450);
            WrongMatchWord = null;
            Notify();
        }
    }

    private async Task MarkWordSolvedAsync()
    {
        var starsToAward = IsDailyChallengeMode ? 2 : 1;
        WordSolved = true;
        Streak++;
        StageStars += starsToAward;
        _progress.RecordWordCompleted(CurrentWord.Word, starsToAward, Streak);
        CompletedWords = _progress.GetCompletedWords();

        await SafeJsAsync("phonicsAudio.success");
        await SafeJsAsync("phonicsAudio.confetti");
        await _speech.SpeakAsync($"Great job! {CurrentWord.Word}", pitch: 1.15f);
    }

    public async Task NextWordAsync()
    {
        if (ActiveStage is null) return;

        if (WordIndex + 1 < ActiveStage.Words.Count)
        {
            WordIndex++;
            await SetupCurrentWordAsync();
        }
        else
        {
            if (IsDailyChallengeMode)
                _progress.MarkDailyChallengeCompleted();

            StageFinished = true;
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
            Notify();
        }
    }

    public async Task CheckPronunciationAsync()
    {
        if (IsListeningSpeech || ActiveStage is null) return;
        IsListeningSpeech = true;
        SpeechFeedback = string.Empty;
        Notify();

        var result = await _speechRec.ListenAndCheckWordAsync(
            CurrentWord.Word,
            $"Say: {CurrentWord.Word}");

        IsListeningSpeech = false;
        if (!result.IsSupported)
        {
            SpeechSuccess = false;
            SpeechFeedback = _loc.T("SpeechNotAvailable");
        }
        else if (result.IsMatch)
        {
            SpeechSuccess = true;
            SpeechFeedback = _loc.T("SpeechMatchOk");
            _progress.RecordWordCompleted(CurrentWord.Word, 1, Streak + 1);
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
        }
        else
        {
            SpeechSuccess = false;
            SpeechFeedback = _loc.T("SpeechMatchRetry");
            await SafeJsAsync("phonicsAudio.wrong");
        }
        Notify();
    }

    public void OpenProfileModal()
    {
        ProfileNameInput = Profile.Name;
        ProfileAvatarInput = Profile.AvatarKey;
        ShowProfileModal = true;
        Notify();
    }

    public void CloseProfileModal()
    {
        ShowProfileModal = false;
        Notify();
    }

    public void SaveProfile()
    {
        _profileService.SaveProfile(ProfileNameInput, ProfileAvatarInput);
        Profile = _profileService.GetProfile();
        ShowProfileModal = false;
        Notify();
    }

    public async Task ShareCertificateAsync()
    {
        var heroName = string.IsNullOrWhiteSpace(Profile.Name)
            ? _loc.T("DefaultHeroName")
            : Profile.Name;
        var stageTitle = ActiveStage is null
            ? _loc.T("AppTitle")
            : (_loc.IsArabic ? ActiveStage.TitleAr : ActiveStage.TitleEn);

        var text = _loc.IsArabic
            ? $"{_loc.T("AcademyName")} - {_loc.T("CertTitle")}: أتم البطل {heroName} {stageTitle} وجمع {StageStars} نجمة!"
            : $"{_loc.T("AcademyName")} - {_loc.T("CertTitle")}: {heroName} completed {stageTitle} and earned {StageStars} stars!";

        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = _loc.T("CertTitle"),
                Text = text
            });
        }
        catch
        {
            // Ignore share cancel
        }
    }

    public Task SpeakCurrentWordAsync() => _speech.SpeakAsync(CurrentWord.Word, pitch: 1.05f);

    public Task SpeakCurrentWordSlowAsync()
    {
        var letters = string.Join(", ", CurrentWord.Word.ToCharArray());
        return _speech.SpeakAsync($"{letters}. {CurrentWord.Word}", pitch: 1.0f);
    }

    public static bool IsVowel(char c) => "AEIOUaeiou".IndexOf(c) >= 0;

    public void OpenActivationModal()
    {
        ActivationFeedback = string.Empty;
        ActivationSuccess = false;
        ShowActivationModal = true;
        Notify();
    }

    public void CloseActivationModal()
    {
        ShowActivationModal = false;
        Notify();
    }

    public async Task SubmitActivationAsync()
    {
        IsActivating = true;
        ActivationFeedback = string.Empty;
        Notify();

        var result = await _activation.ActivateAsync(ActivationCodeInput);
        ActivationSuccess = result.Success;
        ActivationFeedback = _loc.IsArabic ? result.MessageAr : result.MessageEn;
        IsActivating = false;
        Notify();

        if (result.Success)
        {
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
            await Task.Delay(900);
            ShowActivationModal = false;
            Notify();
        }
    }

    private async Task SafeJsAsync(string identifier, params object?[] args)
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

    private void Notify() => StateChanged?.Invoke();

    public void Dispose()
    {
        _loc.Changed -= Notify;
        _curriculum.ContentReloaded -= Notify;
        _dotNetRef?.Dispose();
    }
}
