using Flare.Abstractions.Tokens;

namespace Flare.Abstractions;

/// <summary>
/// A theme = a design system (non-color <see cref="DesignTokens"/>) plus a default palette
/// reference and its static style assets. Colors live in <see cref="Palette"/>s, which are
/// registered separately and chosen independently; light/dark is a <see cref="ThemeMode"/>,
/// not a separate theme.
/// </summary>
public interface ITheme
{
    /// <summary>Stable unique id; also the CSS class suffix (<c>flare-theme-{Id}</c>).</summary>
    string Id { get; }
    /// <summary>Human-readable name for pickers.</summary>
    string DisplayName { get; }
    /// <summary>Mode-agnostic design tokens (typography, shape, motion, elevation geometry, components).</summary>
    DesignTokens Design { get; }
    /// <summary>Id of the palette this theme uses out of the box.</summary>
    string DefaultPaletteId { get; }
    /// <summary>Static stylesheets this theme needs (fonts, base reset, generated token CSS).</summary>
    IReadOnlyList<string> StyleAssets { get; }

    /// <summary>
    /// The palettes this theme ships with. When a theme is registered (auto-discovered from a
    /// referenced assembly or added via <c>AddFlareTheme</c>), these palettes are registered
    /// alongside it so the theme travels with its colors. Defaults to empty -- palettes are
    /// structurally universal and may also be registered independently.
    /// </summary>
    IReadOnlyList<Palette> Palettes => [];

    /// <summary>
    /// Optional dark-mode override of <see cref="DesignTokens.Extended"/> for the rare
    /// mode-specific non-color extras (e.g. Fluent focus-stroke colors). Null = no override.
    /// </summary>
    IReadOnlyDictionary<string, string>? ExtendedDarkOverride => null;

    /// <summary>
    /// Optional palette generator that follows this design system's color rules (MD3 tonal,
    /// Fluent ramp). When null, the core default <see cref="IPaletteGenerator"/> implementation is used.
    /// </summary>
    IPaletteGenerator? PaletteGenerator => null;
}
