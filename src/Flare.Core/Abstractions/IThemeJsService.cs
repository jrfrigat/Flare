using Microsoft.JSInterop;

namespace Flare.Core.Abstractions;

/// <summary>
/// Provides JS operations for theme management.
/// Handles CSS variable injection and theme class toggling.
/// </summary>
public interface IThemeJsService : IAsyncDisposable
{
    /// <summary>Set CSS variables on the document root.</summary>
    ValueTask SetCssVariablesAsync(IReadOnlyDictionary<string, string> vars, CancellationToken ct = default);

    /// <summary>Clear custom CSS variables.</summary>
    ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default);

    /// <summary>Set static theme CSS.</summary>
    ValueTask SetStaticCssAsync(string css, CancellationToken ct = default);

    /// <summary>Set theme classes on the root element.</summary>
    ValueTask SetThemeClassesAsync(string themeId, string paletteId, bool isDark, CancellationToken ct = default);

    /// <summary>Ensure a stylesheet is loaded.</summary>
    ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default);

    /// <summary>Subscribe to OS color scheme changes.</summary>
    ValueTask SubscribeColorSchemeAsync(string id, DotNetObjectReference<Components.FlareThemeProvider> dotNetRef, CancellationToken ct = default);

    /// <summary>Unsubscribe from OS color scheme changes.</summary>
    ValueTask UnsubscribeColorSchemeAsync(string id, CancellationToken ct = default);

    /// <summary>Check if OS prefers dark mode.</summary>
    ValueTask<bool> PrefersColorSchemeDarkAsync(CancellationToken ct = default);
}
