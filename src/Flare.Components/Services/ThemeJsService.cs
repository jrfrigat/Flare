using Flare.Core.Abstractions;
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

    /// <summary>Ensures the theme stylesheet link is present in the document head.</summary>
    public ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default)
        => InvokeVoidAsync("ensureStylesheet", href);

    /// <summary>Subscribes to OS color-scheme (light/dark) change notifications.</summary>
    public ValueTask SubscribeColorSchemeAsync(string id, DotNetObjectReference<Flare.Core.Components.FlareThemeProvider> dotNetRef, CancellationToken ct = default)
        => InvokeVoidAsync("subscribeColorScheme", id, dotNetRef);

    /// <summary>Removes the OS color-scheme change subscription.</summary>
    public ValueTask UnsubscribeColorSchemeAsync(string id, CancellationToken ct = default)
        => InvokeVoidAsync("unsubscribeColorScheme", id);

    /// <summary>Returns true when the OS currently prefers a dark color scheme.</summary>
    public ValueTask<bool> PrefersColorSchemeDarkAsync(CancellationToken ct = default)
        => InvokeAsync<bool>("prefersColorSchemeDark");
}
