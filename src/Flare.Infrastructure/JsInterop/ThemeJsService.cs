using Flare.Abstractions;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Implementation of IThemeJsService using JS interop.
/// </summary>
public sealed class ThemeJsService : FlareJsModule, IThemeJsService
{
    /// <summary>Initializes a new <see cref="ThemeJsService"/>.</summary>
    public ThemeJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-theme.js") { }

    /// <summary>Writes the given CSS custom properties onto the document root.</summary>
    public ValueTask SetCssVariablesAsync(IReadOnlyDictionary<string, string> vars, CancellationToken ct = default)
        => InvokeVoidAsync("setCssVariables", vars);

    /// <summary>Removes previously injected custom token overrides from the document root.</summary>
    public ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default)
        => InvokeVoidAsync("clearCustomTokens", tokenNames);

    /// <summary>Sets the static (non-variable) theme CSS block in the document.</summary>
    public ValueTask SetStaticCssAsync(string css, CancellationToken ct = default)
        => InvokeVoidAsync("setStaticThemeCss", css);

    /// <summary>Applies the active theme's CSS classes to the document root.</summary>
    public ValueTask SetThemeClassesAsync(string themeId, string paletteId, bool isDark, CancellationToken ct = default)
        => InvokeVoidAsync("setThemeClasses", themeId, paletteId, isDark);

    /// <summary>Ensures the theme stylesheet link is present and has finished loading.</summary>
    public ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default)
        => InvokeVoidAsync("ensureStylesheet", href);

    /// <summary>Completes once web fonts are loaded (or the safety timeout elapses).</summary>
    public ValueTask WhenFontsReadyAsync(int timeoutMs = 3000, CancellationToken ct = default)
        => InvokeVoidAsync("whenFontsReady", timeoutMs);

    /// <summary>Signals app-readiness (flare:ready + fades the app's own tagged splash) once the themed UI has painted.</summary>
    public ValueTask RevealAppAsync(CancellationToken ct = default)
        => InvokeVoidAsync("revealApp");

    /// <summary>Subscribes to OS color-scheme (light/dark) change notifications.</summary>
    public ValueTask SubscribeColorSchemeAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class
        => InvokeVoidAsync("subscribeColorScheme", id, dotNetRef);

    /// <summary>Removes the OS color-scheme change subscription.</summary>
    public ValueTask UnsubscribeColorSchemeAsync(string id, CancellationToken ct = default)
        => InvokeVoidAsync("unsubscribeColorScheme", id);

    /// <summary>Returns true when the OS currently prefers a dark color scheme.</summary>
    public ValueTask<bool> PrefersColorSchemeDarkAsync(CancellationToken ct = default)
        => InvokeAsync<bool>("prefersColorSchemeDark");

    /// <summary>Reads the OS/browser accent color as a hex string, or null when unavailable.</summary>
    public ValueTask<string?> GetAccentColorAsync(CancellationToken ct = default)
        => InvokeAsync<string?>("getAccentColor");

    /// <summary>Subscribes to OS accent-color changes (re-read on window focus).</summary>
    public ValueTask SubscribeAccentAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class
        => InvokeVoidAsync("subscribeAccent", id, dotNetRef);

    /// <summary>Removes the OS accent-color change subscription.</summary>
    public ValueTask UnsubscribeAccentAsync(string id, CancellationToken ct = default)
        => InvokeVoidAsync("unsubscribeAccent", id);
}
