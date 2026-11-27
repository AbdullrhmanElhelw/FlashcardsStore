namespace FlashcardsStore.PhonicsGame.Data;

/// <summary>Generates LingoDeer-style hollow letter SVGs with numbered green start dots and directional stroke arrows.</summary>
public static class LetterStrokeGuides
{
    private sealed record StrokeSpec(int Sx, int Sy, int Ex, int Ey);

    // Coordinates inside viewBox="0 0 76 150" where baseline is at y=108
    private static readonly Dictionary<char, StrokeSpec> LowerSpecs = new()
    {
        ['a'] = new(56, 62, 56, 106),
        ['b'] = new(22, 26, 22, 106),
        ['c'] = new(56, 56, 26, 94),
        ['d'] = new(56, 26, 56, 106),
        ['e'] = new(24, 78, 56, 78),
        ['f'] = new(46, 28, 36, 106),
        ['g'] = new(56, 58, 56, 128),
        ['h'] = new(22, 26, 22, 106),
        ['i'] = new(38, 56, 38, 106),
        ['j'] = new(44, 56, 38, 128),
        ['k'] = new(24, 26, 24, 106),
        ['l'] = new(38, 26, 38, 106),
        ['m'] = new(16, 58, 16, 106),
        ['n'] = new(22, 58, 22, 106),
        ['o'] = new(38, 54, 20, 94),
        ['p'] = new(22, 58, 22, 130),
        ['q'] = new(54, 58, 54, 130),
        ['r'] = new(24, 58, 24, 106),
        ['s'] = new(52, 58, 26, 100),
        ['t'] = new(36, 32, 36, 104),
        ['u'] = new(22, 58, 22, 104),
        ['v'] = new(18, 58, 38, 106),
        ['w'] = new(14, 58, 26, 106),
        ['x'] = new(20, 58, 56, 106),
        ['y'] = new(20, 58, 38, 126),
        ['z'] = new(20, 58, 56, 58)
    };

    public static string GetLetterSvg(char ch, bool compact = false)
    {
        var lower = char.ToLowerInvariant(ch);
        var spec = LowerSpecs.TryGetValue(lower, out var s) ? s : new StrokeSpec(38, 36, 38, 104);
        var uid = $"arr_{Guid.NewGuid():N}";
        var w = compact ? 58 : 74;

        return $"""
            <svg viewBox="0 0 76 150" width="{w}" height="150" xmlns="http://www.w3.org/2000/svg" style="overflow:visible;">
                <defs>
                    <marker id="{uid}" markerWidth="7" markerHeight="7" refX="5" refY="3.5" orient="auto">
                        <polygon points="0 0, 7 3.5, 0 7" fill="#059669" />
                    </marker>
                </defs>
                <text x="38" y="108" text-anchor="middle"
                      font-family="Nunito, Fredoka, sans-serif"
                      font-size="104" font-weight="800"
                      fill="rgba(124,58,237,0.14)"
                      stroke="rgba(124,58,237,0.45)"
                      stroke-width="2.5">{ch}</text>
                <line x1="{spec.Sx}" y1="{spec.Sy}" x2="{spec.Ex}" y2="{spec.Ey}"
                      stroke="#059669" stroke-width="2.6" stroke-linecap="round"
                      marker-end="url(#{uid})" />
                <circle cx="{spec.Sx}" cy="{spec.Sy}" r="9" fill="#059669" stroke="#ffffff" stroke-width="1.8" />
                <text x="{spec.Sx}" y="{spec.Sy + 3.5}" text-anchor="middle"
                      font-family="Nunito, sans-serif" font-size="10" font-weight="900" fill="#ffffff">1</text>
            </svg>
            """;
    }
}
