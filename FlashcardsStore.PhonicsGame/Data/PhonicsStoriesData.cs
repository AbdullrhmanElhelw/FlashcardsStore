using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Data;

public static class PhonicsStoriesData
{
    public static IReadOnlyList<PhonicsStory> Stories { get; } =
    [
        new PhonicsStory(
            Id: 1,
            TitleAr: "القط سام والقبعة",
            TitleEn: "Sam the Cat & the Hat",
            SubtitleAr: "قصة مصورة لصوت Short A — مجانية",
            SubtitleEn: "Animated Short A Story — Free",
            CoverArtworkKey: "cat",
            AccentColor: "#F97316",
            IsFree: true,
            Scenes:
            [
                new("Sam is a big fat cat.", "سام قط كبير ولطيف.", "cat", "cat", "#FFF1F2", "#F97316", "anim-bounce"),
                new("Sam sits on a soft mat.", "يجلس سام على سجادة ناعمة.", "mat", "mat", "#ECFDF5", "#10B981", "anim-float"),
                new("Sam has a tall blue hat.", "يمتلك سام قبعة طويلة جميلة.", "hat", "hat", "#EEF2FF", "#4F46E5", "anim-wiggle"),
                new("Sam rides in a fast van!", "يركب سام في شاحنة سريعة!", "van", "van", "#EFF6FF", "#2563EB", "anim-slide")
            ],
            QuizQuestionAr: "على ماذا جلس سام؟",
            QuizQuestionEn: "Where did Sam sit?",
            QuizCorrectWord: "mat",
            QuizOptions: ["mat", "pan", "fan"]),

        new PhonicsStory(
            Id: 2,
            TitleAr: "الخنزير بيب والقلم",
            TitleEn: "Pip the Pig & the Pen",
            SubtitleAr: "قصة مصورة لصوت Short E & I",
            SubtitleEn: "Animated Short E & I Story",
            CoverArtworkKey: "pig",
            AccentColor: "#10B981",
            IsFree: false,
            Scenes:
            [
                new("Pip is a happy pink pig.", "بيب خنزير صغير سعيد.", "pig", "pig", "#FDF2F8", "#DB2777", "anim-bounce"),
                new("Pip has a red pen.", "بيب لديه قلم أحمر.", "pen", "pen", "#F5F3FF", "#7C3AED", "anim-wiggle"),
                new("Pip sees a little hen.", "يرى بيب دجاجة صغيرة.", "hen", "hen", "#FFFBEB", "#D97706", "anim-float"),
                new("Pip naps on his warm bed.", "ينام بيب على سريره الدافئ.", "bed", "bed", "#F0F9FF", "#0284C7", "anim-float")
            ],
            QuizQuestionAr: "ماذا كان لدى بيب؟",
            QuizQuestionEn: "What did Pip have?",
            QuizCorrectWord: "pen",
            QuizOptions: ["pen", "net", "bin"]),

        new PhonicsStory(
            Id: 3,
            TitleAr: "الكلب بوب والشمس",
            TitleEn: "Bob the Dog & the Sun",
            SubtitleAr: "قصة مصورة لصوت Short O & U",
            SubtitleEn: "Animated Short O & U Story",
            CoverArtworkKey: "dog",
            AccentColor: "#8B5CF6",
            IsFree: false,
            Scenes:
            [
                new("Bob is a playful dog.", "بوب كلب مرح يحب اللعب.", "dog", "dog", "#FFFBEB", "#D97706", "anim-bounce"),
                new("The bright sun is up!", "الشمس المشرقة تسطع في السماء!", "sun", "sun", "#FEFCE8", "#CA8A04", "anim-spin-slow"),
                new("Bob sees a tiny red bug.", "يرى بوب حشرة حمراء صغيرة.", "bug", "bug", "#FEF2F2", "#DC2626", "anim-wiggle"),
                new("Bob drinks from a cool mug.", "يشرب بوب من كوب جميل.", "mug", "mug", "#EFF6FF", "#0284C7", "anim-float")
            ],
            QuizQuestionAr: "ماذا رأى بوب تحت الشمس؟",
            QuizQuestionEn: "What did Bob see in the sun?",
            QuizCorrectWord: "bug",
            QuizOptions: ["bug", "box", "pot"])
    ];
}
