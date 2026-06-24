namespace Flare.Css.Classes;

/// <summary>
/// Root and theme-axis classes applied by <c>FlareThemeProvider</c> and <c>FlareThemeScope</c>
/// to the wrapper element so theme/palette/mode CSS resolves for the subtree.
/// </summary>
public static class Theme
{
    /// <summary>The <c>flare-root</c> CSS class (theme root marker on the wrapper element).</summary>
    public const string Root = "flare-root";
    /// <summary>The <c>flare-rtl</c> CSS class (right-to-left layout).</summary>
    public const string Rtl = "flare-rtl";
    /// <summary>The <c>flare-mode-light</c> CSS class (light color scheme).</summary>
    public const string ModeLight = "flare-mode-light";
    /// <summary>The <c>flare-mode-dark</c> CSS class (dark color scheme).</summary>
    public const string ModeDark = "flare-mode-dark";
    /// <summary>The <c>flare-mode-high-contrast</c> CSS class (high-contrast mode).</summary>
    public const string ModeHighContrast = "flare-mode-high-contrast";

    /// <summary>Prefix for the per-theme class; concatenate with the theme id (e.g. <c>flare-theme-aero</c>).</summary>
    public const string ThemePrefix = "flare-theme-";
    /// <summary>Prefix for the per-palette class; concatenate with the palette id (e.g. <c>flare-palette-violet</c>).</summary>
    public const string PalettePrefix = "flare-palette-";

    /// <summary>Builds the per-theme class for the given theme id (<c>flare-theme-{id}</c>).</summary>
    /// <param name="themeId">The theme id.</param>
    /// <returns>The theme class string.</returns>
    public static string ForTheme(string themeId) => ThemePrefix + themeId;

    /// <summary>Builds the per-palette class for the given palette id (<c>flare-palette-{id}</c>).</summary>
    /// <param name="paletteId">The palette id.</param>
    /// <returns>The palette class string.</returns>
    public static string ForPalette(string paletteId) => PalettePrefix + paletteId;

    /// <summary>Returns the color-mode class for the given state (high contrast wins over dark).</summary>
    /// <param name="isDark">Whether the dark scheme is active.</param>
    /// <param name="isHighContrast">Whether high-contrast mode is active.</param>
    /// <returns>The mode class string.</returns>
    public static string Mode(bool isDark, bool isHighContrast = false) =>
        isHighContrast ? ModeHighContrast : isDark ? ModeDark : ModeLight;
}
