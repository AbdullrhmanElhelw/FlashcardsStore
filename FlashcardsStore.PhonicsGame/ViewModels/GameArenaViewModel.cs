using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;
using Microsoft.JSInterop;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages active gameplay across all 6 modes, drag-and-drop, speech check, and stage certificates.</summary>
public sealed partial class GameArenaViewModel : BaseViewModel, IDisposable
{
    private readonly Random _rng = new();
    private readonly ICurriculumRepository _curriculum;
    private readonly IGameProgressService _progress;
    private readonly IActivationService _activation;
    private readonly ISpeechService _speech;
    private readonly ISpeechRecognitionService _speechRec;
    private readonly ILocalizationService _loc;
    private readonly IJsBridgeService _js;
    private readonly ModalsViewModel _modals;
    private readonly AppShellViewModel _shell;
    private DotNetObjectReference<GameArenaViewModel>? _dotNetRef;

    [ObservableProperty] private GameMode _selectedMode = GameMode.WordBuilder;
    [ObservableProperty] private PhonicsStage? _activeStage;
    [ObservableProperty] private int _wordIndex;
    [ObservableProperty] private int _stageStars;
    [ObservableProperty] private int _streak;
    [ObservableProperty] private bool _stageFinished;
    [ObservableProperty] private bool _wordSolved;
    [ObservableProperty] private bool _isDailyChallenge;

    // Word Builder
    [ObservableProperty] private int[] _slots = [];
    [ObservableProperty] private List<LetterTileItem> _tiles = [];
    [ObservableProperty] private bool _builderWrongFlash;

    // Sound Match
    [ObservableProperty] private List<PhonicsWord> _matchOptions = [];
    [ObservableProperty] private string? _wrongMatchWord;

    // Memory Match
    [ObservableProperty] private List<MemoryCardItem> _memoryCards = [];
    private MemoryCardItem? _firstFlippedCard;
    private bool _memoryBusy;

    // Sentence Builder
    [ObservableProperty] private IReadOnlyList<SentenceChallenge> _sentenceList = [];
    [ObservableProperty] private int _sentenceIndex;
    [ObservableProperty] private int[] _sentenceSlots = [];
    [ObservableProperty] private List<(string Word, bool Used)> _sentenceBank = [];
    [ObservableProperty] private bool _sentenceWrongFlash;

    // Rhyming Match
    [ObservableProperty] private IReadOnlyList<RhymeChallenge> _rhymeList = [];
    [ObservableProperty] private int _rhymeIndex;
    [ObservableProperty] private List<string> _rhymeOptions = [];
    [ObservableProperty] private string? _rhymeWrongOption;

    // Minimal Pairs
    [ObservableProperty] private IReadOnlyList<MinimalPairChallenge> _minimalPairList = [];
    [ObservableProperty] private int _minimalPairIndex;
    [ObservableProperty] private bool _minimalTargetIsLong;
    [ObservableProperty] private string? _minimalWrongWord;

    // Grammar Quiz
    [ObservableProperty] private IReadOnlyList<GrammarChallenge> _grammarList = [];
    [ObservableProperty] private int _grammarIndex;
    [ObservableProperty] private string? _grammarWrongOption;

    // Listen & Complete Sentence
    [ObservableProperty] private string _listenMissingWord = string.Empty;
    [ObservableProperty] private List<string> _listenOptions = [];
    [ObservableProperty] private string? _listenWrongOption;

    // Speech Check
    [ObservableProperty] private string _speechFeedback = string.Empty;
    [ObservableProperty] private bool _speechSuccess;
    [ObservableProperty] private bool _isListeningSpeech;

    public GameArenaViewModel(
        ICurriculumRepository curriculum,
        IGameProgressService progress,
        IActivationService activation,
        ISpeechService speech,
        ISpeechRecognitionService speechRec,
        ILocalizationService loc,
        IJsBridgeService js,
        ModalsViewModel modals,
        AppShellViewModel shell)
    {
        _curriculum = curriculum;
        _progress = progress;
        _activation = activation;
        _speech = speech;
        _speechRec = speechRec;
        _loc = loc;
        _js = js;
        _modals = modals;
        _shell = shell;
    }

    public PhonicsWord CurrentWord => ActiveStage!.Words[WordIndex];
    public SentenceChallenge? CurrentSentence => SentenceIndex < SentenceList.Count ? SentenceList[SentenceIndex] : null;
    public RhymeChallenge? CurrentRhyme => RhymeIndex < RhymeList.Count ? RhymeList[RhymeIndex] : null;
    public MinimalPairChallenge? CurrentMinimalPair => MinimalPairIndex < MinimalPairList.Count ? MinimalPairList[MinimalPairIndex] : null;
    public GrammarChallenge? CurrentGrammar => GrammarIndex < GrammarList.Count ? GrammarList[GrammarIndex] : null;
    public string EquippedTheme => _progress.EquippedTheme;

    public Task RegisterDragBridgeAsync()
    {
        _dotNetRef ??= DotNetObjectReference.Create(this);
        return _js.InvokeAsync("phonicsDrag.register", _dotNetRef);
    }

    public static bool IsVowel(char c) => "AEIOUaeiou".IndexOf(c) >= 0;

    public async Task StartStageAsync(PhonicsStage stage)
    {
        if (!stage.IsFree && !_activation.IsActivated)
        {
            _modals.OpenActivationModal();
            return;
        }

        IsDailyChallenge = false;
        ActiveStage = stage;
        WordIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentWordAsync();
    }

    public async Task StartDailyChallengeAsync()
    {
        var eligible = _curriculum.GetStages()
            .Where(s => s.IsFree || _activation.IsActivated)
            .SelectMany(s => s.Words)
            .ToList();

        var seed = DateTime.Today.DayOfYear + DateTime.Today.Year * 1000;
        var dailyRng = new Random(seed);
        var dailyWords = eligible.OrderBy(_ => dailyRng.Next()).Take(5).ToList();

        IsDailyChallenge = true;
        SelectedMode = GameMode.WordBuilder;
        ActiveStage = new PhonicsStage(99, "التحدي اليومي (x2)", "Daily Challenge (2x)", "", "", "star_badge", "#F59E0B", "#D97706", true, dailyWords);
        WordIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentWordAsync();
    }

    public async Task StartWeakWordsReviewAsync()
    {
        var weakKeys = _progress.GetWeakWords().ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (weakKeys.Count == 0) return;

        var reviewWords = _curriculum.GetStages()
            .SelectMany(s => s.Words)
            .Where(w => weakKeys.Contains(w.Word))
            .DistinctBy(w => w.Word, StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();

        if (reviewWords.Count == 0) return;

        IsDailyChallenge = false;
        SelectedMode = GameMode.WordBuilder;
        ActiveStage = new PhonicsStage(96, "مراجعة الكلمات الصعبة", "Weak Words Review", "", "", "star_badge", "#E11D48", "#BE123C", true, reviewWords);
        WordIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentWordAsync();
    }

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
            Tiles = letters.OrderBy(_ => _rng.Next())
                .Select(c => new LetterTileItem { Letter = char.ToUpperInvariant(c), Used = false })
                .ToList();
        }
        else if (SelectedMode == GameMode.SoundMatch)
        {
            var pool = _curriculum.GetStages()
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
            return;
        }

        await SpeakCurrentWordAsync();
    }

    public Task StartTileDragAsync(int tileIdx, double clientX, double clientY)
    {
        if (WordSolved || tileIdx < 0 || tileIdx >= Tiles.Count || Tiles[tileIdx].Used)
            return Task.CompletedTask;
        var letter = Tiles[tileIdx].Letter;
        return _js.InvokeAsync("phonicsDrag.startDrag", tileIdx, letter.ToString(), IsVowel(letter), clientX, clientY);
    }

    [JSInvokable]
    public async Task OnTileTapped(int tileIdx)
    {
        if (WordSolved || tileIdx < 0 || tileIdx >= Tiles.Count || Tiles[tileIdx].Used) return;
        var emptySlot = Array.FindIndex(Slots, s => s < 0);
        if (emptySlot >= 0) await PlaceTileInSlotAsync(tileIdx, emptySlot);
    }

    [JSInvokable]
    public async Task OnTileDroppedOnSlot(int tileIdx, int slotIdx)
    {
        if (WordSolved || tileIdx < 0 || tileIdx >= Tiles.Count || Tiles[tileIdx].Used) return;
        if (slotIdx < 0 || slotIdx >= Slots.Length) return;
        await PlaceTileInSlotAsync(tileIdx, slotIdx);
    }

    private async Task PlaceTileInSlotAsync(int tileIdx, int slotIdx)
    {
        if (Slots[slotIdx] >= 0) Tiles[Slots[slotIdx]].Used = false;
        Slots[slotIdx] = tileIdx;
        Tiles[tileIdx].Used = true;

        await _js.InvokeAsync("phonicsAudio.pop");
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
                await _js.InvokeAsync("phonicsAudio.wrong");
                await Task.Delay(520);
                for (var i = 0; i < Slots.Length; i++)
                {
                    if (Slots[i] >= 0) Tiles[Slots[i]].Used = false;
                    Slots[i] = -1;
                }
                BuilderWrongFlash = false;
            }
        }
        NotifyAll();
    }

    public async Task ClearSlotAsync(int slotIdx)
    {
        if (WordSolved || slotIdx < 0 || slotIdx >= Slots.Length || Slots[slotIdx] < 0) return;
        Tiles[Slots[slotIdx]].Used = false;
        Slots[slotIdx] = -1;
        await _js.InvokeAsync("phonicsAudio.pop");
        NotifyAll();
    }

    [RelayCommand]
    public async Task UseHintAsync()
    {
        if (WordSolved || SelectedMode != GameMode.WordBuilder) return;
        var target = CurrentWord.Word.ToUpperInvariant();
        for (var slotIdx = 0; slotIdx < target.Length; slotIdx++)
        {
            var currentTileIdx = Slots[slotIdx];
            if (currentTileIdx >= 0 && Tiles[currentTileIdx].Letter == target[slotIdx]) continue;
            var neededTileIdx = Tiles.FindIndex(t => !t.Used && t.Letter == target[slotIdx]);
            if (neededTileIdx >= 0)
            {
                await PlaceTileInSlotAsync(neededTileIdx, slotIdx);
                return;
            }
        }
    }

    public async Task SelectMatchOptionAsync(PhonicsWord option)
    {
        if (WordSolved) return;
        if (string.Equals(option.Word, CurrentWord.Word, StringComparison.OrdinalIgnoreCase))
        {
            await MarkWordSolvedAsync();
        }
        else
        {
            WrongMatchWord = option.Word;
            Streak = 0;
            _progress.RecordWordMistake(CurrentWord.Word);
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            WrongMatchWord = null;
        }
    }

    private void SetupMemoryDeck()
    {
        _firstFlippedCard = null;
        _memoryBusy = false;
        var chosen = ActiveStage!.Words.OrderBy(_ => _rng.Next()).Take(4).ToList();
        var raw = new List<MemoryCardItem>(8);
        foreach (var w in chosen)
        {
            raw.Add(new MemoryCardItem { WordKey = w.Word, IsPictureCard = true, ArtworkKey = w.ArtworkKey, AccentColor = w.AccentColor, BgColor = w.BgColor });
            raw.Add(new MemoryCardItem { WordKey = w.Word, IsPictureCard = false, ArtworkKey = w.ArtworkKey, AccentColor = w.AccentColor, BgColor = w.BgColor });
        }
        MemoryCards = raw.OrderBy(_ => _rng.Next())
            .Select((c, i) => new MemoryCardItem { Index = i, WordKey = c.WordKey, IsPictureCard = c.IsPictureCard, ArtworkKey = c.ArtworkKey, AccentColor = c.AccentColor, BgColor = c.BgColor })
            .ToList();
    }

    public async Task FlipMemoryCardAsync(MemoryCardItem card)
    {
        if (_memoryBusy || card.IsFlipped || card.IsMatched) return;
        card.IsFlipped = true;
        await _js.InvokeAsync("phonicsAudio.pop");
        _ = _speech.SpeakAsync(card.WordKey, pitch: 1.08f);
        NotifyAll();

        if (_firstFlippedCard is null) { _firstFlippedCard = card; return; }
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
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            _memoryBusy = false;
            NotifyAll();

            if (MemoryCards.All(c => c.IsMatched))
            {
                await _js.InvokeAsync("phonicsAudio.confetti");
                await Task.Delay(500);
                StageFinished = true;
            }
        }
        else
        {
            Streak = 0;
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(650);
            first.IsFlipped = false;
            card.IsFlipped = false;
            _memoryBusy = false;
            NotifyAll();
        }
    }

    private async Task MarkWordSolvedAsync()
    {
        var stars = IsDailyChallenge ? 2 : 1;
        WordSolved = true;
        Streak++;
        StageStars += stars;
        _progress.RecordWordCompleted(CurrentWord.Word, stars, Streak);
        _shell.RefreshHeader();
        await _js.InvokeAsync("phonicsAudio.success");
        await _js.InvokeAsync("phonicsAudio.confetti");
        await _speech.SpeakAsync($"Great job! {CurrentWord.Word}", pitch: 1.15f);
    }

    [RelayCommand]
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
            if (IsDailyChallenge) _progress.MarkDailyChallengeCompleted();
            StageFinished = true;
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
        }
    }

    [RelayCommand]
    public Task SpeakCurrentWordAsync() => _speech.SpeakAsync(CurrentWord.Word, pitch: 1.05f);

    [RelayCommand]
    public Task SpeakCurrentWordSlowAsync()
    {
        var letters = string.Join(", ", CurrentWord.Word.ToCharArray());
        return _speech.SpeakAsync($"{letters}. {CurrentWord.Word}", pitch: 1.0f);
    }

    [RelayCommand]
    public async Task CheckPronunciationAsync()
    {
        if (IsListeningSpeech || ActiveStage is null) return;
        IsListeningSpeech = true;
        SpeechFeedback = string.Empty;

        var result = await _speechRec.ListenAndCheckWordAsync(CurrentWord.Word, $"Say: {CurrentWord.Word}");
        IsListeningSpeech = false;

        if (!result.IsSupported) { SpeechSuccess = false; SpeechFeedback = _loc.T("SpeechNotAvailable"); }
        else if (result.IsMatch)
        {
            SpeechSuccess = true;
            SpeechFeedback = _loc.T("SpeechMatchOk");
            if (SelectedMode == GameMode.SpeakRepeat && !WordSolved)
            {
                WordSolved = true;
                Streak++;
                StageStars++;
            }
            _progress.RecordWordCompleted(CurrentWord.Word, 1, Streak + 1);
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
        }
        else
        {
            SpeechSuccess = false;
            SpeechFeedback = _loc.T("SpeechMatchRetry");
            await _js.InvokeAsync("phonicsAudio.wrong");
        }
    }

    // --- Sentence Builder ---
    public async Task StartSentenceChallengeAsync()
    {
        SelectedMode = GameMode.SentenceBuilder;
        SentenceList = _curriculum.GetSentences().Where(s => s.IsFree || _activation.IsActivated).ToList();
        SentenceIndex = 0;
        StageStars = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(97, "تركيب الجمل", "Sentence Builder", "", "", "star_badge", "#0284C7", "#2563EB", true, []);
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentSentenceAsync();
    }

    private async Task SetupCurrentSentenceAsync()
    {
        WordSolved = false;
        SentenceWrongFlash = false;
        if (CurrentSentence is null) return;
        SentenceSlots = Enumerable.Repeat(-1, CurrentSentence.Words.Count).ToArray();
        SentenceBank = CurrentSentence.Words.OrderBy(_ => _rng.Next()).Select(w => (w, false)).ToList();
        await SpeakCurrentSentenceAsync();
    }

    [RelayCommand]
    public Task SpeakCurrentSentenceAsync()
        => CurrentSentence is null ? Task.CompletedTask : _speech.SpeakAsync(string.Join(' ', CurrentSentence.Words), pitch: 1.05f);

    public async Task SelectSentenceWordAsync(int bankIdx)
    {
        if (WordSolved || bankIdx < 0 || bankIdx >= SentenceBank.Count || SentenceBank[bankIdx].Used) return;
        var emptySlot = Array.FindIndex(SentenceSlots, s => s < 0);
        if (emptySlot < 0) return;

        SentenceSlots[emptySlot] = bankIdx;
        SentenceBank[bankIdx] = (SentenceBank[bankIdx].Word, true);
        await _js.InvokeAsync("phonicsAudio.pop");
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
                _shell.RefreshHeader();
                await _js.InvokeAsync("phonicsAudio.success");
                await _js.InvokeAsync("phonicsAudio.confetti");
                await _speech.SpeakAsync(target, pitch: 1.1f);
            }
            else
            {
                SentenceWrongFlash = true;
                Streak = 0;
                await _js.InvokeAsync("phonicsAudio.wrong");
                await Task.Delay(520);
                for (var i = 0; i < SentenceSlots.Length; i++)
                {
                    if (SentenceSlots[i] >= 0) SentenceBank[SentenceSlots[i]] = (SentenceBank[SentenceSlots[i]].Word, false);
                    SentenceSlots[i] = -1;
                }
                SentenceWrongFlash = false;
            }
        }
        NotifyAll();
    }

    public async Task ClearSentenceSlotAsync(int slotIdx)
    {
        if (WordSolved || slotIdx < 0 || slotIdx >= SentenceSlots.Length || SentenceSlots[slotIdx] < 0) return;
        var bIdx = SentenceSlots[slotIdx];
        SentenceBank[bIdx] = (SentenceBank[bIdx].Word, false);
        SentenceSlots[slotIdx] = -1;
        await _js.InvokeAsync("phonicsAudio.pop");
        NotifyAll();
    }

    [RelayCommand]
    public async Task NextSentenceAsync()
    {
        if (SentenceIndex + 1 < SentenceList.Count) { SentenceIndex++; await SetupCurrentSentenceAsync(); }
        else { StageFinished = true; await _js.InvokeAsync("phonicsAudio.confetti"); }
    }

    // --- Rhyming Match ---
    public async Task StartRhymingChallengeAsync()
    {
        SelectedMode = GameMode.RhymingMatch;
        RhymeList = _curriculum.GetRhymes().Where(r => r.IsFree || _activation.IsActivated).ToList();
        RhymeIndex = 0;
        StageStars = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(98, "لعبة القافية", "Rhyming Pairs", "", "", "star_badge", "#7C3AED", "#EC4899", true, []);
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentRhymeAsync();
    }

    private async Task SetupCurrentRhymeAsync()
    {
        WordSolved = false;
        RhymeWrongOption = null;
        if (CurrentRhyme is null) return;
        RhymeOptions = CurrentRhyme.Distractors.Append(CurrentRhyme.CorrectRhyme).OrderBy(_ => _rng.Next()).ToList();
        await SpeakCurrentRhymeTargetAsync();
    }

    [RelayCommand]
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
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
            await _speech.SpeakAsync($"{CurrentRhyme.TargetWord}, {CurrentRhyme.CorrectRhyme}", pitch: 1.12f);
        }
        else
        {
            RhymeWrongOption = option;
            Streak = 0;
            _progress.RecordWordMistake(CurrentRhyme.TargetWord);
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            RhymeWrongOption = null;
        }
    }

    [RelayCommand]
    public async Task NextRhymeAsync()
    {
        if (RhymeIndex + 1 < RhymeList.Count) { RhymeIndex++; await SetupCurrentRhymeAsync(); }
        else { StageFinished = true; await _js.InvokeAsync("phonicsAudio.confetti"); }
    }

    // --- Minimal Pairs ---
    public async Task StartMinimalPairsChallengeAsync()
    {
        SelectedMode = GameMode.MinimalPairs;
        MinimalPairList = _curriculum.GetMinimalPairs().Where(p => p.IsFree || _activation.IsActivated).ToList();
        MinimalPairIndex = 0;
        StageStars = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(95, "الفرق الصوتي", "Minimal Pairs", "", "", "star_badge", "#0D9488", "#0284C7", true, []);
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentMinimalPairAsync();
    }

    private async Task SetupCurrentMinimalPairAsync()
    {
        WordSolved = false;
        MinimalWrongWord = null;
        if (CurrentMinimalPair is null) return;
        MinimalTargetIsLong = _rng.Next(2) == 1;
        await SpeakCurrentMinimalPairAsync();
    }

    [RelayCommand]
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
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
            await _speech.SpeakAsync($"Great! {targetWord}", pitch: 1.12f);
        }
        else
        {
            MinimalWrongWord = pickedLong ? CurrentMinimalPair.LongWord : CurrentMinimalPair.ShortWord;
            Streak = 0;
            _progress.RecordWordMistake(targetWord);
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            MinimalWrongWord = null;
        }
    }

    [RelayCommand]
    public async Task NextMinimalPairAsync()
    {
        if (MinimalPairIndex + 1 < MinimalPairList.Count) { MinimalPairIndex++; await SetupCurrentMinimalPairAsync(); }
        else { StageFinished = true; await _js.InvokeAsync("phonicsAudio.confetti"); }
    }

    // --- Grammar Quiz ---
    public async Task StartGrammarChallengeAsync(string topic, bool isFree)
    {
        if (!isFree && !_activation.IsActivated)
        {
            _modals.OpenActivationModal();
            return;
        }

        SelectedMode = GameMode.GrammarQuiz;
        GrammarList = _curriculum.GetGrammarChallenges()
            .Where(g => string.Equals(g.Topic, topic, StringComparison.OrdinalIgnoreCase))
            .ToList();
        GrammarIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        var first = GrammarList.FirstOrDefault();
        ActiveStage = new PhonicsStage(94, first?.TitleAr ?? "القواعد", first?.TitleEn ?? "Grammar", "", "", "star_badge", "#7C3AED", "#2563EB", true, []);
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentGrammarAsync();
    }

    private async Task SetupCurrentGrammarAsync()
    {
        WordSolved = false;
        GrammarWrongOption = null;
        if (CurrentGrammar is null) return;
        await SpeakGrammarPromptAsync();
    }

    [RelayCommand]
    public Task SpeakGrammarPromptAsync()
    {
        if (CurrentGrammar is null) return Task.CompletedTask;
        var text = WordSolved
            ? CurrentGrammar.PromptTemplate.Replace("___", CurrentGrammar.CorrectOption)
            : CurrentGrammar.PromptTemplate.Replace("___", "blank");
        return _speech.SpeakAsync(text, pitch: 1.05f);
    }

    public async Task SelectGrammarOptionAsync(string option)
    {
        if (WordSolved || CurrentGrammar is null) return;
        if (string.Equals(option, CurrentGrammar.CorrectOption, StringComparison.OrdinalIgnoreCase))
        {
            WordSolved = true;
            Streak++;
            StageStars++;
            _progress.RecordWordCompleted(CurrentGrammar.CorrectOption, 1, Streak);
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
            var fullSentence = CurrentGrammar.PromptTemplate.Replace("___", CurrentGrammar.CorrectOption);
            await _speech.SpeakAsync(fullSentence, pitch: 1.08f);
        }
        else
        {
            GrammarWrongOption = option;
            Streak = 0;
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            GrammarWrongOption = null;
        }
    }

    [RelayCommand]
    public async Task NextGrammarAsync()
    {
        if (GrammarIndex + 1 < GrammarList.Count) { GrammarIndex++; await SetupCurrentGrammarAsync(); }
        else { StageFinished = true; await _js.InvokeAsync("phonicsAudio.confetti"); }
    }

    // --- Listening & Speaking Modes ---
    public Task StartListenChooseChallengeAsync()
    {
        SelectedMode = GameMode.SoundMatch;
        var pool = _curriculum.GetStages()
            .Where(s => s.IsFree || _activation.IsActivated)
            .SelectMany(s => s.Words)
            .OrderBy(_ => _rng.Next())
            .Take(8)
            .ToList();
        return StartStageAsync(new PhonicsStage(93, "اسمع واختار الصورة", "Listen & Pick Picture", "", "", "star_badge", "#0284C7", "#0EA5E9", true, pool));
    }

    public Task StartSpeakRepeatChallengeAsync()
    {
        SelectedMode = GameMode.SpeakRepeat;
        var pool = _curriculum.GetStages()
            .Where(s => s.IsFree || _activation.IsActivated)
            .SelectMany(s => s.Words)
            .OrderBy(_ => _rng.Next())
            .Take(8)
            .ToList();
        return StartStageAsync(new PhonicsStage(92, "كرّر الكلمة بعد الصوت", "Repeat After Me", "", "", "star_badge", "#10B981", "#059669", true, pool));
    }

    public async Task StartListenCompleteChallengeAsync()
    {
        SelectedMode = GameMode.ListenComplete;
        SentenceList = _curriculum.GetSentences().Where(s => s.IsFree || _activation.IsActivated).ToList();
        SentenceIndex = 0;
        StageStars = 0;
        Streak = 0;
        StageFinished = false;
        ActiveStage = new PhonicsStage(91, "اسمع وكمّل الجملة", "Listen & Complete", "", "", "star_badge", "#F97316", "#EA580C", true, []);
        _shell.NavigateTo(AppScreen.Playing);
        await SetupCurrentListenCompleteAsync();
    }

    private async Task SetupCurrentListenCompleteAsync()
    {
        WordSolved = false;
        ListenWrongOption = null;
        if (CurrentSentence is null) return;
        ListenMissingWord = CurrentSentence.Words[^1];
        var distractors = _curriculum.GetStages()
            .SelectMany(s => s.Words)
            .Select(w => w.Word)
            .Where(w => !string.Equals(w, ListenMissingWord, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(_ => _rng.Next())
            .Take(3)
            .ToList();
        ListenOptions = distractors.Append(ListenMissingWord).OrderBy(_ => _rng.Next()).ToList();
        await SpeakCurrentSentenceAsync();
    }

    public async Task SelectListenCompleteOptionAsync(string option)
    {
        if (WordSolved || CurrentSentence is null) return;
        if (string.Equals(option, ListenMissingWord, StringComparison.OrdinalIgnoreCase))
        {
            WordSolved = true;
            Streak++;
            StageStars++;
            _progress.RecordWordCompleted(ListenMissingWord, 1, Streak);
            _shell.RefreshHeader();
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
        }
        else
        {
            ListenWrongOption = option;
            Streak = 0;
            await _js.InvokeAsync("phonicsAudio.wrong");
            await Task.Delay(450);
            ListenWrongOption = null;
        }
    }

    [RelayCommand]
    public async Task NextListenCompleteAsync()
    {
        if (SentenceIndex + 1 < SentenceList.Count) { SentenceIndex++; await SetupCurrentListenCompleteAsync(); }
        else { StageFinished = true; await _js.InvokeAsync("phonicsAudio.confetti"); }
    }

    [RelayCommand]
    public async Task ShareCertificateAsync()
    {
        var heroName = string.IsNullOrWhiteSpace(_modals.Profile.Name) ? _loc.T("DefaultHeroName") : _modals.Profile.Name;
        var stageTitle = ActiveStage is null ? _loc.T("AppTitle") : (_loc.IsArabic ? ActiveStage.TitleAr : ActiveStage.TitleEn);
        var text = _loc.IsArabic
            ? $"{_loc.T("AcademyName")} - {_loc.T("CertTitle")}: أتم البطل {heroName} {stageTitle} وجمع {StageStars} نجمة!"
            : $"{_loc.T("AcademyName")} - {_loc.T("CertTitle")}: {heroName} completed {stageTitle} and earned {StageStars} stars!";
        try { await Share.Default.RequestAsync(new ShareTextRequest { Title = _loc.T("CertTitle"), Text = text }); }
        catch { }
    }

    public void Dispose() => _dotNetRef?.Dispose();
}
