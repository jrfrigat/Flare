using Flare.Core.Abstractions;
using Flare.Core.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Flare.Components.Tests;

/// <summary>
/// Pins the test run to the invariant culture so that resource-backed strings
/// (FlareStrings -> FlareStrings.resx) resolve to the neutral English values
/// deterministically, regardless of the developer's machine locale.
/// </summary>
internal static class TestCultureInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
    }
}

/// <summary>
/// Minimal stub for IThemeService - components subscribe to OnThemeChanged
/// but the base class treats IThemeService as optional (nullable cascading param).
/// Most components render fine with no ThemeService at all; this stub is available
/// when a test explicitly needs the cascading value.
/// </summary>
public sealed class StubThemeService : IThemeService
{
    private readonly ITheme _theme = new StubTheme();
    private static readonly Palette _palette = new()
    {
        Id = "stub",
        Name = "Stub",
        Light = null!,
        Dark = null!,
    };

    public ITheme CurrentTheme => _theme;
    public Palette CurrentPalette => _palette;
    public ThemeMode Mode => ThemeMode.Light;
    public bool IsDark => false;
    public bool IsHighContrast => false;
    public bool IsRtl => false;
    public ThemeDelivery Delivery => ThemeDelivery.ClassToggle;
    public IReadOnlyList<ITheme> Themes => [_theme];
    public IReadOnlyList<Palette> Palettes => [_palette];

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
    public Palette GeneratePalette(string id, string name, Flare.Core.Services.PaletteSeed seed, string? source = null) =>
        Flare.Core.Services.DefaultPaletteGenerator.Instance.Generate(id, name, seed, source);
    public void CustomizeColors(Func<ColorScheme, ColorScheme> mutate) { }
    public void CustomizeDesign(Func<DesignTokens, DesignTokens> mutate) { }
    public void SetCustomToken(string tokenName, string value) { }
    public void SetCustomTokens(IReadOnlyDictionary<string, string> tokens) { }
    public void ClearCustomToken(string tokenName) { }
    public void ClearAllCustomTokens() { }
    public IReadOnlyDictionary<string, string> GetCustomTokens() => new Dictionary<string, string>();
}

/// <summary>
/// IThemeService stub that exposes a caller-supplied custom-token dictionary, for testing
/// token-gated behaviour (e.g. FlareProgress wavy mode keyed on --flare-progress-wavy-enabled).
/// </summary>
public sealed class TokenThemeService : IThemeService
{
    private readonly IReadOnlyDictionary<string, string> _tokens;
    public TokenThemeService(IReadOnlyDictionary<string, string> tokens) => _tokens = tokens;

    private readonly ITheme _theme = new StubTheme();
    private static readonly Palette _palette = new() { Id = "stub", Name = "Stub", Light = null!, Dark = null! };

    public ITheme CurrentTheme => _theme;
    public Palette CurrentPalette => _palette;
    public ThemeMode Mode => ThemeMode.Light;
    public bool IsDark => false;
    public bool IsHighContrast => false;
    public bool IsRtl => false;
    public ThemeDelivery Delivery => ThemeDelivery.ClassToggle;
    public IReadOnlyList<ITheme> Themes => [_theme];
    public IReadOnlyList<Palette> Palettes => [_palette];
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
    public Palette GeneratePalette(string id, string name, Flare.Core.Services.PaletteSeed seed, string? source = null) =>
        Flare.Core.Services.DefaultPaletteGenerator.Instance.Generate(id, name, seed, source);
    public void CustomizeColors(Func<ColorScheme, ColorScheme> mutate) { }
    public void CustomizeDesign(Func<DesignTokens, DesignTokens> mutate) { }
    public void SetCustomToken(string tokenName, string value) { }
    public void SetCustomTokens(IReadOnlyDictionary<string, string> tokens) { }
    public void ClearCustomToken(string tokenName) { }
    public void ClearAllCustomTokens() { }
    public IReadOnlyDictionary<string, string> GetCustomTokens() => _tokens;
}

internal sealed class StubTheme : ITheme
{
    public string Id => "stub";
    public string DisplayName => "Stub";
    // null! intentional: tests don't consume token values;
    // ThemeService is a nullable cascading param on FlareComponentBase.
    public DesignTokens Design => null!;
    public string DefaultPaletteId => "stub";
    public IReadOnlyList<string> StyleAssets => [];
}

/// <summary>
/// Stub ICollisionService for testing.
/// </summary>
public sealed class StubCollisionService : ICollisionService
{
    public ValueTask<CollisionResult> CalculatePlacementAsync(
        Microsoft.AspNetCore.Components.ElementReference anchor,
        Microsoft.AspNetCore.Components.ElementReference floating,
        string preferredPlacement,
        CollisionOptions? options = null,
        CancellationToken ct = default)
    {
        return ValueTask.FromResult(new CollisionResult
        {
            Placement = preferredPlacement,
            Top = 0,
            Left = 0,
            ArrowTop = 0,
            ArrowLeft = 0,
            NeedsFlip = false
        });
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// Stub IThemeJsService for testing.
/// </summary>
public sealed class StubThemeJsService : IThemeJsService
{
    public ValueTask SetCssVariablesAsync(IReadOnlyDictionary<string, string> vars, CancellationToken ct = default) => ValueTask.CompletedTask;
    public ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default) => ValueTask.CompletedTask;
    public ValueTask SetStaticCssAsync(string css, CancellationToken ct = default) => ValueTask.CompletedTask;
    public ValueTask SetThemeClassesAsync(string themeId, string paletteId, bool isDark, CancellationToken ct = default) => ValueTask.CompletedTask;
    public ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default) => ValueTask.CompletedTask;
    public ValueTask SubscribeColorSchemeAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class => ValueTask.CompletedTask;
    public ValueTask UnsubscribeColorSchemeAsync(string id, CancellationToken ct = default) => ValueTask.CompletedTask;
    public ValueTask<bool> PrefersColorSchemeDarkAsync(CancellationToken ct = default) => ValueTask.FromResult(false);
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// bUnit context with Flare services pre-registered.
/// Wraps Bunit.BunitContext (bUnit 2) and adds a StubThemeService.
/// </summary>
public class FlareTestContext : BunitContext
{
    public FlareTestContext()
    {
        Services.AddSingleton<IThemeService, StubThemeService>();
        Services.AddSingleton(TimeProvider.System);
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Typed JS-interop services consumed by migrated components (clipboard/download/color extractor).
        Services.AddScoped<IFlareClipboard, FlareClipboardService>();
        Services.AddScoped<IFlareDownload, FlareDownloadService>();

        // Collision and Theme JS services.
        Services.AddScoped<ICollisionService, StubCollisionService>();
        Services.AddScoped<IThemeJsService, StubThemeJsService>();
        Services.AddScoped<Flare.Components.Services.ISplitterJsService, Flare.Components.Services.SplitterJsService>();
        Services.AddScoped<Flare.Components.Services.ITreeJsService, Flare.Components.Services.TreeJsService>();
    }
}
