using Flare.Abstractions;

namespace Flare.Abstractions.Tokens;

/// <summary>
/// Immutable snapshot of the current theme state. Cascaded via [CascadingParameter] to
/// trigger automatic re-renders when the theme changes, replacing the per-component
/// OnThemeChanged subscription pattern for better performance.
/// </summary>
public sealed record ThemeSnapshot
{
    /// <summary>The active design-system theme.</summary>
    public required ITheme Theme { get; init; }

    /// <summary>The active color palette.</summary>
    public required Palette Palette { get; init; }

    /// <summary>The effective dark state.</summary>
    public required bool IsDark { get; init; }

    /// <summary>Whether high-contrast mode is active.</summary>
    public bool IsHighContrast { get; init; }

    /// <summary>Whether RTL (Right-to-Left) layout is enabled.</summary>
    public bool IsRtl { get; init; }

    /// <summary>The delivery mode.</summary>
    public required ThemeDelivery Delivery { get; init; }

    /// <summary>Version counter - incremented on each theme change for cache busting.</summary>
    public int Version { get; init; }
}
