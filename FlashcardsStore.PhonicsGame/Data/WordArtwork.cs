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
        ["fig"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M60 22C66 22 70 28 70 36C82 44 88 60 84 76C80 90 70 96 60 96C50 96 40 90 36 76C32 60 38 44 50 36C50 28 54 22 60 22Z" fill="#7E22CE" stroke="#4C1D95" stroke-width="3"/>
                <path d="M60 22C64 14 74 14 78 20C72 24 65 24 60 22Z" fill="#16A34A"/>
            </svg>
            """,
        ["bee"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="46" cy="36" rx="14" ry="10" fill="#E0F2FE" stroke="#0284C7" stroke-width="2.5"/>
                <ellipse cx="74" cy="36" rx="14" ry="10" fill="#E0F2FE" stroke="#0284C7" stroke-width="2.5"/>
                <ellipse cx="60" cy="64" rx="30" ry="22" fill="#FACC15" stroke="#1E1B4B" stroke-width="3"/>
                <path d="M48 44V84M62 42V86M76 46V82" stroke="#1E1B4B" stroke-width="5"/>
            </svg>
            """,
        ["boat"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="60,22 60,64 88,64" fill="#F97316" stroke="#9A3412" stroke-width="3" stroke-linejoin="round"/>
                <path d="M20 68H100L86 88H34L20 68Z" fill="#0284C7" stroke="#0C4A6E" stroke-width="3" stroke-linejoin="round"/>
            </svg>
            """,
        ["car"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M22 62L34 40H80L98 62V78H22V62Z" fill="#EF4444" stroke="#991B1B" stroke-width="3" stroke-linejoin="round"/>
                <rect x="38" y="45" width="38" height="14" rx="3" fill="#DBEAFE"/>
                <circle cx="38" cy="80" r="10" fill="#1E293B" stroke="#F8FAFC" stroke-width="3"/>
                <circle cx="82" cy="80" r="10" fill="#1E293B" stroke="#F8FAFC" stroke-width="3"/>
            </svg>
            """,
        ["bird"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="58" cy="64" rx="28" ry="22" fill="#38BDF8" stroke="#0369A1" stroke-width="3"/>
                <circle cx="74" cy="46" r="14" fill="#38BDF8" stroke="#0369A1" stroke-width="3"/>
                <polygon points="88,46 100,50 88,54" fill="#F59E0B"/>
                <circle cx="78" cy="44" r="3" fill="#1E1B4B"/>
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
            """,
        ["flag"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <line x1="30" y1="20" x2="30" y2="100" stroke="#475569" stroke-width="5" stroke-linecap="round"/>
                <path d="M32 24H94L82 42L94 60H32V24Z" fill="#0EA5E9" stroke="#0369A1" stroke-width="3" stroke-linejoin="round"/>
            </svg>
            """,
        ["star"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M60 16L73 43L102 47L81 68L86 97L60 83L34 97L39 68L18 47L47 43L60 16Z" fill="#FACC15" stroke="#CA8A04" stroke-width="3.5" stroke-linejoin="round"/>
            </svg>
            """,
        ["crab"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="60" cy="68" rx="30" ry="22" fill="#EF4444" stroke="#991B1B" stroke-width="3"/>
                <circle cx="48" cy="46" r="5" fill="#1E1B4B"/>
                <circle cx="72" cy="46" r="5" fill="#1E1B4B"/>
                <path d="M30 56C18 46 20 30 32 32C28 40 30 46 36 50" fill="#EF4444" stroke="#991B1B" stroke-width="3"/>
                <path d="M90 56C102 46 100 30 88 32C92 40 90 46 84 50" fill="#EF4444" stroke="#991B1B" stroke-width="3"/>
            </svg>
            """,
        ["drum"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="24" y="42" width="72" height="44" rx="10" fill="#8B5CF6" stroke="#4C1D95" stroke-width="3"/>
                <ellipse cx="60" cy="42" rx="36" ry="11" fill="#FEF3C7" stroke="#4C1D95" stroke-width="3"/>
                <path d="M36 52L48 80L60 52L72 80L84 52" stroke="#FDE047" stroke-width="3.5" stroke-linejoin="round"/>
            </svg>
            """,
        ["tree"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="52" y="68" width="16" height="32" rx="4" fill="#92400E"/>
                <circle cx="60" cy="48" r="28" fill="#16A34A" stroke="#14532D" stroke-width="3"/>
                <circle cx="42" cy="56" r="18" fill="#22C55E"/>
                <circle cx="78" cy="56" r="18" fill="#22C55E"/>
            </svg>
            """,
        ["ship"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="60,20 60,64 90,64" fill="#FDE047" stroke="#CA8A04" stroke-width="3" stroke-linejoin="round"/>
                <polygon points="56,28 56,64 30,64" fill="#FFFFFF" stroke="#475569" stroke-width="3" stroke-linejoin="round"/>
                <path d="M22 68H98L86 90H34L22 68Z" fill="#2563EB" stroke="#1E3A8A" stroke-width="3" stroke-linejoin="round"/>
            </svg>
            """,
        ["fish"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="84,60 104,42 104,78" fill="#06B6D4" stroke="#155E75" stroke-width="3" stroke-linejoin="round"/>
                <ellipse cx="56" cy="60" rx="32" ry="22" fill="#38BDF8" stroke="#0369A1" stroke-width="3"/>
                <circle cx="40" cy="54" r="4" fill="#1E1B4B"/>
                <path d="M54 46Q62 60 54 74" stroke="#BAE6FD" stroke-width="3" stroke-linecap="round"/>
            </svg>
            """,
        ["cake"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="26" y="54" width="68" height="38" rx="10" fill="#F9A8D4" stroke="#9D174D" stroke-width="3"/>
                <path d="M26 62Q38 72 50 62Q62 72 74 62Q86 72 94 62" stroke="#FFF1F2" stroke-width="6" stroke-linecap="round"/>
                <rect x="56" y="34" width="8" height="20" rx="3" fill="#FDE047" stroke="#CA8A04" stroke-width="2"/>
                <circle cx="60" cy="26" r="5" fill="#F97316"/>
            </svg>
            """,
        ["bike"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="34" cy="76" r="18" fill="none" stroke="#1E293B" stroke-width="5"/>
                <circle cx="86" cy="76" r="18" fill="none" stroke="#1E293B" stroke-width="5"/>
                <path d="M34 76L54 48H74L86 76M54 48L62 76H34" stroke="#2563EB" stroke-width="5" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M70 42H80" stroke="#475569" stroke-width="5" stroke-linecap="round"/>
            </svg>
            """,
        ["kite"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="60,16 92,48 60,82 28,48" fill="#FACC15" stroke="#CA8A04" stroke-width="3" stroke-linejoin="round"/>
                <path d="M60 16V82M28 48H92" stroke="#92400E" stroke-width="2.5"/>
                <path d="M60 82Q70 94 56 104" stroke="#EF4444" stroke-width="3.5" stroke-linecap="round"/>
            </svg>
            """,
        ["home"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="60,20 100,54 20,54" fill="#0D9488" stroke="#134E4A" stroke-width="3" stroke-linejoin="round"/>
                <rect x="30" y="54" width="60" height="42" rx="6" fill="#CCFBF1" stroke="#134E4A" stroke-width="3"/>
                <rect x="52" y="68" width="16" height="28" rx="3" fill="#0F766E"/>
            </svg>
            """,
        ["ant"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="36" cy="70" rx="14" ry="12" fill="#B91C1C"/>
                <circle cx="58" cy="64" r="10" fill="#DC2626"/>
                <circle cx="78" cy="54" r="12" fill="#EF4444"/>
                <circle cx="82" cy="51" r="3" fill="#fff"/>
                <path d="M76 42L84 26M84 44L96 32" stroke="#7F1D1D" stroke-width="3" stroke-linecap="round"/>
                <path d="M40 80L30 96M58 74L54 96M64 74L72 96" stroke="#7F1D1D" stroke-width="3.5" stroke-linecap="round"/>
            </svg>
            """,
        ["top"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="56" y="18" width="8" height="16" rx="4" fill="#1E293B"/>
                <path d="M24 52C24 40 96 40 96 52L60 98L24 52Z" fill="#3B82F6" stroke="#1D4ED8" stroke-width="3" stroke-linejoin="round"/>
                <path d="M32 58H88" stroke="#FDE047" stroke-width="5" stroke-linecap="round"/>
            </svg>
            """,
        ["igloo"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 86C18 48 102 48 102 86Z" fill="#E0F2FE" stroke="#0284C7" stroke-width="3.5"/>
                <path d="M48 86V68C48 60 72 60 72 68V86Z" fill="#0369A1"/>
                <path d="M26 66H94M34 54H86" stroke="#7DD3FC" stroke-width="2.5"/>
            </svg>
            """,
        ["egg"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M60 20C40 20 28 54 28 74C28 92 42 102 60 102C78 102 92 92 92 74C92 54 80 20 60 20Z" fill="#FEF3C7" stroke="#D97706" stroke-width="3.5"/>
                <ellipse cx="48" cy="52" rx="7" ry="12" transform="rotate(-20 48 52)" fill="#fff" opacity="0.7"/>
            </svg>
            """,
        ["lion"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="60" r="34" fill="#D97706"/>
                <circle cx="60" cy="62" r="22" fill="#FDE68A"/>
                <circle cx="52" cy="57" r="3.5" fill="#1E293B"/>
                <circle cx="68" cy="57" r="3.5" fill="#1E293B"/>
                <polygon points="60,62 55,68 65,68" fill="#92400E"/>
            </svg>
            """,
        ["goat"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M42 36Q34 20 24 26M78 36Q86 20 96 26" stroke="#92400E" stroke-width="5" stroke-linecap="round"/>
                <ellipse cx="60" cy="64" rx="24" ry="26" fill="#F1F5F9" stroke="#475569" stroke-width="3"/>
                <circle cx="52" cy="58" r="3.5" fill="#1E293B"/>
                <circle cx="68" cy="58" r="3.5" fill="#1E293B"/>
                <path d="M56 88L60 102L64 88" fill="#CBD5E1"/>
            </svg>
            """,
        ["octopus"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="48" r="24" fill="#8B5CF6"/>
                <circle cx="52" cy="46" r="4" fill="#fff"/>
                <circle cx="68" cy="46" r="4" fill="#fff"/>
                <path d="M38 68Q24 88 32 98M50 70Q44 92 48 100M70 70Q76 92 72 100M82 68Q96 88 88 98" stroke="#7C3AED" stroke-width="6" stroke-linecap="round"/>
            </svg>
            """,
        ["umbrella"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M20 60C20 34 100 34 100 60Z" fill="#EC4899" stroke="#BE185D" stroke-width="3"/>
                <path d="M60 36V88C60 96 72 96 72 88" stroke="#475569" stroke-width="4.5" stroke-linecap="round"/>
            </svg>
            """,
        ["rain"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M34 62H88C96 62 100 54 96 46C92 38 82 38 78 40C74 28 54 26 46 38C34 38 26 46 28 56C29 60 31 62 34 62Z" fill="#CBD5E1" stroke="#64748B" stroke-width="3"/>
                <path d="M40 74L34 90M60 74L54 90M80 74L74 90" stroke="#0284C7" stroke-width="4.5" stroke-linecap="round"/>
            </svg>
            """,
        ["moon"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M74 24C52 24 36 42 36 62C36 82 54 98 76 96C58 90 52 72 56 54C60 38 66 30 74 24Z" fill="#FACC15" stroke="#CA8A04" stroke-width="3"/>
            </svg>
            """,
        ["night"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="18" y="20" width="84" height="80" rx="20" fill="#1E1B4B"/>
                <path d="M72 34C58 34 48 46 48 60C48 74 60 84 74 82C62 78 58 64 62 52C64 42 68 38 72 34Z" fill="#FDE047"/>
                <circle cx="38" cy="42" r="3" fill="#fff"/>
                <circle cx="86" cy="68" r="2.5" fill="#fff"/>
            </svg>
            """,
        ["light"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="52" r="22" fill="#FEF08A" stroke="#EAB308" stroke-width="3.5"/>
                <rect x="50" y="74" width="20" height="14" rx="4" fill="#94A3B8"/>
                <path d="M60 16V24M24 52H32M88 52H96M34 26L40 32M86 26L80 32" stroke="#F59E0B" stroke-width="4" stroke-linecap="round"/>
            </svg>
            """,
        ["ear"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M46 34C66 22 86 36 82 58C79 74 68 88 56 90C46 92 40 84 44 74" fill="#FED7AA" stroke="#C2410C" stroke-width="4" stroke-linecap="round"/>
                <path d="M56 46C66 44 70 54 64 64" stroke="#C2410C" stroke-width="3" stroke-linecap="round"/>
            </svg>
            """,
        ["chair"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="36" y="24" width="48" height="34" rx="6" fill="#F59E0B" stroke="#92400E" stroke-width="3"/>
                <rect x="32" y="58" width="56" height="12" rx="4" fill="#D97706" stroke="#92400E" stroke-width="3"/>
                <path d="M38 70V98M82 70V98" stroke="#78350F" stroke-width="6" stroke-linecap="round"/>
            </svg>
            """,
        ["lamb"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="52" cy="58" r="22" fill="#F8FAFC" stroke="#CBD5E1" stroke-width="3"/>
                <circle cx="70" cy="58" r="18" fill="#F8FAFC" stroke="#CBD5E1" stroke-width="3"/>
                <circle cx="78" cy="54" r="14" fill="#1E293B"/>
                <circle cx="82" cy="51" r="2.5" fill="#fff"/>
                <path d="M44 78V96M72 76V96" stroke="#334155" stroke-width="5" stroke-linecap="round"/>
            </svg>
            """,
        ["comb"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect x="22" y="38" width="76" height="16" rx="6" fill="#2563EB"/>
                <path d="M28 54V82M38 54V82M48 54V82M58 54V82M68 54V82M78 54V82M88 54V82" stroke="#2563EB" stroke-width="4.5" stroke-linecap="round"/>
            </svg>
            """,
        ["knee"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M42 22L68 58L46 98" stroke="#FDBA74" stroke-width="24" stroke-linecap="round" stroke-linejoin="round"/>
                <circle cx="68" cy="58" r="10" fill="#FB923C" opacity="0.45"/>
            </svg>
            """,
        ["bone"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M32 52H88V68H32Z" fill="#F8FAFC"/>
                <circle cx="28" cy="48" r="10" fill="#F8FAFC" stroke="#94A3B8" stroke-width="2.5"/>
                <circle cx="28" cy="70" r="10" fill="#F8FAFC" stroke="#94A3B8" stroke-width="2.5"/>
                <circle cx="92" cy="48" r="10" fill="#F8FAFC" stroke="#94A3B8" stroke-width="2.5"/>
                <circle cx="92" cy="70" r="10" fill="#F8FAFC" stroke="#94A3B8" stroke-width="2.5"/>
            </svg>
            """,
        ["cube"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="60,22 94,40 60,58 26,40" fill="#A78BFA" stroke="#5B21B6" stroke-width="3"/>
                <polygon points="26,40 60,58 60,96 26,78" fill="#8B5CF6" stroke="#5B21B6" stroke-width="3"/>
                <polygon points="94,40 60,58 60,96 94,78" fill="#7C3AED" stroke="#5B21B6" stroke-width="3"/>
            </svg>
            """,
        ["cup"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M34 40H80V74C80 86 70 94 57 94C44 94 34 86 34 74V40Z" fill="#F472B6" stroke="#BE185D" stroke-width="3"/>
                <path d="M80 48H90C96 48 96 68 90 68H80" stroke="#BE185D" stroke-width="4" fill="none"/>
            </svg>
            """,
        ["ring"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="68" r="26" fill="none" stroke="#EAB308" stroke-width="8"/>
                <polygon points="60,22 72,36 60,46 48,36" fill="#38BDF8" stroke="#0284C7" stroke-width="2.5"/>
            </svg>
            """,
        ["fork"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M46 22V48M60 22V48M74 22V48M46 48C46 58 74 58 74 48M60 56V98" stroke="#64748B" stroke-width="5.5" stroke-linecap="round"/>
            </svg>
            """,
        ["queen"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <polygon points="24,80 28,36 48,56 60,26 72,56 92,36 96,80" fill="#FACC15" stroke="#B45309" stroke-width="3.5" stroke-linejoin="round"/>
                <circle cx="60" cy="24" r="5" fill="#EC4899"/>
            </svg>
            """,
        ["cloud"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M32 78H88C98 78 104 70 100 60C96 52 86 50 82 52C78 36 54 34 46 48C32 48 22 58 24 68C25 74 28 78 32 78Z" fill="#E0F2FE" stroke="#0284C7" stroke-width="3.5"/>
            </svg>
            """,
        ["coin"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="60" cy="60" r="34" fill="#FACC15" stroke="#CA8A04" stroke-width="4"/>
                <circle cx="60" cy="60" r="24" fill="none" stroke="#FEF08A" stroke-width="3"/>
                <text x="60" y="70" text-anchor="middle" font-family="Nunito, sans-serif" font-size="30" font-weight="900" fill="#92400E">$</text>
            </svg>
            """,
        ["book"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M24 32C42 28 56 34 60 40C64 34 78 28 96 32V86C78 82 64 88 60 94C56 88 42 82 24 86V32Z" fill="#DBEAFE" stroke="#2563EB" stroke-width="3.5" stroke-linejoin="round"/>
                <path d="M60 40V94" stroke="#2563EB" stroke-width="3"/>
            </svg>
            """,
        ["lake"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <ellipse cx="60" cy="70" rx="42" ry="20" fill="#38BDF8" stroke="#0284C7" stroke-width="3"/>
                <path d="M36 68Q48 62 60 68T84 68" stroke="#E0F2FE" stroke-width="3" stroke-linecap="round"/>
            </svg>
            """,
        ["rope"] = """
            <svg viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M26 78C44 36 76 84 94 42" stroke="#D97706" stroke-width="8" stroke-linecap="round"/>
                <path d="M26 78C44 36 76 84 94 42" stroke="#FEF3C7" stroke-width="3" stroke-dasharray="6 8" stroke-linecap="round"/>
            </svg>
            """
    };
}
