namespace Flare.Tools.Md3SpecParser.Parsing;

/// <summary>One semantic color role resolved for both themes (maximum depth).</summary>
/// <param name="Role">Semantic role token name, e.g. <c>md.sys.color.primary</c>.</param>
/// <param name="LightTone">Reference tone backing the role in Light (e.g. <c>primary40</c>), if any.</param>
/// <param name="LightHex">Light hex value.</param>
/// <param name="DarkTone">Reference tone backing the role in Dark (e.g. <c>primary80</c>), if any.</param>
/// <param name="DarkHex">Dark hex value.</param>
public sealed record RoleEntry(
    string Role, string? LightTone, string LightHex, string? DarkTone, string DarkHex);

/// <summary>One reference tonal-palette entry (theme-independent).</summary>
/// <param name="Tone">Tone label, e.g. <c>primary40</c>.</param>
/// <param name="Hex">Resolved hex value.</param>
public sealed record ToneEntry(string Tone, string Hex);

/// <summary>A reference tonal palette family with its tones.</summary>
/// <param name="Family">Family name, e.g. <c>primary</c>, <c>neutral-variant</c>.</param>
/// <param name="Tones">Tones ordered ascending.</param>
public sealed record PaletteFamily(string Family, IReadOnlyList<ToneEntry> Tones);

/// <summary>Palette data extracted from one source document.</summary>
public sealed class PaletteSnapshot
{
    /// <summary>Semantic roles actually used by this source's component(s).</summary>
    public List<RoleEntry> UsedRoles { get; init; } = new();

    /// <summary>All semantic roles present in this source's design system.</summary>
    public List<RoleEntry> AllRoles { get; init; } = new();

    /// <summary>All reference tonal palettes present in this source.</summary>
    public List<PaletteFamily> RefPalettes { get; init; } = new();
}
