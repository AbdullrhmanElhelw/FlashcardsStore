namespace FlashcardsStore.PhonicsGame.Data;

/// <summary>Custom vector SVG avatars for the child profile (zero emojis).</summary>
public static class AvatarArtwork
{
    public static IReadOnlyList<(string Key, string LabelAr, string LabelEn)> Avatars { get; } =
    [
        ("lion", "أسد شجاع", "Brave Lion"),
        ("owl", "بومة ذكية", "Wise Owl"),
        ("fox", "ثعلب سريع", "Quick Fox"),
        ("astronaut", "رائد فضاء", "Explorer")
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
