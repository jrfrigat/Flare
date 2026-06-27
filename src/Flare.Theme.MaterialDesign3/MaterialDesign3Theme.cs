using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3Expressive;

namespace Flare.Theme.MaterialDesign3;

/// <summary>
/// Material Design 3 theme (baseline, non-Expressive). Shares the MD3 color system with the
/// Expressive theme but uses the calmer baseline component behavior: classic Label Large button
/// typography at every size (no Expressive type-scale ramp), no dynamic shape morphing, flat
/// (non-wavy) progress indicators, and single-surface menus (no Expressive "island" groups).
/// Light/dark is a mode; colors come from a palette.
/// </summary>
public sealed class MaterialDesign3Theme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>
    public const string ThemeId = "md3";

    /// <inheritdoc />
    public string Id => ThemeId;

    /// <inheritdoc />
    public string DisplayName => "Material Design 3";

    /// <inheritdoc />
    public DesignTokens Design => MaterialDesign3Tokens.Design;

    /// <inheritdoc />
    public string DefaultPaletteId => Md3Palettes.VioletId;

    /// <inheritdoc />
    public IReadOnlyList<Palette> Palettes => Md3Palettes.All;

    /// <inheritdoc />
    public IPaletteGenerator? PaletteGenerator => Md3TonalGenerator.Instance;

    /// <inheritdoc />
    public IReadOnlyList<string> StyleAssets =>
    [
        "https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap",
    ];
}
