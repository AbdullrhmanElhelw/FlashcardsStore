using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Ui;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class OnboardingViewModel : BaseViewModel
{
    public const string SeenOnboardingKey = "has_seen_onboarding_v1";

    private readonly INavigationService _navigation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLastSlide))]
    [NotifyPropertyChangedFor(nameof(NextButtonText))]
    private int _currentPosition;

    public ObservableCollection<OnboardingSlide> Slides { get; } = new()
    {
        new OnboardingSlide
        {
            Title = "اكتشفي بطاقات الفلاش كاردز",
            Description = "مجموعات تعليمية مصممة بعناية مع ميس خلود النادي لتأسيس طفلك في الفونكس والقواعد والمفردات.",
            ImageName = "onboarding_discover.png"
        },
        new OnboardingSlide
        {
            Title = "اختاري المناسب لعمر طفلك",
            Description = "تصفحي حسب المرحلة الدراسية مع معاينة الكلمات، التقييمات، وتسجيلات المعلمة الصوتية.",
            ImageName = "onboarding_choose.png"
        },
        new OnboardingSlide
        {
            Title = "تعلّم تفاعلي وطلب سهل",
            Description = "استمعي للنطق الصحيح بالسرعة البطيئة للأطفال واطلبي الباقات بسهولة حتى باب البيت.",
            ImageName = "onboarding_practice.png"
        }
    };

    public bool IsLastSlide => CurrentPosition >= Slides.Count - 1;
    public string NextButtonText => IsLastSlide ? "ابدئي التسوق الآن" : "التالي";

    public OnboardingViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        Title = "مرحباً بكِ";
    }

    [RelayCommand]
    private void NextOrFinish()
    {
        if (!IsLastSlide)
        {
            CurrentPosition++;
            return;
        }
        CompleteOnboarding();
    }

    [RelayCommand]
    private void Skip() => CompleteOnboarding();

    private void CompleteOnboarding()
    {
        Preferences.Default.Set(SeenOnboardingKey, true);
        _navigation.SetStoreRoot();
    }
}
