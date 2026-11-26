namespace FlashcardsStore.PhonicsGame.Data;

/// <summary>Custom vector SVG illustrations for every phonics word and stage (zero emojis).</summary>
public static class WordArtwork
{
    public static string GetSvg(string key, string accentColor = "#2B071C")
    {
        if (SvgMap.TryGetValue(key, out var svg))
            return svg;

        // Stylized Sight-Word / Fallback vector card
        var label = key.StartsWith("sight_", StringComparison.OrdinalIgnoreCase)
            ? key[6..].ToUpperInvariant()
            : key.ToUpperInvariant();

        return $"""
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="14" y="20" width="92" height="80" rx="22" fill="{accentColor}" fill-opacity="0.14" stroke="{accentColor}" stroke-width="4"/>
                <circle cx="92" cy="34" r="8" fill="{accentColor}" fill-opacity="0.35"/>
                <circle cx="28" cy="86" r="6" fill="{accentColor}" fill-opacity="0.25"/>
                <text x="60" y="68" text-anchor="middle" font-family="Cairo, sans-serif" font-size="28" font-weight="900" fill="{accentColor}">{label}</text>
            </svg>
            """;
    }

    private static readonly Dictionary<string, string> SvgMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["cat"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M26 42L22 18L46 30Z" fill="#F59E0B" stroke="#92400E" stroke-width="3" stroke-linejoin="round"/>
                <path d="M94 42L98 18L74 30Z" fill="#F59E0B" stroke="#92400E" stroke-width="3" stroke-linejoin="round"/>
                <ellipse cx="60" cy="64" rx="38" ry="33" fill="#FBBF24" stroke="#92400E" stroke-width="3"/>
                <circle cx="45" cy="58" r="5" fill="#1E1B4B"/>
                <circle cx="75" cy="58" r="5" fill="#1E1B4B"/>
                <polygon points="60,67 54,73 66,73" fill="#F43F5E"/>
                <path d="M51 77Q60 84 69 77" stroke="#92400E" stroke-width="3" stroke-linecap="round"/>
                <path d="M18 64H34M18 72H34M86 64H102M86 72H102" stroke="#92400E" stroke-width="2.5" stroke-linecap="round"/>
            </svg>
            """,
        ["hat"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M36 74L42 28C43 23 47 20 52 20H68C73 20 77 23 78 28L84 74H36Z" fill="#312E81" stroke="#1E1B4B" stroke-width="3"/>
                <rect x="37" y="60" width="46" height="11" fill="#F59E0B"/>
                <ellipse cx="60" cy="76" rx="46" ry="11" fill="#4338CA" stroke="#1E1B4B" stroke-width="3"/>
            </svg>
            """,
        ["bat"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M60 52C44 36 22 38 12 52C22 56 26 64 26 72C36 66 44 68 50 74L60 66L70 74C76 68 84 66 94 72C94 64 98 56 108 52C98 38 76 36 60 52Z" fill="#6D28D9" stroke="#3B0764" stroke-width="3" stroke-linejoin="round"/>
                <ellipse cx="60" cy="60" rx="14" ry="18" fill="#4C1D95" stroke="#3B0764" stroke-width="3"/>
                <circle cx="54" cy="56" r="3" fill="#FDE047"/>
                <circle cx="66" cy="56" r="3" fill="#FDE047"/>
            </svg>
            """,
        ["mat"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="18" y="34" width="84" height="52" rx="10" fill="#10B981" stroke="#065F46" stroke-width="3"/>
                <rect x="28" y="44" width="64" height="32" rx="6" fill="#A7F3D0" stroke="#059669" stroke-width="2.5"/>
                <path d="M12 42H18M12 54H18M12 66H18M12 78H18M102 42H108M102 54H108M102 66H108M102 78H108" stroke="#065F46" stroke-width="3" stroke-linecap="round"/>
            </svg>
            """,
        ["fan"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="54" y="64" width="12" height="32" rx="4" fill="#64748B"/>
                <rect x="36" y="92" width="48" height="10" rx="5" fill="#475569"/>
                <circle cx="60" cy="48" r="28" fill="#E0F2FE" stroke="#0284C7" stroke-width="3"/>
                <ellipse cx="60" cy="31" rx="7" ry="12" fill="#38BDF8"/>
                <ellipse cx="75" cy="57" rx="12" ry="7" transform="rotate(28 75 57)" fill="#38BDF8"/>
                <ellipse cx="45" cy="57" rx="12" ry="7" transform="rotate(-28 45 57)" fill="#38BDF8"/>
                <circle cx="60" cy="48" r="7" fill="#0369A1"/>
            </svg>
            """,
        ["man"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M32 98C32 80 44 72 60 72C76 72 88 80 88 98" fill="#2563EB" stroke="#1E3A8A" stroke-width="3" stroke-linecap="round"/>
                <circle cx="60" cy="44" r="20" fill="#FDE68A" stroke="#92400E" stroke-width="3"/>
                <path d="M41 38C43 24 77 24 79 38C72 32 48 32 41 38Z" fill="#451A03"/>
                <circle cx="53" cy="44" r="2.8" fill="#1E1B4B"/>
                <circle cx="67" cy="44" r="2.8" fill="#1E1B4B"/>
                <path d="M53 53Q60 58 67 53" stroke="#92400E" stroke-width="2.5" stroke-linecap="round"/>
            </svg>
            """,
        ["pan"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="68" y="54" width="38" height="10" rx="5" fill="#475569" stroke="#1E293B" stroke-width="2.5"/>
                <circle cx="48" cy="59" r="28" fill="#334155" stroke="#0F172A" stroke-width="3"/>
                <circle cx="48" cy="59" r="20" fill="#64748B"/>
                <circle cx="46" cy="58" r="8" fill="#FEF08A"/>
                <circle cx="46" cy="58" r="4" fill="#F59E0B"/>
            </svg>
            """,
        ["van"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 40C18 34 23 30 29 30H76L98 52V78H18V40Z" fill="#3B82F6" stroke="#1E3A8A" stroke-width="3" stroke-linejoin="round"/>
                <rect x="28" y="38" width="22" height="16" rx="3" fill="#DBEAFE"/>
                <path d="M58 38H74L88 54H58V38Z" fill="#DBEAFE"/>
                <circle cx="38" cy="80" r="11" fill="#1E293B" stroke="#F8FAFC" stroke-width="3"/>
                <circle cx="82" cy="80" r="11" fill="#1E293B" stroke="#F8FAFC" stroke-width="3"/>
            </svg>
            """,
        ["cap"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M24 68C24 46 40 32 60 32C80 32 96 46 96 68H24Z" fill="#4F46E5" stroke="#1E1B4B" stroke-width="3"/>
                <path d="M60 68H106C106 76 92 80 72 80H24V68H60Z" fill="#312E81" stroke="#1E1B4B" stroke-width="3" stroke-linejoin="round"/>
                <circle cx="60" cy="30" r="4" fill="#F59E0B"/>
            </svg>
            """,
        ["map"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 32L46 24L74 34L102 24V88L74 96L46 86L18 96V32Z" fill="#FEF3C7" stroke="#B45309" stroke-width="3" stroke-linejoin="round"/>
                <path d="M46 24V86M74 34V96" stroke="#D97706" stroke-width="2.5" stroke-dasharray="4 4"/>
                <path d="M32 66C44 50 58 64 70 50C78 42 86 48 90 54" stroke="#10B981" stroke-width="3.5" stroke-linecap="round"/>
                <circle cx="88" cy="54" r="5" fill="#EF4444"/>
            </svg>
            """,
        ["bed"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 40V88M102 52V88" stroke="#78350F" stroke-width="6" stroke-linecap="round"/>
                <rect x="20" y="60" width="80" height="20" rx="6" fill="#38BDF8" stroke="#0369A1" stroke-width="3"/>
                <rect x="24" y="48" width="22" height="12" rx="5" fill="#FFFFFF" stroke="#0284C7" stroke-width="2.5"/>
            </svg>
            """,
        ["red"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="60" r="36" fill="#EF4444" stroke="#991B1B" stroke-width="4"/>
                <ellipse cx="47" cy="44" rx="10" ry="6" transform="rotate(-30 47 44)" fill="#FCA5A5" fill-opacity="0.7"/>
            </svg>
            """,
        ["pen"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <g transform="rotate(-35 60 60)">
                    <rect x="48" y="22" width="24" height="62" rx="6" fill="#7C3AED" stroke="#4C1D95" stroke-width="3"/>
                    <polygon points="48,84 72,84 60,104" fill="#FDE68A" stroke="#4C1D95" stroke-width="3" stroke-linejoin="round"/>
                    <circle cx="60" cy="102" r="3" fill="#1E1B4B"/>
                </g>
            </svg>
            """,
        ["hen"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="60" cy="66" rx="30" ry="24" fill="#FDE68A" stroke="#B45309" stroke-width="3"/>
                <circle cx="45" cy="46" r="14" fill="#FDE68A" stroke="#B45309" stroke-width="3"/>
                <path d="M40 30C42 22 50 22 52 30" fill="#EF4444"/>
                <polygon points="31,46 21,50 31,54" fill="#F59E0B" stroke="#B45309" stroke-width="2"/>
                <circle cx="41" cy="44" r="3" fill="#1E1B4B"/>
            </svg>
            """,
        ["net"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M24 32H96L84 86H36L24 32Z" fill="#DCFCE7" stroke="#15803D" stroke-width="3.5" stroke-linejoin="round"/>
                <path d="M40 32L46 86M60 32V86M80 32L74 86M28 50H92M32 68H88" stroke="#16A34A" stroke-width="2.5"/>
            </svg>
            """,
        ["pig"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M26 42L22 20L44 30Z" fill="#F9A8D4" stroke="#9D174D" stroke-width="3" stroke-linejoin="round"/>
                <path d="M94 42L98 20L76 30Z" fill="#F9A8D4" stroke="#9D174D" stroke-width="3" stroke-linejoin="round"/>
                <circle cx="60" cy="62" r="35" fill="#FBCFE8" stroke="#9D174D" stroke-width="3"/>
                <ellipse cx="60" cy="70" rx="16" ry="11" fill="#F472B6" stroke="#9D174D" stroke-width="2.5"/>
                <circle cx="54" cy="70" r="2.8" fill="#831843"/>
                <circle cx="66" cy="70" r="2.8" fill="#831843"/>
                <circle cx="44" cy="52" r="4" fill="#1E1B4B"/>
                <circle cx="76" cy="52" r="4" fill="#1E1B4B"/>
            </svg>
            """,
        ["wig"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M26 68C22 40 38 22 60 22C82 22 98 40 94 68C94 82 84 92 78 92C78 70 72 52 60 52C48 52 42 70 42 92C36 92 26 82 26 68Z" fill="#C026D3" stroke="#701A75" stroke-width="3" stroke-linejoin="round"/>
                <circle cx="60" cy="64" r="14" fill="#FDE68A" stroke="#92400E" stroke-width="2.5"/>
            </svg>
            """,
        ["pin"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <line x1="60" y1="54" x2="60" y2="100" stroke="#64748B" stroke-width="5" stroke-linecap="round"/>
                <circle cx="60" cy="38" r="18" fill="#E11D48" stroke="#881337" stroke-width="3"/>
                <circle cx="54" cy="32" r="5" fill="#FDA4AF"/>
            </svg>
            """,
        ["bin"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="30" y="34" width="60" height="62" rx="10" fill="#64748B" stroke="#1E293B" stroke-width="3"/>
                <rect x="24" y="26" width="72" height="10" rx="5" fill="#475569" stroke="#1E293B" stroke-width="3"/>
                <line x1="46" y1="46" x2="46" y2="82" stroke="#CBD5E1" stroke-width="4" stroke-linecap="round"/>
                <line x1="60" y1="46" x2="60" y2="82" stroke="#CBD5E1" stroke-width="4" stroke-linecap="round"/>
                <line x1="74" y1="46" x2="74" y2="82" stroke="#CBD5E1" stroke-width="4" stroke-linecap="round"/>
            </svg>
            """,
        ["fin"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 84C32 80 46 86 60 82C74 78 88 86 102 82" stroke="#0284C7" stroke-width="4" stroke-linecap="round"/>
                <path d="M36 80C40 48 58 28 80 26C72 44 74 64 82 80H36Z" fill="#06B6D4" stroke="#155E75" stroke-width="3" stroke-linejoin="round"/>
            </svg>
            """,
        ["dog"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="28" cy="58" rx="12" ry="24" fill="#92400E" stroke="#451A03" stroke-width="3"/>
                <ellipse cx="92" cy="58" rx="12" ry="24" fill="#92400E" stroke="#451A03" stroke-width="3"/>
                <circle cx="60" cy="60" r="32" fill="#FDE68A" stroke="#78350F" stroke-width="3"/>
                <circle cx="47" cy="54" r="4.5" fill="#1E1B4B"/>
                <circle cx="73" cy="54" r="4.5" fill="#1E1B4B"/>
                <ellipse cx="60" cy="67" rx="8" ry="6" fill="#1E1B4B"/>
                <path d="M55 77Q60 85 65 77" fill="#FB7185" stroke="#BE123C" stroke-width="2"/>
            </svg>
            """,
        ["sun"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="60" r="26" fill="#FACC15" stroke="#CA8A04" stroke-width="3"/>
                <path d="M60 16V26M60 94V104M16 60H26M94 60H104M29 29L36 36M84 84L91 91M91 29L84 36M36 84L29 91" stroke="#F59E0B" stroke-width="5" stroke-linecap="round"/>
            </svg>
            """,
        ["bug"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="64" r="26" fill="#EF4444" stroke="#7F1D1D" stroke-width="3"/>
                <path d="M60 38V90" stroke="#1E1B4B" stroke-width="3"/>
                <circle cx="60" cy="36" r="11" fill="#1E1B4B"/>
                <circle cx="48" cy="56" r="4" fill="#1E1B4B"/>
                <circle cx="72" cy="56" r="4" fill="#1E1B4B"/>
                <circle cx="49" cy="74" r="4" fill="#1E1B4B"/>
                <circle cx="71" cy="74" r="4" fill="#1E1B4B"/>
            </svg>
            """,
        ["box"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="60,22 98,40 60,58 22,40" fill="#FCD34D" stroke="#92400E" stroke-width="3" stroke-linejoin="round"/>
                <polygon points="22,40 60,58 60,96 22,78" fill="#F59E0B" stroke="#92400E" stroke-width="3" stroke-linejoin="round"/>
                <polygon points="98,40 60,58 60,96 98,78" fill="#D97706" stroke="#92400E" stroke-width="3" stroke-linejoin="round"/>
            </svg>
            """,
        ["pot"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="26" y="46" width="68" height="44" rx="14" fill="#0284C7" stroke="#0C4A6E" stroke-width="3"/>
                <rect x="22" y="38" width="76" height="10" rx="5" fill="#38BDF8" stroke="#0C4A6E" stroke-width="3"/>
                <path d="M52 38C52 30 68 30 68 38" stroke="#0C4A6E" stroke-width="3"/>
            </svg>
            """,
        ["mug"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M80 46H90C96 46 100 52 100 60C100 68 96 74 90 74H80" stroke="#0369A1" stroke-width="6" stroke-linecap="round"/>
                <rect x="28" y="38" width="52" height="52" rx="10" fill="#38BDF8" stroke="#0369A1" stroke-width="3"/>
                <ellipse cx="54" cy="38" rx="26" ry="6" fill="#BAE6FD" stroke="#0369A1" stroke-width="2.5"/>
            </svg>
            """,
        ["star_badge"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M60 16L73 43L102 47L81 68L86 97L60 83L34 97L39 68L18 47L47 43L60 16Z" fill="#FDE047" stroke="#CA8A04" stroke-width="3.5" stroke-linejoin="round"/>
            </svg>
            """,
        ["trophy_badge"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M38 26H82V54C82 66 72 76 60 76C48 76 38 66 38 54V26Z" fill="#FACC15" stroke="#A16207" stroke-width="3.5"/>
                <path d="M38 34H24V46C24 54 30 58 38 58M82 34H96V46C96 54 90 58 82 58" stroke="#A16207" stroke-width="3.5" stroke-linecap="round"/>
                <path d="M60 76V92M42 92H78" stroke="#A16207" stroke-width="5" stroke-linecap="round"/>
            </svg>
            """
    };
}
