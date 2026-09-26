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
    /// The theme this one is built on, or null for a theme that stands on its own. Every theme
    /// stylesheet is scoped to the class its own id produces, so the root of a theme carries one
    /// <c>flare-theme-{id}</c> class per generation of this chain and the CSS of every ancestor keeps
    /// applying. The chain is followed transitively; a theme names only its direct parent.
    /// <see cref="StyleAssets"/> and <see cref="ScriptAssets"/> of a theme with a base are expected
    /// to list the base's assets first, which is what <c>Derive</c> and <c>FlareThemeBuilder.WithBase</c>
    /// do. The value must not change over the theme's lifetime.
    /// </summary>
    ITheme? Base => null;

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
