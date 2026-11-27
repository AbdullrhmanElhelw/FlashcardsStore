namespace FlashcardsStore.PhonicsGame.Data;

/// <summary>Custom vector SVG avatars for the child profile &amp; Reward Shop (zero emojis).</summary>
public static class AvatarArtwork
{
    public static IReadOnlyList<(string Key, string LabelAr, string LabelEn)> Avatars { get; } =
    [
        ("lion", "أسد شجاع", "Brave Lion"),
        ("owl", "بومة ذكية", "Wise Owl"),
        ("fox", "ثعلب سريع", "Quick Fox"),
        ("astronaut", "رائد فضاء", "Explorer")
    ];

    public static IReadOnlyList<(string Key, string LabelAr, string LabelEn)> ShopAvatars { get; } =
    [
        ("panda", "باندا لطيف", "Cute Panda"),
        ("unicorn", "يونيكورن", "Unicorn"),
        ("rocket", "صاروخ", "Rocket"),
        ("crown_cat", "القط الملكي", "Royal Cat")
    ];

    public static string GetSvg(string key) => key switch
    {
        "owl" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="50" cy="52" r="36" fill="#7C3AED"/>
                <circle cx="37" cy="46" r="11" fill="#fff"/>
                <circle cx="63" cy="46" r="11" fill="#fff"/>
                <circle cx="37" cy="46" r="5" fill="#1E1B4B"/>
                <circle cx="63" cy="46" r="5" fill="#1E1B4B"/>
                <polygon points="50,53 44,62 56,62" fill="#F59E0B"/>
            </svg>
            """,
        "fox" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M22 36L20 14L42 26Z" fill="#EA580C"/>
                <path d="M78 36L80 14L58 26Z" fill="#EA580C"/>
                <circle cx="50" cy="54" r="32" fill="#F97316"/>
                <path d="M26 58Q50 84 74 58Q50 66 26 58Z" fill="#FFF7ED"/>
                <circle cx="39" cy="48" r="4" fill="#1E1B4B"/>
                <circle cx="61" cy="48" r="4" fill="#1E1B4B"/>
                <circle cx="50" cy="60" r="4" fill="#1E1B4B"/>
            </svg>
            """,
        "astronaut" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="50" cy="50" r="36" fill="#E2E8F0" stroke="#475569" stroke-width="4"/>
                <rect x="26" y="30" width="48" height="36" rx="16" fill="#1E293B"/>
                <circle cx="40" cy="46" r="3.5" fill="#38BDF8"/>
                <circle cx="60" cy="46" r="3.5" fill="#38BDF8"/>
                <path d="M43 56Q50 61 57 56" stroke="#38BDF8" stroke-width="2.5" stroke-linecap="round"/>
            </svg>
            """,
        "panda" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="24" cy="26" r="12" fill="#1E293B"/>
                <circle cx="76" cy="26" r="12" fill="#1E293B"/>
                <circle cx="50" cy="54" r="34" fill="#FFFFFF" stroke="#1E293B" stroke-width="3"/>
                <ellipse cx="37" cy="48" rx="9" ry="11" fill="#1E293B"/>
                <ellipse cx="63" cy="48" rx="9" ry="11" fill="#1E293B"/>
                <circle cx="37" cy="46" r="3.5" fill="#FFFFFF"/>
                <circle cx="63" cy="46" r="3.5" fill="#FFFFFF"/>
                <ellipse cx="50" cy="62" rx="6" ry="4" fill="#1E293B"/>
            </svg>
            """,
        "unicorn" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="50,8 42,32 58,32" fill="#FACC15" stroke="#CA8A04" stroke-width="2.5"/>
                <circle cx="50" cy="56" r="32" fill="#FCE7F3" stroke="#DB2777" stroke-width="3"/>
                <path d="M24 38C28 24 44 22 50 28" stroke="#EC4899" stroke-width="6" stroke-linecap="round"/>
                <circle cx="39" cy="52" r="4" fill="#1E1B4B"/>
                <circle cx="61" cy="52" r="4" fill="#1E1B4B"/>
                <path d="M42 66Q50 72 58 66" stroke="#DB2777" stroke-width="3" stroke-linecap="round"/>
            </svg>
            """,
        "rocket" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M50 14C66 28 70 50 64 74H36C30 50 34 28 50 14Z" fill="#3B82F6" stroke="#1E3A8A" stroke-width="3"/>
                <circle cx="50" cy="44" r="9" fill="#DBEAFE" stroke="#1E3A8A" stroke-width="2.5"/>
                <path d="M36 58L22 74H36M64 58L78 74H64" fill="#EF4444" stroke="#991B1B" stroke-width="2.5" stroke-linejoin="round"/>
                <polygon points="44,74 56,74 50,90" fill="#FACC15"/>
            </svg>
            """,
        "crown_cat" => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="32,26 40,12 50,22 60,12 68,26" fill="#FACC15" stroke="#CA8A04" stroke-width="2.5" stroke-linejoin="round"/>
                <circle cx="50" cy="56" r="31" fill="#FBBF24" stroke="#92400E" stroke-width="3"/>
                <circle cx="39" cy="52" r="4" fill="#1E1B4B"/>
                <circle cx="61" cy="52" r="4" fill="#1E1B4B"/>
                <polygon points="50,60 45,66 55,66" fill="#F43F5E"/>
            </svg>
            """,
        _ => """
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="50" cy="52" r="36" fill="#D97706"/>
                <circle cx="50" cy="54" r="24" fill="#FDE68A"/>
                <circle cx="41" cy="49" r="3.8" fill="#1E1B4B"/>
                <circle cx="59" cy="49" r="3.8" fill="#1E1B4B"/>
                <polygon points="50,55 45,61 55,61" fill="#92400E"/>
                <path d="M43 65Q50 70 57 65" stroke="#92400E" stroke-width="2.5" stroke-linecap="round"/>
            </svg>
            """
    };
}
