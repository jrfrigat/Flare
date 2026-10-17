using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The theme root and the document element have to agree on the color mode: overlays portaled to body are styled
/// by the document element, the app content by the root. In high contrast both carry the high-contrast class and
/// the dark one.
/// </summary>
public sealed class ThemeModeClassesTests : FlareTestContext
{
    private readonly RecordingJs _js = new();

    public ThemeModeClassesTests()
    {
        Services.AddSingleton<IThemeStorageService, NothingSaved>();
        Services.AddSingleton<IThemeJsService>(_js);
    }

    [Theory]
    [InlineData(ThemeMode.Light, false, false)]
    [InlineData(ThemeMode.Dark, true, false)]
    [InlineData(ThemeMode.HighContrast, true, true)]
    public async Task RootAndDocumentGetTheSameModeClasses(ThemeMode mode, bool dark, bool highContrast)
    {
        var cut = Render<FlareThemeProvider>();
        var theme = Services.GetRequiredService<IThemeService>();

        await cut.InvokeAsync(() => theme.SetModeAsync(mode));

        var root = cut.Find($".{Css.Classes.Theme.Root}").ClassList;
        var expected = Css.Classes.Theme.ModeClasses(dark, highContrast).Split(' ');
        Assert.All(expected, c => Assert.Contains(c, root));
        Assert.Equal(expected.Length, root.Count(c => c.StartsWith("flare-mode-", StringComparison.Ordinal)));
        cut.WaitForAssertion(() => Assert.Equal((dark, highContrast), _js.LastMode));
    }

    private sealed class NothingSaved : IThemeStorageService
    {
        public Task<ThemeSelection?> GetAsync() => Task.FromResult<ThemeSelection?>(null);
        public Task SaveAsync(ThemeSelection selection) => Task.CompletedTask;
    }

    private sealed class RecordingJs : IThemeJsService
    {
        public (bool Dark, bool HighContrast)? LastMode { get; private set; }

        public ValueTask SetThemeClassesAsync(IReadOnlyList<string> themeLineage, string paletteId, bool isDark, bool isHighContrast, CancellationToken ct = default)
        {
            LastMode = (isDark, isHighContrast);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetThemeClassesAsync(string themeId, string paletteId, bool isDark, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SetCssVariablesAsync(IReadOnlyDictionary<string, string> vars, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SetStaticCssAsync(string css, CancellationToken ct = default) => ValueTask.CompletedTask;
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
