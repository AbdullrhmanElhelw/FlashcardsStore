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
    private readonly ISpeechService _speech;
    private readonly IActivationService _activation;
    private readonly IGameProgressService _progress;
    private readonly ILocalizationService _loc;
    private IJSRuntime? _js;
    private DotNetObjectReference<PhonicsGameViewModel>? _dotNetRef;

    public event Action? StateChanged;

    public PhonicsGameViewModel(
        ISpeechService speech,
        IActivationService activation,
        IGameProgressService progress,
        ILocalizationService loc)
    {
        _speech = speech;
        _activation = activation;
        _progress = progress;
        _loc = loc;
        _loc.Changed += Notify;
        CompletedWords = _progress.GetCompletedWords();
    }

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

    public void NextOnboardingSlide()
    {
        if (OnboardingSlide < 2)
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
        OnboardingSlide = Math.Clamp(index, 0, 2);
        Notify();
    }

    public void CompleteOnboarding()
    {
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

        ActiveStage = stage;
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
        StageFinished = false;
        CompletedWords = _progress.GetCompletedWords();
        CurrentScreen = AppScreen.Home;
        Notify();
    }

    private async Task SetupCurrentWordAsync()
    {
        WordSolved = false;
        BuilderWrongFlash = false;
        WrongMatchWord = null;

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
        else
        {
            var pool = PhonicsCurriculum.Stages
                .SelectMany(s => s.Words)
                .Where(w => !string.Equals(w.Word, CurrentWord.Word, StringComparison.OrdinalIgnoreCase))
                .OrderBy(_ => _rng.Next())
                .Take(3)
                .ToList();

            pool.Add(CurrentWord);
            MatchOptions = pool.OrderBy(_ => _rng.Next()).ToList();
        }

        Notify();
        await SpeakCurrentWordAsync();
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
            await SafeJsAsync("phonicsAudio.wrong");
            Notify();
            await Task.Delay(450);
            WrongMatchWord = null;
            Notify();
        }
    }

    private async Task MarkWordSolvedAsync()
    {
        WordSolved = true;
        Streak++;
        StageStars++;
        _progress.RecordWordCompleted(CurrentWord.Word, 1, Streak);
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
            StageFinished = true;
            await SafeJsAsync("phonicsAudio.success");
            await SafeJsAsync("phonicsAudio.confetti");
            Notify();
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
        _dotNetRef?.Dispose();
    }
}
