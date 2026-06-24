using Flare.Core.Abstractions;
using Flare.Core.Tokens;

namespace Flare.Core.Services;

/// <summary>Default <see cref="Flare.Core.Abstractions.IThemeService"/> managing the active theme, palette and light/dark mode.</summary>
public sealed class ThemeService : IThemeService
{
    private readonly ICssVariableInjector _injector;
    private readonly object _lock = new();
    private readonly List<ITheme> _themes = [];
    private readonly List<Palette> _palettes = [];
    private readonly Dictionary<string, string> _customTokens = new();

    // Only the theme/palette combinations actually in use (the active axes plus any requested by
    // a FlareThemeScope) are emitted into the static stylesheet -- not every registered one.
    private readonly HashSet<string> _neededThemeIds = new(StringComparer.Ordinal);
    private readonly HashSet<string> _neededPaletteIds = new(StringComparer.Ordinal);
    private string? _emittedCss;

    private ITheme _theme;
    private Palette _palette;
    private ThemeMode _mode;
    private bool _systemDark;
    private bool _isRtl;
    private bool _staticDirty = true;

    /// <summary>Initializes a new <see cref="ThemeService"/> with the given injector and default theme/palette/mode.</summary>
    public ThemeService(
        ICssVariableInjector injector,
        ITheme defaultTheme,
        Palette defaultPalette,
        ThemeMode defaultMode = ThemeMode.Auto,
        ThemeDelivery delivery = ThemeDelivery.ClassToggle)
    {
        _injector = injector;
        _theme = defaultTheme;
        _palette = defaultPalette;
        _mode = defaultMode;
        Delivery = delivery;
        _themes.Add(defaultTheme);
        _palettes.Add(defaultPalette);
    }

    /// <summary>The active design-system theme.</summary>
    public ITheme CurrentTheme => _theme;
    /// <summary>The active color palette.</summary>
    public Palette CurrentPalette => _palette;
    /// <summary>The selected mode (Light/Dark/Auto/HighContrast).</summary>
    public ThemeMode Mode => _mode;
    /// <summary>The effective dark state (resolves <see cref="ThemeMode.Auto"/> against the OS preference).</summary>
    public bool IsDark => _mode == ThemeMode.Dark || _mode == ThemeMode.HighContrast || (_mode == ThemeMode.Auto && _systemDark);
    /// <summary>Whether high-contrast mode is active.</summary>
    public bool IsHighContrast => _mode == ThemeMode.HighContrast;
    /// <summary>Whether right-to-left layout is enabled.</summary>
    public bool IsRtl => _isRtl;
    /// <summary>How theme CSS is delivered (class-toggle static CSS vs CSS-variable injection).</summary>
    public ThemeDelivery Delivery { get; }

    /// <summary>All registered themes.</summary>
    public IReadOnlyList<ITheme> Themes
    {
        get { lock (_lock) return _themes.ToList(); }
    }

    /// <summary>All registered color palettes.</summary>
    public IReadOnlyList<Palette> Palettes
    {
        get { lock (_lock) return _palettes.ToList(); }
    }

    /// <summary>Raised after any theme axis (theme, palette, mode or RTL) changes.</summary>
    public event Func<Task> OnThemeChanged = () => Task.CompletedTask;

    /// <summary>Registers a design-system theme so it can be activated by id.</summary>
    public void RegisterTheme(ITheme theme)
    {
        lock (_lock)
        {
            if (_themes.All(t => t.Id != theme.Id))
            {
                _themes.Add(theme);
                _staticDirty = true;
            }
        }
    }

    /// <summary>Registers a color palette so it can be activated by id.</summary>
    public void RegisterPalette(Palette palette)
    {
        lock (_lock)
        {
            if (_palettes.All(p => p.Id != palette.Id))
            {
                _palettes.Add(palette);
                _staticDirty = true;
            }
        }
    }

    /// <summary>Switches the active design-system theme by id and re-applies it.</summary>
    public async Task SetThemeAsync(string themeId)
    {
        ITheme theme;
        lock (_lock)
        {
            theme = _themes.FirstOrDefault(t => t.Id == themeId)
                ?? throw new InvalidOperationException($"Theme '{themeId}' is not registered.");
            _theme = theme;
        }
        await ApplyAsync();
    }

    /// <summary>Switches the active color palette by id and re-applies it.</summary>
    public async Task SetPaletteAsync(string paletteId)
    {
        Palette palette;
        lock (_lock)
        {
            palette = _palettes.FirstOrDefault(p => p.Id == paletteId)
                ?? throw new InvalidOperationException($"Palette '{paletteId}' is not registered.");
            _palette = palette;
        }
        await ApplyAsync();
    }

    /// <summary>Switches the light/dark/auto mode and re-applies it.</summary>
    public async Task SetModeAsync(ThemeMode mode)
    {
        _mode = mode;
        await ApplyAsync();
    }

    /// <summary>Enables or disables right-to-left layout.</summary>
    public async Task SetRtlAsync(bool isRtl)
    {
        _isRtl = isRtl;
        await ApplyAsync();
    }

    /// <summary>Feeds the current OS prefers-color-scheme so Auto mode can resolve.</summary>
    public async Task SetSystemDarkAsync(bool isDark)
    {
        if (_systemDark == isDark) return;
        _systemDark = isDark;
        if (_mode == ThemeMode.Auto)
            await ApplyAsync();
    }

    /// <summary>Generates a palette from a seed using the active theme's generator. Does not register or apply it.</summary>
    public Palette GeneratePalette(string id, string name, PaletteSeed seed, string? source = null) =>
        (_theme.PaletteGenerator ?? DefaultPaletteGenerator.Instance).Generate(id, name, seed, source);

    /// <summary>Ensures the class-scoped static stylesheet is present (ClassToggle delivery; no-op otherwise).</summary>
    public async Task EnsureStaticCssAsync()
    {
        if (Delivery != ThemeDelivery.ClassToggle) return;

        ITheme[] themesCopy;
        Palette[] palettesCopy;
        lock (_lock)
        {
            // The active axes are always part of the static stylesheet. We emit only the "needed"
            // theme/palette combinations (active + anything a FlareThemeScope asked for via
            // RequireThemeAssetsAsync) instead of every registered one, so a page using a single
            // theme keeps a ~10 KB stylesheet rather than one covering all registered palettes.
            _neededThemeIds.Add(_theme.Id);
            _neededPaletteIds.Add(_palette.Id);
            themesCopy = _themes.Where(t => _neededThemeIds.Contains(t.Id)).ToArray();
            palettesCopy = _palettes.Where(p => _neededPaletteIds.Contains(p.Id)).ToArray();
            _staticDirty = false;
        }

        var css = TokensToCss.Bundle(themesCopy, palettesCopy);
        lock (_lock)
        {
            if (css == _emittedCss) return;   // nothing new to push
            _emittedCss = css;
        }

        await _injector.SetStaticCssAsync(css);
    }

    /// <summary>Ensures a specific theme/palette's class-scoped CSS is emitted even when not active.</summary>
    public async Task RequireThemeAssetsAsync(string? themeId, string? paletteId)
    {
        if (Delivery != ThemeDelivery.ClassToggle) return;

        bool changed = false;
        lock (_lock)
        {
            if (themeId is not null && _themes.Any(t => t.Id == themeId))
                changed |= _neededThemeIds.Add(themeId);
            if (paletteId is not null && _palettes.Any(p => p.Id == paletteId))
                changed |= _neededPaletteIds.Add(paletteId);
        }

        if (changed) await EnsureStaticCssAsync();
    }

    private async Task ApplyAsync()
    {
        if (Delivery == ThemeDelivery.ClassToggle)
        {
            await EnsureStaticCssAsync();
        }
        else
        {
            var design = _theme.Design;
            if (IsDark && _theme.ExtendedDarkOverride is { } darkExt)
                design = design with { Extended = darkExt };
            await _injector.InjectAsync(design, ResolveColors());
        }

        Dictionary<string, string> customCopy;
        lock (_lock)
        {
            customCopy = new Dictionary<string, string>(_customTokens);
        }

        if (customCopy.Count > 0)
            await _injector.SetCustomTokensAsync(customCopy);
        await OnThemeChanged();
    }

    /// <summary>Resolves the <see cref="ColorScheme"/> effective for the active mode.</summary>
    private ColorScheme ResolveColors() => _mode switch
    {
        ThemeMode.HighContrast => _palette.HighContrast ?? _palette.Dark,
        ThemeMode.Dark => _palette.Dark,
        ThemeMode.Auto when _systemDark => _palette.Dark,
        _ => _palette.Light,
    };

    /// <summary>Pushes a typed override of the color axis into the custom-token layer.</summary>
    public void CustomizeColors(Func<ColorScheme, ColorScheme> mutate)
    {
        var baseline = ResolveColors();
        ApplyCustomDelta(baseline.FlattenColors(), mutate(baseline).FlattenColors());
    }

    /// <summary>Pushes a typed override of the design axis into the custom-token layer.</summary>
    public void CustomizeDesign(Func<DesignTokens, DesignTokens> mutate)
    {
        var baseline = _theme.Design;
        ApplyCustomDelta(FlattenComponentPriority(baseline), FlattenComponentPriority(mutate(baseline)));
    }

    /// <summary>
    /// Flattens design tokens with the component/core tokens taking precedence over the theme's
    /// <see cref="DesignTokens.Extended"/> overrides (the opposite of <see cref="CssVarMap.FlattenDesign"/>,
    /// where Extended wins). Used for the runtime override diff so that an edited component token wins
    /// over a shadowing Extended entry; Extended-only keys are still included. Because the diff cancels
    /// unchanged keys, the live (Extended-wins) rendering of untouched tokens is preserved.
    /// </summary>
    private static Dictionary<string, string> FlattenComponentPriority(DesignTokens d)
    {
        var result = new Dictionary<string, string>(d.Extended);
        foreach (var (k, v) in (d with { Extended = _noExtended }).FlattenDesign())
            result[k] = v;
        return result;
    }

    private static readonly IReadOnlyDictionary<string, string> _noExtended = new Dictionary<string, string>();

    /// <summary>Pushes only the vars that the mutator actually changed into the custom-token layer.</summary>
    private void ApplyCustomDelta(
        IReadOnlyDictionary<string, string> before,
        IReadOnlyDictionary<string, string> after)
    {
        var delta = new Dictionary<string, string>();
        foreach (var (k, val) in after)
            if (!before.TryGetValue(k, out var old) || old != val)
                delta[k] = val;

        if (delta.Count == 0) return;

        lock (_lock)
        {
            foreach (var (k, val) in delta)
                _customTokens[k] = val;
        }
        _ = _injector.SetCustomTokensAsync(delta);
    }

    /// <summary>Overrides a single CSS variable at runtime.</summary>
    public void SetCustomToken(string tokenName, string value)
    {
        lock (_lock)
        {
            _customTokens[tokenName] = value;
        }
        _ = _injector.SetCustomTokensAsync(new Dictionary<string, string> { [tokenName] = value });
    }

    /// <summary>Overrides multiple CSS variables at runtime.</summary>
    public void SetCustomTokens(IReadOnlyDictionary<string, string> tokens)
    {
        lock (_lock)
        {
            foreach (var (k, v) in tokens)
                _customTokens[k] = v;
        }
        _ = _injector.SetCustomTokensAsync(new Dictionary<string, string>(_customTokens));
    }

    /// <summary>Removes a previously set custom token override.</summary>
    public void ClearCustomToken(string tokenName)
    {
        lock (_lock)
        {
            _customTokens.Remove(tokenName);
        }
        _ = _injector.ClearCustomTokensAsync([tokenName]);
    }

    /// <summary>Removes all custom token overrides.</summary>
    public void ClearAllCustomTokens()
    {
        string[] names;
        lock (_lock)
        {
            names = _customTokens.Keys.ToArray();
            _customTokens.Clear();
        }
        _ = _injector.ClearCustomTokensAsync(names);
    }

    /// <summary>Returns all currently active custom token overrides.</summary>
    public IReadOnlyDictionary<string, string> GetCustomTokens()
    {
        lock (_lock)
        {
            return new Dictionary<string, string>(_customTokens);
        }
    }
}
