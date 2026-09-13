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

    private static Palette BrandPalette() =>
        new() { Id = "brand", Name = "Brand", Light = null!, Dark = null!, StyleAsset = BrandPaletteCss };

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
