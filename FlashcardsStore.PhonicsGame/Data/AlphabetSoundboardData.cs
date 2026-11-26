using FlashcardsStore.PhonicsGame.Models;

namespace FlashcardsStore.PhonicsGame.Data;

public static class AlphabetSoundboardData
{
    public static IReadOnlyList<PhonicsLetterItem> Letters { get; } =
    [
        new('A', "Apple", "تفاحة", true),
        new('B', "Ball", "كرة", false),
        new('C', "Cat", "قطة", false),
        new('D', "Dog", "كلب", false),
        new('E', "Egg", "بيضة", true),
        new('F', "Fan", "مروحة", false),
        new('G', "Goat", "ماعز", false),
        new('H', "Hat", "قبعة", false),
        new('I', "Igloo", "كوخ جليدي", true),
        new('J', "Jump", "يقفز", false),
        new('K', "Kite", "طائرة ورقية", false),
        new('L', "Lion", "أسد", false),
        new('M', "Map", "خريطة", false),
        new('N', "Net", "شبكة", false),
        new('O', "Octopus", "أخطبوط", true),
        new('P', "Pen", "قلم", false),
        new('Q', "Queen", "ملكة", false),
        new('R', "Run", "يجري", false),
        new('S', "Sun", "شمس", false),
        new('T', "Top", "قمة", false),
        new('U', "Umbrella", "مظلة", true),
        new('V', "Van", "شاحنة", false),
        new('W', "Wig", "باروكة", false),
        new('X', "Box", "صندوق", false),
        new('Y', "Yellow", "أصفر", false),
        new('Z', "Zebra", "حمار وحشي", false)
    ];
}
