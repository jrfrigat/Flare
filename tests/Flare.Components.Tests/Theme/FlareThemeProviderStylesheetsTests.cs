using Microsoft.AspNetCore.Components.Web;
using Bunit;
using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public sealed class FlareThemeProviderStylesheetsTests : FlareTestContext
{
    private const string BrandPaletteCss = "css/brand-palette.css";

    private readonly RecordingThemeJsService _js = new();

    public FlareThemeProviderStylesheetsTests()
    {
        Services.AddSingleton<IThemeStorageService, NothingSaved>();
        Services.AddSingleton<IThemeJsService>(_js);
    }

    [Fact]
    public void ByDefaultTheProviderWritesTheThemeLinks()
    {
        var cut = Render<FlareThemeProvider>();

        Assert.Single(cut.FindComponents<FlareStyles>());
    }

    /// <summary>
    /// An app that links the theme stylesheets in its own head must not get a second set: the same
    /// sheet would be applied twice, the later copy moving the theme after the app's own overrides.
    /// </summary>
    [Fact]
    public void InManualModeTheProviderWritesNoLinks()
    {
        var cut = Render<FlareThemeProvider>(p => p.Add(x => x.Stylesheets, ThemeStylesheets.Manual));

        Assert.Empty(cut.FindComponents<FlareStyles>());
    }

    /// <summary>
    /// With no FlareStyles on the page, the provider is the only thing that can fetch a palette's own
    /// stylesheet; without it the palette class switches and none of its declarations apply.
    /// </summary>
    [Fact]
    public void InManualModeTheProviderLoadsTheActivePaletteStylesheet()
    {
        Services.AddSingleton<IThemeService>(new StubThemeService(BrandPalette()));

        Render<FlareThemeProvider>(p => p.Add(x => x.Stylesheets, ThemeStylesheets.Manual));

        Assert.Contains(BrandPaletteCss, _js.Stylesheets);
    }

    [Fact]
    public void AScopedPaletteStylesheetIsLoaded()
    {
        Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService(BrandPalette()))
            .Add(x => x.Palette, "brand"));

        Assert.Contains(BrandPaletteCss, _js.Stylesheets);
    }

    /// <summary>
    /// An app that registers several themes paints with one of them. Emitting the others' sheets costs a
    /// request each - after .NET has started, in a WebAssembly app - for CSS nothing applies, which is
    /// why the head gets the active theme alone. Switching is not what pays for it: the provider fetches
    /// an incoming theme's sheet before it swaps the root classes.
    /// </summary>
    [Fact]
    public void ByDefaultOnlyTheActiveThemesStylesheetReachesTheHead()
    {
        Services.AddSingleton<IThemeService>(new ThreeThemes());

        var head = RenderHeadWithStyles();

        Assert.Contains("css/active.css", head);
        Assert.DoesNotContain("css/other-1.css", head);
        Assert.DoesNotContain("css/other-2.css", head);
    }

    /// <summary>
    /// The way back, for CSS of one's own that reads an inactive theme's variables or a theme switched
    /// past the provider.
    /// </summary>
    [Fact]
    public void ActiveOnlyFalseReachesTheHeadWithEveryRegisteredSheet()
    {
        Services.AddSingleton<IThemeService>(new ThreeThemes());

        var head = RenderHeadWithStyles(p => p.Add(x => x.ActiveOnly, false));

        Assert.Contains("css/active.css", head);
        Assert.Contains("css/other-1.css", head);
        Assert.Contains("css/other-2.css", head);
    }

    // HeadContent only reaches a HeadOutlet, so the outlet is rendered first and then read back.
    private string RenderHeadWithStyles(
        Action<ComponentParameterCollectionBuilder<FlareStyles>>? parameters = null)
    {
        var outlet = Render<HeadOutlet>();
        Render<FlareStyles>(parameters ?? (_ => { }));
        return outlet.Markup;
    }

    private static Palette BrandPalette() =>
        new() { Id = "brand", Name = "Brand", Light = null!, Dark = null!, StyleAsset = BrandPaletteCss };

    // Three registered themes, each with a sheet of its own, and the first one active. Everything the
    // stylesheet question does not touch is left to the shared stub.
    private sealed class ThreeThemes : IThemeService
    {
        private readonly StubThemeService _rest = new();
        private readonly ITheme _active = new SheetTheme("active");
        private readonly ITheme[] _all = [new SheetTheme("active"), new SheetTheme("other-1"), new SheetTheme("other-2")];

        public ITheme CurrentTheme => _active;
        public IReadOnlyList<ITheme> Themes => _all;
        public Palette CurrentPalette => _rest.CurrentPalette;
        public IReadOnlyList<Palette> Palettes => _rest.Palettes;
        public bool IsDynamicPalette => false;
        public string DynamicFallbackSeed => _rest.DynamicFallbackSeed;
        public Palette? DynamicFallbackPalette => null;
        public ThemeMode Mode => ThemeMode.Light;
        public bool IsDark => false;
        public bool IsHighContrast => false;
        public bool IsRtl => false;
        public ThemeDelivery Delivery => ThemeDelivery.ClassToggle;

        public event Func<Task> OnThemeChanged = () => Task.CompletedTask;

        public void RegisterTheme(ITheme theme) { }
        public void RegisterPalette(Palette palette) { }
        public Task SetThemeAsync(string themeId) => Task.CompletedTask;
        public Task SetPaletteAsync(string paletteId) => Task.CompletedTask;
        public Task SetModeAsync(ThemeMode mode) => Task.CompletedTask;
        public Task SetRtlAsync(bool isRtl) => Task.CompletedTask;
        public Task SetSystemDarkAsync(bool isDark) => Task.CompletedTask;
        public Task EnsureStaticCssAsync() => Task.CompletedTask;
        public Task RequireThemeAssetsAsync(string? themeId, string? paletteId) => Task.CompletedTask;
        public Palette GeneratePalette(string id, string name, PaletteSeed seed, string? source = null) =>
            _rest.GeneratePalette(id, name, seed, source);
        public Task ApplyDynamicPaletteAsync(PaletteSeed seed) => Task.CompletedTask;
        public Task ApplyDynamicPaletteAsync(Palette source) => Task.CompletedTask;
        public void CustomizeColors(Func<ColorScheme, ColorScheme> mutate) { }
        public void CustomizeDesign(Func<DesignTokens, DesignTokens> mutate) { }
        public void SetCustomToken(string tokenName, string value) { }
        public void SetCustomTokens(IReadOnlyDictionary<string, string> tokens) { }
        public void ClearCustomToken(string tokenName) { }
        public void ClearAllCustomTokens() { }
        public IReadOnlyDictionary<string, string> GetCustomTokens() => new Dictionary<string, string>();

        private sealed class SheetTheme(string id) : ITheme
        {
            public string Id => id;
            public string DisplayName => id;
            public DesignTokens Design => null!;
            public string DefaultPaletteId => "stub";
            public IReadOnlyList<string> StyleAssets => [$"css/{id}.css"];
        }
    }

    private sealed class NothingSaved : IThemeStorageService
    {
        public Task<ThemeSelection?> GetAsync() => Task.FromResult<ThemeSelection?>(null);
        public Task SaveAsync(ThemeSelection selection) => Task.CompletedTask;
    }

    private sealed class RecordingThemeJsService : IThemeJsService
    {
        public List<string> Stylesheets { get; } = [];

        public ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default)
        {
            Stylesheets.Add(href);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetCssVariablesAsync(IReadOnlyDictionary<string, string> vars, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SetStaticCssAsync(string css, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SetThemeClassesAsync(string themeId, string paletteId, bool isDark, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask WhenFontsReadyAsync(int timeoutMs = 3000, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask RevealAppAsync(CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SubscribeColorSchemeAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class => ValueTask.CompletedTask;
        public ValueTask UnsubscribeColorSchemeAsync(string id, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask<bool> PrefersColorSchemeDarkAsync(CancellationToken ct = default) => ValueTask.FromResult(false);
        public ValueTask<string?> GetAccentColorAsync(CancellationToken ct = default) => ValueTask.FromResult<string?>(null);
        public ValueTask SubscribeAccentAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class => ValueTask.CompletedTask;
        public ValueTask UnsubscribeAccentAsync(string id, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
