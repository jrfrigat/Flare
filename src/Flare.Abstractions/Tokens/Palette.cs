namespace Flare.Abstractions.Tokens;

/// <summary>
/// A named, reusable set of colors with a light, dark, and optional high-contrast <see cref="ColorScheme"/>.
/// Palettes are decoupled from themes: they live in a registry, can be added by any
/// developer, and are structurally universal (the same role contract applies to every
/// theme). A theme references a default palette by id; the active palette is chosen
/// independently and may be switched at runtime.
/// </summary>
public sealed record Palette
{
    /// <summary>Stable unique id; also the CSS class suffix (<c>flare-palette-{Id}</c>).</summary>
    public required string Id { get; init; }
    /// <summary>Human-readable name for pickers.</summary>
    public required string Name { get; init; }

    /// <summary>
    /// Origin/family this palette ships with, for grouping in pickers (e.g. "Material Design 3",
    /// "Fluent UI 2"). Purely informational -- a palette is structurally universal and can be
    /// applied to any theme. Null/empty groups under a generic heading.
    /// </summary>
    public string? Source { get; init; }
    /// <summary>Colors used when the active mode is light.</summary>
    public required ColorScheme Light { get; init; }
    /// <summary>Colors used when the active mode is dark.</summary>
    public required ColorScheme Dark { get; init; }

    /// <summary>
    /// Optional high-contrast color scheme for WCAG AAA compliance. When null, the
    /// <see cref="ThemeMode.HighContrast"/> mode falls back to the dark scheme with
    /// enhanced contrast overrides applied at the CSS level.
    /// </summary>
    public ColorScheme? HighContrast { get; init; }

    /// <summary>
    /// Optional href of a static CSS asset shipping this palette (build-time delivery).
    /// Null for palettes generated/registered at runtime (delivered via injected style).
    /// </summary>
    public string? StyleAsset { get; init; }

    /// <summary>Returns the scheme for the given mode (Auto resolves to light here; the
    /// service resolves Auto against the OS before calling).</summary>
    public ColorScheme For(ThemeMode mode) => mode switch
    {
        ThemeMode.Dark => Dark,
        ThemeMode.HighContrast => HighContrast ?? Dark,
        _ => Light,
    };
}
