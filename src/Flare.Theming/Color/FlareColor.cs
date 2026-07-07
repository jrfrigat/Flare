using Flare.Abstractions;
using Flare.Components.Security;
using Flare.Theming;
using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Dynamic-color scheme variant - how the source (seed) color is expanded into tonal roles.
/// </summary>
public enum DynamicVariant
{
    /// <summary>Balanced palette close to the source hue (the default).</summary>
    TonalSpot,
    /// <summary>More saturated, punchier palette.</summary>
    Vibrant,
    /// <summary>Hue-shifted, playful palette.</summary>
    Expressive,
    /// <summary>Low-chroma, muted palette.</summary>
    Neutral,
    /// <summary>Greyscale palette derived from the source's tone only.</summary>
    Monochrome,
}

/// <summary>Semantic color roles shared across Flare components.</summary>
public enum FlareColorRole
{
    /// <summary>The component's theme default color (no explicit role).</summary>
    Default,
    /// <summary>Primary brand color - the main accent.</summary>
    Primary,
    /// <summary>Secondary accent color.</summary>
    Secondary,
    /// <summary>Tertiary accent color.</summary>
    Tertiary,
    /// <summary>Success / positive state color.</summary>
    Success,
    /// <summary>Warning / caution state color.</summary>
    Warning,
    /// <summary>Error / danger state color.</summary>
    Error,
    /// <summary>Info / neutral-informative state color.</summary>
    Info,
    /// <summary>On-surface color - default high-emphasis text/icon color on surfaces.</summary>
    OnSurface,
    /// <summary>On-surface-variant color - muted/secondary text and icons on surfaces.</summary>
    OnSurfaceVariant,
    /// <summary>On-surface-variant-2 color - fainter tertiary text (footnotes, counts, captions).</summary>
    OnSurfaceVariant2,
}

/// <summary>
/// Unified color value for Flare components. Holds either a semantic <see cref="FlareColorRole"/>
/// (rendered via a shared CSS class - the fast, cached path) or an arbitrary CSS color
/// (inlined as local color tokens). Create roles via the static presets
/// (<c>FlareColor.Primary</c>) or a custom color via <see cref="Custom"/> / an implicit string
/// (<c>"#FF0000"</c>, <c>Colors.Primary</c>).
/// </summary>
public readonly record struct FlareColor
{
    private readonly FlareColorRole _role;
    private readonly string? _custom;
    // Dynamic-color path only: precomputed paired tones (each may be a light-dark() pair).
    // Null for plain custom colors, where these are derived on demand via FlareColorResolver.
    private readonly string? _on;
    private readonly string? _container;
    private readonly string? _onContainer;

    private FlareColor(FlareColorRole role)
    {
        _role = role;
        _custom = null;
    }

    private FlareColor(string custom)
    {
        _role = FlareColorRole.Default;
        _custom = CssValidator.SanitizeColor(custom);   // unsafe/empty -> null -> behaves as Default
    }

    // Dynamic ctor: values come from our own generator (already safe), so they bypass sanitization
    // (they are light-dark() expressions, which the color sanitizer would otherwise reject).
    private FlareColor(string main, string on, string container, string onContainer)
    {
        _role = FlareColorRole.Default;
        _custom = main;
        _on = on;
        _container = container;
        _onContainer = onContainer;
    }

    /// <summary>The theme default color for the component.</summary>
    public static FlareColor Default => default;
    /// <summary>Primary brand color.</summary>
    public static FlareColor Primary => new(FlareColorRole.Primary);
    /// <summary>Secondary color.</summary>
    public static FlareColor Secondary => new(FlareColorRole.Secondary);
    /// <summary>Tertiary color.</summary>
    public static FlareColor Tertiary => new(FlareColorRole.Tertiary);
    /// <summary>Success (positive) color.</summary>
    public static FlareColor Success => new(FlareColorRole.Success);
    /// <summary>Warning (caution) color.</summary>
    public static FlareColor Warning => new(FlareColorRole.Warning);
    /// <summary>Error (danger) color.</summary>
    public static FlareColor Error => new(FlareColorRole.Error);
    /// <summary>Info (neutral-informative) color.</summary>
    public static FlareColor Info => new(FlareColorRole.Info);
    /// <summary>On-surface (high-emphasis content) color.</summary>
    public static FlareColor OnSurface => new(FlareColorRole.OnSurface);
    /// <summary>On-surface-variant (muted/secondary content) color.</summary>
    public static FlareColor OnSurfaceVariant => new(FlareColorRole.OnSurfaceVariant);
    /// <summary>On-surface-variant-2 (fainter tertiary content) color.</summary>
    public static FlareColor OnSurfaceVariant2 => new(FlareColorRole.OnSurfaceVariant2);

    /// <summary>
    /// Creates a color from an arbitrary CSS value (e.g. <c>#FF0000</c>, <c>rgb(...)</c>) or a Flare
    /// role expression (<c>var(--flare-color-primary)</c>). Role expressions resolve back to the
    /// semantic role so they still use the fast class path.
    /// </summary>
    public static FlareColor Custom(string? cssColor)
    {
        if (string.IsNullOrWhiteSpace(cssColor)) return Default;
        return TryRoleFromVar(cssColor.Trim(), out var role) ? new FlareColor(role) : new FlareColor(cssColor);
    }

    /// <summary>Implicit conversion from a CSS color or role expression string.</summary>
    public static implicit operator FlareColor(string? value) => Custom(value);

    /// <summary>
    /// <b>Dynamic color</b>: derives a harmonized, contrast-safe tonal set (main / on / container /
    /// on-container) from a single source/seed color (e.g. one extracted from an album cover) and applies
    /// it to a component. The four roles are emitted as CSS <c>light-dark()</c> pairs so they adapt to the
    /// active light/dark mode automatically. Unlike <see cref="Custom"/> (a single color with naive
    /// derivations), this produces proper tonal containers and on-colors via the palette generator.
    /// </summary>
    /// <param name="source">Seed color (e.g. <c>#3F51B5</c>). Invalid input falls back to <see cref="Default"/>.</param>
    /// <param name="variant">How the seed is expanded into tones (default <see cref="DynamicVariant.TonalSpot"/>).</param>
    /// <param name="contrast">Contrast level -1..1; positive deepens on-colors for stronger contrast.</param>
    public static FlareColor Dynamic(string? source, DynamicVariant variant = DynamicVariant.TonalSpot, double contrast = 0)
    {
        var seed = CssValidator.SanitizeColor(source ?? string.Empty);
        // Dynamic needs a concrete hex seed (e.g. extracted from an image) to derive tonal roles.
        if (seed is null || !TryParseHex(seed, out _, out _, out _)) return Default;

        // Variant: reshape the seed's chroma/hue before generating the tonal palette.
        var main = variant switch
        {
            DynamicVariant.Vibrant => ColorMath.WithSaturation(seed, Math.Min(1.0, ColorMath.ToHsl(seed).S * 1.4 + 0.25)),
            DynamicVariant.Expressive => ColorMath.WithSaturation(ColorMath.RotateHue(seed, 30), Math.Min(1.0, ColorMath.ToHsl(seed).S * 1.3 + 0.2)),
            DynamicVariant.Neutral => ColorMath.WithSaturation(seed, 0.16),
            DynamicVariant.Monochrome => ColorMath.WithSaturation(seed, 0.0),
            _ => seed,
        };

        var pal = DefaultPaletteGenerator.Instance.Generate("flare-dynamic", "Dynamic", new PaletteSeed(main));
        var c = Math.Clamp(contrast, -1, 1);

        // contrast>0 pushes the on-colors further from their background (darker on light, lighter on dark).
        string onL(string x) => c > 0 ? ColorMath.Darken(x, 0.30 * c) : x;
        string onD(string x) => c > 0 ? ColorMath.Lighten(x, 0.30 * c) : x;
        static string Ld(string light, string dark) => $"light-dark({light}, {dark})";

        return new FlareColor(
            main: Ld(pal.Light.Primary, pal.Dark.Primary),
            on: Ld(onL(pal.Light.OnPrimary), onD(pal.Dark.OnPrimary)),
            container: Ld(pal.Light.PrimaryContainer, pal.Dark.PrimaryContainer),
            onContainer: Ld(onL(pal.Light.OnPrimaryContainer), onD(pal.Dark.OnPrimaryContainer)));
    }

    // Golden angle (360 / phi^2) gives a maximally-spread, repeatable hue sequence.
    private const double GoldenAngle = 137.50776405003785;
    // Seed used when the input has no parseable hex (default / role / non-hex custom).
    private const int _seedR = 0x42, _seedG = 0x85, _seedB = 0xF4;

    /// <summary>
    /// Returns a random custom color as a <c>#RRGGBB</c> hex string. Not theme-aware -
    /// it is an arbitrary color; handy for ad-hoc coloring (random chip/avatar background).
    /// </summary>
    public static FlareColor Random() =>
        new($"#{System.Random.Shared.Next(0x1000000):X6}");

    /// <summary>
    /// Returns the next color after <paramref name="color"/> as a <c>#RRGGBB</c> hex value,
    /// rotating its hue by the golden angle (~137.5deg) while keeping a pleasant saturation and
    /// lightness. Deterministic and not tied to the semantic palette, so repeated calls yield a
    /// well-spread sequence of distinct colors. <see cref="Default"/>, roles, or non-hex custom
    /// values start from a fixed seed color.
    /// </summary>
    public static FlareColor Next(FlareColor color)
    {
        if (!TryParseHex(color._custom, out var r, out var g, out var b))
            (r, g, b) = (_seedR, _seedG, _seedB);

        RgbToHsl(r, g, b, out var h, out var s, out var l);
        h = (h + GoldenAngle) % 360;
        if (s < 0.20) s = 0.65;                 // avoid getting stuck on greys
        if (l < 0.20 || l > 0.80) l = 0.55;     // keep it readable
        HslToRgb(h, s, l, out var nr, out var ng, out var nb);
        return new FlareColor($"#{nr:X2}{ng:X2}{nb:X2}");
    }

    /// <summary>Instance shortcut for <see cref="Next(FlareColor)"/>: <c>color = color.Next();</c>.</summary>
    public FlareColor Next() => Next(this);

    private static bool TryParseHex(string? hex, out int r, out int g, out int b)
    {
        r = g = b = 0;
        if (string.IsNullOrWhiteSpace(hex)) return false;
        var h = hex.Trim().TrimStart('#');
        if (h.Length == 3) h = $"{h[0]}{h[0]}{h[1]}{h[1]}{h[2]}{h[2]}";
        if ((h.Length == 6 || h.Length == 8) &&
            int.TryParse(h.AsSpan(0, 6), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
        {
            r = (rgb >> 16) & 0xFF; g = (rgb >> 8) & 0xFF; b = rgb & 0xFF;
            return true;
        }
        return false;
    }

    private static void RgbToHsl(int r, int g, int b, out double h, out double s, out double l)
    {
        double rf = r / 255.0, gf = g / 255.0, bf = b / 255.0;
        double max = Math.Max(rf, Math.Max(gf, bf)), min = Math.Min(rf, Math.Min(gf, bf)), d = max - min;
        l = (max + min) / 2;
        if (d == 0) { h = 0; s = 0; return; }
        s = d / (1 - Math.Abs(2 * l - 1));
        if (max == rf) h = 60 * (((gf - bf) / d) % 6);
        else if (max == gf) h = 60 * ((bf - rf) / d + 2);
        else h = 60 * ((rf - gf) / d + 4);
        if (h < 0) h += 360;
    }

    private static void HslToRgb(double h, double s, double l, out int r, out int g, out int b)
    {
        double c = (1 - Math.Abs(2 * l - 1)) * s, x = c * (1 - Math.Abs(h / 60 % 2 - 1)), m = l - c / 2;
        double rf, gf, bf;
        if (h < 60) { rf = c; gf = x; bf = 0; }
        else if (h < 120) { rf = x; gf = c; bf = 0; }
        else if (h < 180) { rf = 0; gf = c; bf = x; }
        else if (h < 240) { rf = 0; gf = x; bf = c; }
        else if (h < 300) { rf = x; gf = 0; bf = c; }
        else { rf = c; gf = 0; bf = x; }
        r = (int)Math.Round((rf + m) * 255);
        g = (int)Math.Round((gf + m) * 255);
        b = (int)Math.Round((bf + m) * 255);
    }

    /// <summary>True for the theme-default (no explicit color).</summary>
    public bool IsDefault => _custom is null && _role == FlareColorRole.Default;
    /// <summary>True when this is a semantic role.</summary>
    public bool IsRole => _custom is null && _role != FlareColorRole.Default;
    /// <summary>True when this is an arbitrary (sanitized) custom color.</summary>
    public bool IsCustom => _custom is not null;

    /// <summary>The role name ("primary".."info") for a role, otherwise null.</summary>
    public string? RoleName => IsRole ? RoleToString(_role) : null;
    /// <summary>The sanitized custom CSS color, or null when not custom.</summary>
    public string? Value => _custom;
    /// <summary>The shared role CSS class ("flare-color-primary"), or null for default/custom.</summary>
    public string? CssClass => RoleName is { } r ? $"flare-color-{r}" : null;

    /// <summary>True for a dynamic-color value (precomputed tonal set), a subset of <see cref="IsCustom"/>.</summary>
    public bool IsDynamic => _on is not null;

    // ---- Centralized inline-token emission for the custom/dynamic path -------------------------
    // Components apply the subset of --fc-* tokens they consume. For a plain custom color the paired
    // tones are derived via FlareColorResolver; for a dynamic color they are the precomputed light-dark
    // pairs. Role/Default colors return null (they use the shared flare-color-* class instead).

    /// <summary>Inline <c>--fc-main</c> token (custom/dynamic), or null for role/default.</summary>
    public string? StyleMain() => _custom is null ? null : $"{Css.Tokens.LocalColor.Main}:{_custom};";

    /// <summary>Inline <c>--fc-main</c> + <c>--fc-on</c> tokens, or null for role/default.</summary>
    public string? StyleMainOn(string? onOverride = null) =>
        _custom is null ? null : $"{Css.Tokens.LocalColor.Main}:{_custom};{Css.Tokens.LocalColor.On}:{_on ?? FlareColorResolver.OnColor(_custom, onOverride)};";

    /// <summary>Inline <c>--fc-container</c> + <c>--fc-on-container</c> tokens, or null for role/default.</summary>
    public string? StyleContainer() =>
        _custom is null ? null : $"{Css.Tokens.LocalColor.Container}:{_container ?? FlareColorResolver.Container(_custom)};{Css.Tokens.LocalColor.OnContainer}:{_onContainer ?? FlareColorResolver.OnContainer(_custom)};";

    /// <summary>Inline full set (<c>--fc-main/on/container/on-container</c>), or null for role/default.</summary>
    public string? StyleFull(string? onOverride = null) =>
        _custom is null ? null : StyleMainOn(onOverride) + StyleContainer();

    /// <summary>
    /// Inline a <b>solid</b> tonal pair onto <c>--fc-container</c>/<c>--fc-on-container</c> (the accent
    /// itself as the container fill with a contrasting on-color), for tonal surfaces like Avatar. Null
    /// for role/default. Dynamic colors use the harmonized primary/on tones.
    /// </summary>
    public string? StyleContainerSolid() =>
        _custom is null ? null : $"{Css.Tokens.LocalColor.Container}:{_custom};{Css.Tokens.LocalColor.OnContainer}:{_on ?? FlareColorResolver.OnColor(_custom)};";

    private static string RoleToString(FlareColorRole role) => role switch
    {
        FlareColorRole.Primary => "primary",
        FlareColorRole.Secondary => "secondary",
        FlareColorRole.Tertiary => "tertiary",
        FlareColorRole.Success => "success",
        FlareColorRole.Warning => "warning",
        FlareColorRole.Error => "error",
        FlareColorRole.Info => "info",
        FlareColorRole.OnSurface => "on-surface",
        FlareColorRole.OnSurfaceVariant => "on-surface-variant",
        FlareColorRole.OnSurfaceVariant2 => "on-surface-variant2",
        _ => "primary",
    };

    private static bool TryRoleFromVar(string value, out FlareColorRole role)
    {
        role = FlareColorRole.Default;
        const string prefix = "var(--flare-color-";
        if (!value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith(")", StringComparison.Ordinal))
            return false;
        var inner = value[prefix.Length..^1].Trim();
        role = inner switch
        {
            "primary" => FlareColorRole.Primary,
            "secondary" => FlareColorRole.Secondary,
            "tertiary" => FlareColorRole.Tertiary,
            "success" => FlareColorRole.Success,
            "warning" => FlareColorRole.Warning,
            "error" => FlareColorRole.Error,
            "info" => FlareColorRole.Info,
            "on-surface" => FlareColorRole.OnSurface,
            "on-surface-variant" => FlareColorRole.OnSurfaceVariant,
            "on-surface-variant2" => FlareColorRole.OnSurfaceVariant2,
            _ => FlareColorRole.Default,
        };
        return role != FlareColorRole.Default;
    }
}
