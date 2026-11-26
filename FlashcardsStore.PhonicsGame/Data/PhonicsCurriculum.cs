namespace FlashcardsStore.PhonicsGame.Data;

public sealed record PhonicsWord(
    string Word,
    string ArabicMeaning,
    string Emoji,
    string BgColor,
    string AccentColor,
    bool IsSightWord = false);

public sealed record PhonicsStage(
    int Id,
    string TitleAr,
    string TitleEn,
    string SubtitleAr,
    string BadgeIcon,
    string GradientStart,
    string GradientEnd,
    bool IsFree,
    IReadOnlyList<PhonicsWord> Words);

public static class PhonicsCurriculum
{
    public static IReadOnlyList<PhonicsStage> Stages { get; } =
    [
        new PhonicsStage(
            Id: 1,
            TitleAr: "المرحلة 1: الفتحة القصيرة",
            TitleEn: "Short A (-at / -an / -ap)",
            SubtitleAr: "كلمات ثلاثية سهلة — مجانية",
            BadgeIcon: "🐱",
            GradientStart: "#FF6B6B",
            GradientEnd: "#FF9F43",
            IsFree: true,
            Words:
            [
                new("cat", "قطة", "🐱", "#FFE5E5", "#FF6B6B"),
                new("hat", "قبعة", "🎩", "#FFF3D6", "#F59E0B"),
                new("bat", "خفاش / مضرب", "🦇", "#EDE9FE", "#7C3AED"),
                new("mat", "سجادة", "🧶", "#DCFCE7", "#16A34A"),
                new("fan", "مروحة", "💨", "#E0F2FE", "#0284C7"),
                new("man", "رجل", "👨", "#FEF3C7", "#D97706"),
                new("pan", "مقلاة", "🍳", "#FFE4E6", "#E11D48"),
                new("van", "شاحنة", "🚐", "#DBEAFE", "#2563EB"),
                new("cap", "كاب", "🧢", "#E0E7FF", "#4F46E5"),
                new("map", "خريطة", "🗺️", "#DCFCE7", "#059669")
            ]),

        new PhonicsStage(
            Id: 2,
            TitleAr: "المرحلة 2: الكسرة والفتح",
            TitleEn: "Short E & I (-ed / -en / -ig / -in)",
            SubtitleAr: "تأسيس النطق والتمييز — مجانية",
            BadgeIcon: "🐷",
            GradientStart: "#10B981",
            GradientEnd: "#06B6D4",
            IsFree: true,
            Words:
            [
                new("bed", "سرير", "🛏️", "#E0F2FE", "#0284C7"),
                new("red", "أحمر", "🔴", "#FEE2E2", "#DC2626"),
                new("pen", "قلم", "🖊️", "#EDE9FE", "#7C3AED"),
                new("hen", "دجاجة", "🐔", "#FEF3C7", "#D97706"),
                new("net", "شبكة", "🥅", "#DCFCE7", "#16A34A"),
                new("pig", "خنزير صغير", "🐷", "#FCE7F3", "#DB2777"),
                new("wig", "باروكة", "👩‍🦱", "#FAE8FF", "#C026D3"),
                new("pin", "دبوس", "📌", "#FFE4E6", "#E11D48"),
                new("bin", "سلة", "🗑️", "#E2E8F0", "#475569"),
                new("fin", "زعنفة", "🦈", "#CFFAFE", "#0891B2")
            ]),

        new PhonicsStage(
            Id: 3,
            TitleAr: "المرحلة 3: الضمة القصيرة",
            TitleEn: "Short O (-ot / -og / -op)",
            SubtitleAr: "كلمات ممتعة مع الصور والأصوات",
            BadgeIcon: "🐶",
            GradientStart: "#8B5CF6",
            GradientEnd: "#EC4899",
            IsFree: false,
            Words:
            [
                new("dog", "كلب", "🐶", "#FEF3C7", "#D97706"),
                new("log", "جذع شجرة", "🪵", "#FFEDD5", "#C2410C"),
                new("frog", "ضفدع", "🐸", "#DCFCE7", "#16A34A"),
                new("hot", "ساخن", "🔥", "#FEE2E2", "#EA580C"),
                new("pot", "إناء", "🍲", "#E0F2FE", "#0284C7"),
                new("dot", "نقطة", "🔵", "#DBEAFE", "#2563EB"),
                new("hop", "يقفز", "🐇", "#FCE7F3", "#DB2777"),
                new("mop", "ممسحة", "🧹", "#CCFBF1", "#0D9488"),
                new("top", "قمة / نحلة دوارة", "🪀", "#EDE9FE", "#7C3AED"),
                new("box", "صندوق", "📦", "#FEF3C7", "#B45309")
            ]),

        new PhonicsStage(
            Id: 4,
            TitleAr: "المرحلة 4: الصوت القصير U",
            TitleEn: "Short U (-ug / -un / -ub)",
            SubtitleAr: "إتقان الحروف المتحركة الخمسة",
            BadgeIcon: "☀️",
            GradientStart: "#F59E0B",
            GradientEnd: "#EF4444",
            IsFree: false,
            Words:
            [
                new("sun", "شمس", "☀️", "#FEF9C3", "#CA8A04"),
                new("run", "يجري", "🏃", "#DBEAFE", "#2563EB"),
                new("bun", "خبز صغير", "🍔", "#FFEDD5", "#EA580C"),
                new("fun", "مرح", "🎉", "#FCE7F3", "#DB2777"),
                new("bug", "حشرة", "🐞", "#FEE2E2", "#DC2626"),
                new("mug", "كوب", "☕", "#E0F2FE", "#0284C7"),
                new("rug", "سجادة صغيرة", "🛋️", "#EDE9FE", "#7C3AED"),
                new("hug", "حضن", "🤗", "#FEF3C7", "#D97706"),
                new("tub", "حوض استحمام", "🛁", "#CFFAFE", "#0891B2"),
                new("cub", "شبل", "🦁", "#FFEDD5", "#D97706")
            ]),

        new PhonicsStage(
            Id: 5,
            TitleAr: "المرحلة 5: الكلمات البصرية 1",
            TitleEn: "Sight Words — Level 1",
            SubtitleAr: "أهم كلمات القراءة السريعة للأطفال",
            BadgeIcon: "⭐",
            GradientStart: "#3B82F6",
            GradientEnd: "#6366F1",
            IsFree: false,
            Words:
            [
                new("the", "الـ (أداة تعريف)", "📘", "#DBEAFE", "#2563EB", true),
                new("and", "و", "🔗", "#DCFCE7", "#16A34A", true),
                new("you", "أنت", "👉", "#FEF3C7", "#D97706", true),
                new("see", "يرى", "👀", "#E0F2FE", "#0284C7", true),
                new("can", "يستطيع", "💪", "#FCE7F3", "#DB2777", true),
                new("big", "كبير", "🐘", "#EDE9FE", "#7C3AED", true),
                new("for", "لأجل", "🎁", "#FFE4E6", "#E11D48", true),
                new("one", "واحد", "1️⃣", "#DCFCE7", "#059669", true),
                new("two", "اثنان", "2️⃣", "#DBEAFE", "#1D4ED8", true),
                new("she", "هي", "👧", "#FCE7F3", "#BE185D", true)
            ]),

        new PhonicsStage(
            Id: 6,
            TitleAr: "المرحلة 6: الكلمات البصرية 2",
            TitleEn: "Sight Words — Level 2",
            SubtitleAr: "تحدي القراءة والتركيب المتقدم",
            BadgeIcon: "🏆",
            GradientStart: "#7C3AED",
            GradientEnd: "#2563EB",
            IsFree: false,
            Words:
            [
                new("play", "يلعب", "⚽", "#DCFCE7", "#16A34A", true),
                new("look", "ينظر", "🔍", "#E0F2FE", "#0284C7", true),
                new("like", "يحب", "❤️", "#FEE2E2", "#DC2626", true),
                new("jump", "يقفز", "🦘", "#FEF3C7", "#D97706", true),
                new("help", "يساعد", "🤝", "#DBEAFE", "#2563EB", true),
                new("good", "جيد", "👍", "#DCFCE7", "#059669", true),
                new("come", "يأتي", "👋", "#FCE7F3", "#DB2777", true),
                new("here", "هنا", "📍", "#FFE4E6", "#E11D48", true),
                new("this", "هذا", "✨", "#EDE9FE", "#7C3AED", true),
                new("blue", "أزرق", "💙", "#DBEAFE", "#1D4ED8", true)
            ])
    ];
}
