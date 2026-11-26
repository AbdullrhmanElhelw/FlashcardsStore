namespace FlashcardsStore.PhonicsGame.Services;

public interface ILocalizationService
{
    string CurrentLang { get; }
    bool IsArabic { get; }
    string Direction { get; }
    event Action? Changed;
    string T(string key);
    void ToggleLanguage();
    void SetLanguage(string lang);
}

/// <summary>Bilingual AR/EN localization service persisted across app launches.</summary>
public sealed class LocalizationService : ILocalizationService
{
    private const string KeyLang = "phonics_lang";
    private string _currentLang;

    public event Action? Changed;

    public LocalizationService()
    {
        _currentLang = Preferences.Default.Get(KeyLang, "ar");
    }

    public string CurrentLang => _currentLang;
    public bool IsArabic => _currentLang == "ar";
    public string Direction => IsArabic ? "rtl" : "ltr";

    public void ToggleLanguage() => SetLanguage(IsArabic ? "en" : "ar");

    public void SetLanguage(string lang)
    {
        var normalized = lang == "en" ? "en" : "ar";
        if (_currentLang == normalized) return;
        _currentLang = normalized;
        Preferences.Default.Set(KeyLang, normalized);
        Changed?.Invoke();
    }

    public string T(string key)
    {
        if (Strings.TryGetValue(key, out var pair))
            return IsArabic ? pair.Ar : pair.En;
        return key;
    }

    private static readonly Dictionary<string, (string Ar, string En)> Strings = new()
    {
        ["AppTitle"] = ("ألعاب الفونكس", "Phonics Games"),
        ["AcademyName"] = ("أكاديمية خلود النادي", "Kholoud Elnady Academy"),
        ["SplashSubtitle"] = ("تعلم القراءة والنطق باللعب", "Learn Reading & Phonics Through Play"),
        ["SplashLoading"] = ("جاري تجهيز الألعاب...", "Preparing your games..."),

        ["Skip"] = ("تخطي", "Skip"),
        ["Next"] = ("التالي", "Next"),
        ["StartPlaying"] = ("ابدأ اللعب الآن", "Start Playing"),
        ["Onb1Title"] = ("ركّب الكلمات باللمس والسحب", "Build Words with Drag & Drop"),
        ["Onb1Desc"] = ("اسحب الحروف إلى أماكنها الصحيحة وتعلّم نطق كل حرف وصوته بسهولة.", "Drag letters into their slots and hear every letter sound clearly."),
        ["Onb2Title"] = ("طابق الصوت بالصورة", "Match Sound to Picture"),
        ["Onb2Desc"] = ("استمع لنطق الكلمة بالسرعة العادية أو البطيئة واختر الصورة الصحيحة.", "Listen at normal or slow speed and tap the matching picture."),
        ["Onb3Title"] = ("اجمع النجوم وافتح المراحل", "Collect Stars & Unlock Stages"),
        ["Onb3Desc"] = ("المرحلتان الأولى والثانية مجاناً، وافتح باقي المراحل بكود الباقة.", "First 2 stages are free; unlock all stages with your bundle code."),

        ["ActivateCode"] = ("تفعيل", "Activate"),
        ["ActivatedBadge"] = ("مفعّل", "Unlocked"),
        ["HeroGreeting"] = ("أهلاً يا بطل!", "Hello, Champion!"),
        ["HeroSub"] = ("اختر طريقة اللعب وابدأ التحدي", "Pick a game mode and start the challenge"),
        ["WordsMastered"] = ("كلمة", "Words"),
        ["BestStreak"] = ("أعلى سلسلة", "Best Streak"),

        ["ModeBuilderTitle"] = ("تركيب الكلمة", "Word Builder"),
        ["ModeBuilderSub"] = ("اسحب الحروف وركّبها", "Drag & drop letters"),
        ["ModeMatchTitle"] = ("طابق الصوت", "Sound Match"),
        ["ModeMatchSub"] = ("اسمع واختر الصورة", "Listen & pick picture"),

        ["PlayBtn"] = ("العب", "Play"),
        ["LockedBtn"] = ("مقفول", "Locked"),
        ["BackToStages"] = ("المراحل", "Stages"),
        ["ListenWord"] = ("اسمع الكلمة", "Listen"),
        ["SlowSpeak"] = ("نطق بطيء", "Slow"),
        ["MatchPrompt"] = ("استمع للكلمة واختر الصورة الصحيحة", "Listen and choose the right picture"),
        ["GreatJob"] = ("ممتاز!", "Great Job!"),
        ["OneStarEarned"] = ("+1 نجمة ذهبية", "+1 Gold Star"),
        ["NextWord"] = ("التالي", "Next"),
        ["StageCompleteTitle"] = ("أحسنت يا بطل!", "Well Done, Champion!"),
        ["StageCompleteDesc"] = ("أتممت المرحلة بنجاح وجمعت نجوماً جديدة.", "You completed this stage and earned new stars."),
        ["ReturnHome"] = ("العودة للمراحل", "Back to Stages"),

        ["ModalTitle"] = ("تفعيل جميع المراحل", "Unlock All Stages"),
        ["ModalDesc"] = ("أدخل كود التفعيل المرفق مع باقة الفلاش كاردز أو الاشتراك لفتح المراحل من 3 إلى 6.", "Enter the activation code from your flashcard bundle or subscription to unlock stages 3 to 6."),
        ["ModalBtn"] = ("تفعيل الكود الآن", "Activate Code"),
        ["ModalVerifying"] = ("جاري التحقق...", "Verifying..."),
        ["ModalWhatsApp"] = ("اطلب كود تفعيل عبر واتساب", "Get Activation Code on WhatsApp"),
        ["Close"] = ("إغلاق", "Close")
    };
}
