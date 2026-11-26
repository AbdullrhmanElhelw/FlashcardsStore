using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Data;

public static class PhonicsCurriculum
{
    public static IReadOnlyList<PhonicsStage> Stages { get; } =
    [
        new PhonicsStage(
            Id: 1,
            TitleAr: "المرحلة 1: الفتحة القصيرة",
            TitleEn: "Stage 1: Short A",
            SubtitleAr: "كلمات ثلاثية سهلة (-at / -an / -ap) — مجانية",
            SubtitleEn: "Easy CVC words (-at / -an / -ap) — Free",
            BadgeKey: "cat",
            GradientStart: "#F97316",
            GradientEnd: "#EA580C",
            IsFree: true,
            Words:
            [
                new("cat", "قطة", "cat", "#FFE5E5", "#FF6B6B"),
                new("hat", "قبعة", "hat", "#FFF3D6", "#F59E0B"),
                new("bat", "خفاش", "bat", "#EDE9FE", "#7C3AED"),
                new("mat", "سجادة", "mat", "#DCFCE7", "#16A34A"),
                new("fan", "مروحة", "fan", "#E0F2FE", "#0284C7"),
                new("man", "رجل", "man", "#FEF3C7", "#D97706"),
                new("pan", "مقلاة", "pan", "#FFE4E6", "#E11D48"),
                new("van", "شاحنة", "van", "#DBEAFE", "#2563EB"),
                new("cap", "كاب", "cap", "#E0E7FF", "#4F46E5"),
                new("map", "خريطة", "map", "#DCFCE7", "#059669")
            ]),

        new PhonicsStage(
            Id: 2,
            TitleAr: "المرحلة 2: الكسرة والفتح",
            TitleEn: "Stage 2: Short E & I",
            SubtitleAr: "تأسيس النطق والتمييز (-ed / -en / -ig / -in) — مجانية",
            SubtitleEn: "Phonics & sound contrast (-ed / -en / -ig / -in) — Free",
            BadgeKey: "pig",
            GradientStart: "#10B981",
            GradientEnd: "#0D9488",
            IsFree: true,
            Words:
            [
                new("bed", "سرير", "bed", "#E0F2FE", "#0284C7"),
                new("red", "أحمر", "red", "#FEE2E2", "#DC2626"),
                new("pen", "قلم", "pen", "#EDE9FE", "#7C3AED"),
                new("hen", "دجاجة", "hen", "#FEF3C7", "#D97706"),
                new("net", "شبكة", "net", "#DCFCE7", "#16A34A"),
                new("pig", "خنزير صغير", "pig", "#FCE7F3", "#DB2777"),
                new("wig", "باروكة", "wig", "#FAE8FF", "#C026D3"),
                new("pin", "دبوس", "pin", "#FFE4E6", "#E11D48"),
                new("bin", "سلة", "bin", "#E2E8F0", "#475569"),
                new("fin", "زعنفة", "fin", "#CFFAFE", "#0891B2")
            ]),

        new PhonicsStage(
            Id: 3,
            TitleAr: "المرحلة 3: الضمة القصيرة",
            TitleEn: "Stage 3: Short O",
            SubtitleAr: "كلمات ممتعة (-ot / -og / -op)",
            SubtitleEn: "Fun words (-ot / -og / -op)",
            BadgeKey: "dog",
            GradientStart: "#8B5CF6",
            GradientEnd: "#C026D3",
            IsFree: false,
            Words:
            [
                new("dog", "كلب", "dog", "#FEF3C7", "#D97706"),
                new("log", "جذع شجرة", "log", "#FFEDD5", "#C2410C"),
                new("frog", "ضفدع", "frog", "#DCFCE7", "#16A34A"),
                new("hot", "ساخن", "hot", "#FEE2E2", "#EA580C"),
                new("pot", "إناء", "pot", "#E0F2FE", "#0284C7"),
                new("dot", "نقطة", "dot", "#DBEAFE", "#2563EB"),
                new("hop", "يقفز", "hop", "#FCE7F3", "#DB2777"),
                new("mop", "ممسحة", "mop", "#CCFBF1", "#0D9488"),
                new("top", "نحلة دوارة", "top", "#EDE9FE", "#7C3AED"),
                new("box", "صندوق", "box", "#FEF3C7", "#B45309")
            ]),

        new PhonicsStage(
            Id: 4,
            TitleAr: "المرحلة 4: الصوت القصير U",
            TitleEn: "Stage 4: Short U",
            SubtitleAr: "إتقان الحروف المتحركة الخمسة (-ug / -un / -ub)",
            SubtitleEn: "Master all 5 short vowels (-ug / -un / -ub)",
            BadgeKey: "sun",
            GradientStart: "#F59E0B",
            GradientEnd: "#E11D48",
            IsFree: false,
            Words:
            [
                new("sun", "شمس", "sun", "#FEF9C3", "#CA8A04"),
                new("run", "يجري", "run", "#DBEAFE", "#2563EB"),
                new("bun", "خبز صغير", "bun", "#FFEDD5", "#EA580C"),
                new("fun", "مرح", "fun", "#FCE7F3", "#DB2777"),
                new("bug", "حشرة", "bug", "#FEE2E2", "#DC2626"),
                new("mug", "كوب", "mug", "#E0F2FE", "#0284C7"),
                new("rug", "سجادة صغيرة", "rug", "#EDE9FE", "#7C3AED"),
                new("hug", "حضن", "hug", "#FEF3C7", "#D97706"),
                new("tub", "حوض استحمام", "tub", "#CFFAFE", "#0891B2"),
                new("cub", "شبل", "cub", "#FFEDD5", "#D97706")
            ]),

        new PhonicsStage(
            Id: 5,
            TitleAr: "المرحلة 5: الكلمات البصرية 1",
            TitleEn: "Stage 5: Sight Words I",
            SubtitleAr: "أهم كلمات القراءة السريعة للأطفال",
            SubtitleEn: "Essential high-frequency reading words",
            BadgeKey: "star_badge",
            GradientStart: "#3B82F6",
            GradientEnd: "#4F46E5",
            IsFree: false,
            Words:
            [
                new("the", "أداة تعريف", "sight_the", "#DBEAFE", "#2563EB", true),
                new("and", "و", "sight_and", "#DCFCE7", "#16A34A", true),
                new("you", "أنت", "sight_you", "#FEF3C7", "#D97706", true),
                new("see", "يرى", "sight_see", "#E0F2FE", "#0284C7", true),
                new("can", "يستطيع", "sight_can", "#FCE7F3", "#DB2777", true),
                new("big", "كبير", "sight_big", "#EDE9FE", "#7C3AED", true),
                new("for", "لأجل", "sight_for", "#FFE4E6", "#E11D48", true),
                new("one", "واحد", "sight_one", "#DCFCE7", "#059669", true),
                new("two", "اثنان", "sight_two", "#DBEAFE", "#1D4ED8", true),
                new("she", "هي", "sight_she", "#FCE7F3", "#BE185D", true)
            ]),

        new PhonicsStage(
            Id: 6,
            TitleAr: "المرحلة 6: الكلمات البصرية 2",
            TitleEn: "Stage 6: Sight Words II",
            SubtitleAr: "تحدي القراءة والتركيب المتقدم",
            SubtitleEn: "Advanced reading & word building",
            BadgeKey: "trophy_badge",
            GradientStart: "#7C3AED",
            GradientEnd: "#2563EB",
            IsFree: false,
            Words:
            [
                new("play", "يلعب", "sight_play", "#DCFCE7", "#16A34A", true),
                new("look", "ينظر", "sight_look", "#E0F2FE", "#0284C7", true),
                new("like", "يحب", "sight_like", "#FEE2E2", "#DC2626", true),
                new("jump", "يقفز", "sight_jump", "#FEF3C7", "#D97706", true),
                new("help", "يساعد", "sight_help", "#DBEAFE", "#2563EB", true),
                new("good", "جيد", "sight_good", "#DCFCE7", "#059669", true),
                new("come", "يأتي", "sight_come", "#FCE7F3", "#DB2777", true),
                new("here", "هنا", "sight_here", "#FFE4E6", "#E11D48", true),
                new("this", "هذا", "sight_this", "#EDE9FE", "#7C3AED", true),
                new("blue", "أزرق", "sight_blue", "#DBEAFE", "#1D4ED8", true)
            ])
    ];
}
