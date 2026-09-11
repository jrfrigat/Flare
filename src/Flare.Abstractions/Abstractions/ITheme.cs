using Flare.Abstractions.Tokens;

namespace Flare.Abstractions;

/// <summary>
/// A theme = a design system (non-color <see cref="DesignTokens"/>) plus a default palette
/// reference and its static assets. Colors live in <see cref="Palette"/>s, which are
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
    /// Id of the visual family whose stylesheets style this theme, emitted as a second root class
    /// (<c>flare-theme-{StyleFamilyId}</c>) next to <see cref="Id"/>'s own. A theme that ships its
    /// own stylesheets is its own family, which is why this defaults to <see cref="Id"/>. A theme
    /// derived from another one only re-values tokens, so it keeps the base theme's family and the
    /// base theme's CSS keeps applying to it; without this a derived theme would render unstyled,
    /// because every theme stylesheet is scoped to the class its own id produces.
    /// </summary>
    string StyleFamilyId => Id;

    /// <summary>
    /// Optional JavaScript modules that implement theme-owned rendering behavior. Loaded once when
    /// the theme becomes active, including when it becomes active only for the subtree of a
    /// <c>FlareThemeScope</c>. A module must key its behavior off its own public CSS classes rather
    /// than off a theme id, so that it also serves themes derived from this one.
    /// </summary>
    IReadOnlyList<string> ScriptAssets => [];

    /// <summary>
    /// The palettes this theme ships with. When a theme is registered (auto-discovered from a
    /// referenced assembly or added via <c>AddFlareTheme</c>), these palettes are registered
    /// alongside it so the theme travels with its colors. Defaults to empty -- palettes are
    /// structurally universal and may also be registered independently.
    /// </summary>
    IReadOnlyList<Palette> Palettes => [];

    /// <summary>
    /// Optional dark-mode override of <see cref="DesignTokens.Extended"/> for the rare
    /// mode-specific non-color extras (e.g. focus-stroke colors). Null = no override.
    /// </summary>
    IReadOnlyDictionary<string, string>? ExtendedDarkOverride => null;

    /// <summary>
    /// Optional palette generator that follows this design system's own color rules (e.g. a tonal
    /// or ramp-based scheme). When null, the core default <see cref="IPaletteGenerator"/> implementation is used.
    /// </summary>
    IPaletteGenerator? PaletteGenerator => null;
}
