using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Core.Abstractions;

/// <summary>
/// Manages the three theme axes -- design-system theme, color palette and light/dark mode --
/// and applies their composition to the document. Themes and palettes are registered
/// independently; any axis can be switched at runtime.
/// </summary>
public interface IThemeService
{
    /// <summary>The active design-system theme.</summary>
    ITheme CurrentTheme { get; }
    /// <summary>The active color palette.</summary>
    Palette CurrentPalette { get; }
    /// <summary>The selected mode (Light/Dark/Auto).</summary>
    ThemeMode Mode { get; }
    /// <summary>The effective dark state (resolves <see cref="ThemeMode.Auto"/> against the OS preference).</summary>
    bool IsDark { get; }
    /// <summary>Whether high-contrast mode is active.</summary>
    bool IsHighContrast { get; }
    /// <summary>Whether RTL (Right-to-Left) layout is enabled.</summary>
    bool IsRtl { get; }
    /// <summary>How theme CSS is delivered (class-toggle static CSS vs full var injection).</summary>
    ThemeDelivery Delivery { get; }

    /// <summary>All registered themes.</summary>
    IReadOnlyList<ITheme> Themes { get; }
    /// <summary>All registered palettes.</summary>
    IReadOnlyList<Palette> Palettes { get; }

    /// <summary>Raised after any axis changes. Subscribers should call StateHasChanged.</summary>
    event Func<Task> OnThemeChanged;

    /// <summary>Registers a design-system theme so it can be activated by id.</summary>
    void RegisterTheme(ITheme theme);
    /// <summary>Registers a color palette so it can be activated by id.</summary>
    void RegisterPalette(Palette palette);

    /// <summary>Switches the active design-system theme by id and re-applies it.</summary>
    Task SetThemeAsync(string themeId);
    /// <summary>Switches the active color palette by id and re-applies it.</summary>
    Task SetPaletteAsync(string paletteId);
    /// <summary>Switches the light/dark/auto mode and re-applies it.</summary>
    Task SetModeAsync(ThemeMode mode);
    /// <summary>Enables or disables right-to-left layout.</summary>
    Task SetRtlAsync(bool isRtl);

    /// <summary>Feeds the current OS <c>prefers-color-scheme</c> so <see cref="ThemeMode.Auto"/> can resolve.</summary>
    Task SetSystemDarkAsync(bool isDark);

    /// <summary>Ensures the static class-scoped stylesheet is present (ClassToggle strategy; no-op otherwise).</summary>
    Task EnsureStaticCssAsync();

    /// <summary>
    /// Ensures the class-scoped CSS for a specific theme/palette is included in the static
    /// stylesheet even when it is not the active selection. <c>FlareThemeScope</c>
    /// calls this so a subtree can switch to any registered theme/palette without the cost of
    /// emitting every registered combination up front. No-op unless <see cref="Delivery"/> is
    /// <see cref="ThemeDelivery.ClassToggle"/> or the id is unknown. Pass <c>null</c> to skip an axis.
    /// </summary>
    Task RequireThemeAssetsAsync(string? themeId, string? paletteId);

    /// <summary>
    /// Generates a palette from a seed using the current theme's generator (MD3 tonal /
    /// Fluent ramp) or the core default. Does not register or apply it.
    /// </summary>
    Palette GeneratePalette(string id, string name, PaletteSeed seed, string? source = null);

    /// <summary>
    /// Typed override of the color axis. The mutator receives the <see cref="ColorScheme"/>
    /// effective for the active mode; only the roles it changes are pushed into the custom-token
    /// layer. Example: <c>CustomizeColors(c =&gt; c with { Primary = "#1C6EA4" })</c>.
    /// </summary>
    void CustomizeColors(Func<ColorScheme, ColorScheme> mutate);

    /// <summary>
    /// Typed override of the design axis. The mutator receives the active theme's
    /// <see cref="DesignTokens"/>; only the tokens it changes are pushed into the custom-token
    /// layer. Example: <c>CustomizeDesign(d =&gt; d with { Button = d.Button with { GapMd = "8px" } })</c>.
    /// </summary>
    void CustomizeDesign(Func<DesignTokens, DesignTokens> mutate);

    /// <summary>Overrides a single CSS var at runtime (e.g. "--flare-color-primary").</summary>
    void SetCustomToken(string tokenName, string value);
    /// <summary>Overrides multiple CSS vars at runtime.</summary>
    void SetCustomTokens(IReadOnlyDictionary<string, string> tokens);
    /// <summary>Removes a previously set custom token override.</summary>
    void ClearCustomToken(string tokenName);
    /// <summary>Removes all custom token overrides.</summary>
    void ClearAllCustomTokens();
    /// <summary>Gets all currently active custom token overrides.</summary>
    IReadOnlyDictionary<string, string> GetCustomTokens();
}
