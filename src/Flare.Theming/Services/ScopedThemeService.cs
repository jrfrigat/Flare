using Flare.Abstractions;
using Flare.Abstractions.Tokens;

namespace Flare.Theming;

/// <summary>
/// A thin <see cref="IThemeService"/> view used by <c>FlareThemeScope</c> to re-theme a subtree.
/// Overrides only the active theme/palette/mode axes; everything else (registries, custom tokens,
/// mutations, palette generation) delegates to the outer service. The scope component keeps the
/// overridden axes in sync and raises <see cref="OnThemeChanged"/> when they (or the outer) change.
/// </summary>
public sealed class ScopedThemeService : IThemeService
{
    private readonly IThemeService _inner;

    /// <summary>Initializes a new <see cref="ScopedThemeService"/> wrapping the given outer service.</summary>
    public ScopedThemeService(IThemeService inner)
    {
        _inner = inner;
        CurrentTheme = inner.CurrentTheme;
        CurrentPalette = inner.CurrentPalette;
        Mode = inner.Mode;
        IsDark = inner.IsDark;
        IsHighContrast = inner.IsHighContrast;
        IsRtl = inner.IsRtl;
    }

    /// <summary>Updates the scoped axes; call after resolving the scope's parameters.</summary>
    public void Set(ITheme theme, Palette palette, ThemeMode mode, bool isDark, bool isHighContrast = false, bool isRtl = false)
    {
        CurrentTheme = theme;
        CurrentPalette = palette;
        Mode = mode;
        IsDark = isDark;
        IsHighContrast = isHighContrast;
        IsRtl = isRtl;
    }

    // -- Overridden axes ----------------------------------------------------------------------
    /// <summary>The active design-system theme.</summary>
    public ITheme CurrentTheme { get; private set; }
    /// <summary>The active color palette.</summary>
    public Palette CurrentPalette { get; private set; }
    /// <summary>The selected mode (Light/Dark/Auto/HighContrast).</summary>
    public ThemeMode Mode { get; private set; }
    /// <summary>The effective dark state (resolves Auto against the OS preference).</summary>
    public bool IsDark { get; private set; }
    /// <summary>Whether high-contrast mode is active.</summary>
    public bool IsHighContrast { get; private set; }
    /// <summary>Whether right-to-left layout is enabled.</summary>
    public bool IsRtl { get; private set; }

    // -- Delegated to the outer service -------------------------------------------------------
    /// <summary>How theme CSS is delivered (class-toggle static CSS vs CSS-variable injection).</summary>
    public ThemeDelivery Delivery => _inner.Delivery;
    /// <summary>All registered themes.</summary>
    public IReadOnlyList<ITheme> Themes => _inner.Themes;
    /// <summary>All registered color palettes.</summary>
    public IReadOnlyList<Palette> Palettes => _inner.Palettes;

    /// <summary>Raised after any theme axis (theme, palette, mode or RTL) changes.</summary>
    public event Func<Task> OnThemeChanged = () => Task.CompletedTask;

    /// <summary>Notifies scoped subscribers (called by the scope component when its axes change).</summary>
    public Task RaiseAsync() => OnThemeChanged();

    /// <summary>Registers a design-system theme so it can be activated by id.</summary>
    public void RegisterTheme(ITheme theme) => _inner.RegisterTheme(theme);
    /// <summary>Registers a color palette so it can be activated by id.</summary>
    public void RegisterPalette(Palette palette) => _inner.RegisterPalette(palette);
    /// <summary>Switches the active design-system theme by id and re-applies it.</summary>
    public Task SetThemeAsync(string themeId) => _inner.SetThemeAsync(themeId);
    /// <summary>Switches the active color palette by id and re-applies it.</summary>
    public Task SetPaletteAsync(string paletteId) => _inner.SetPaletteAsync(paletteId);
    /// <summary>Switches the light/dark/auto mode and re-applies it.</summary>
    public Task SetModeAsync(ThemeMode mode) => _inner.SetModeAsync(mode);
    /// <summary>Enables or disables right-to-left layout.</summary>
    public Task SetRtlAsync(bool isRtl) => _inner.SetRtlAsync(isRtl);
    /// <summary>Feeds the current OS prefers-color-scheme so Auto mode can resolve.</summary>
    public Task SetSystemDarkAsync(bool isDark) => _inner.SetSystemDarkAsync(isDark);
    /// <summary>Ensures the class-scoped static stylesheet is present (ClassToggle delivery; no-op otherwise).</summary>
    public Task EnsureStaticCssAsync() => _inner.EnsureStaticCssAsync();
    /// <summary>Ensures a specific theme/palette's class-scoped CSS is emitted even when not active.</summary>
    public Task RequireThemeAssetsAsync(string? themeId, string? paletteId) => _inner.RequireThemeAssetsAsync(themeId, paletteId);
    /// <summary>Generates a palette from a seed using the active theme's generator. Does not register or apply it.</summary>
    public Palette GeneratePalette(string id, string name, PaletteSeed seed, string? source = null) =>
        _inner.GeneratePalette(id, name, seed, source);
    /// <summary>Pushes a typed override of the color axis into the custom-token layer.</summary>
    public void CustomizeColors(Func<ColorScheme, ColorScheme> mutate) => _inner.CustomizeColors(mutate);
    /// <summary>Pushes a typed override of the design axis into the custom-token layer.</summary>
    public void CustomizeDesign(Func<DesignTokens, DesignTokens> mutate) => _inner.CustomizeDesign(mutate);
    /// <summary>Overrides a single CSS variable at runtime.</summary>
    public void SetCustomToken(string tokenName, string value) => _inner.SetCustomToken(tokenName, value);
    /// <summary>Overrides multiple CSS variables at runtime.</summary>
    public void SetCustomTokens(IReadOnlyDictionary<string, string> tokens) => _inner.SetCustomTokens(tokens);
    /// <summary>Removes a previously set custom token override.</summary>
    public void ClearCustomToken(string tokenName) => _inner.ClearCustomToken(tokenName);
    /// <summary>Removes all custom token overrides.</summary>
    public void ClearAllCustomTokens() => _inner.ClearAllCustomTokens();
    /// <summary>Returns all currently active custom token overrides.</summary>
    public IReadOnlyDictionary<string, string> GetCustomTokens() => _inner.GetCustomTokens();
}
