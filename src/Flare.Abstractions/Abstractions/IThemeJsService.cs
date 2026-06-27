using Microsoft.JSInterop;

namespace Flare.Abstractions;

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

    /// <summary>
    /// Ensures a stylesheet link is present and completes only once it has finished loading (or a
    /// safety timeout elapses), so callers can reveal styled UI without flashing unstyled content.
    /// </summary>
    ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default);

    /// <summary>
    /// Completes once the document's web fonts have loaded (text typefaces and icon glyphs), or after
    /// the given safety timeout elapses. Used to gate the startup splash so text appears in its final
    /// typeface without a font-swap flash.
    /// </summary>
    /// <param name="timeoutMs">Maximum time to wait before completing anyway, in milliseconds.</param>
    /// <param name="ct">A cancellation token.</param>
    ValueTask WhenFontsReadyAsync(int timeoutMs = 3000, CancellationToken ct = default);

    /// <summary>
    /// Fades out the anti-FOUC startup splash painted by <c>flare-bootstrap.js</c>, after the freshly
    /// applied theme has painted. A safe no-op when no splash is present.
    /// </summary>
    ValueTask RevealAppAsync(CancellationToken ct = default);

    /// <summary>
    /// Subscribe to OS color scheme changes. <typeparamref name="T"/> is the .NET object exposing the
    /// <c>[JSInvokable]</c> callback (kept generic so this contract does not depend on any UI component).
    /// </summary>
    ValueTask SubscribeColorSchemeAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class;

    /// <summary>Unsubscribe from OS color scheme changes.</summary>
    ValueTask UnsubscribeColorSchemeAsync(string id, CancellationToken ct = default);

    /// <summary>Check if OS prefers dark mode.</summary>
    ValueTask<bool> PrefersColorSchemeDarkAsync(CancellationToken ct = default);

    /// <summary>
    /// Reads the OS/browser accent color (CSS <c>AccentColor</c> system color) as a <c>#RRGGBB</c> hex,
    /// or null when the engine does not expose it. Used to seed the Dynamic Color palette.
    /// </summary>
    ValueTask<string?> GetAccentColorAsync(CancellationToken ct = default);

    /// <summary>Subscribe to OS accent-color changes (re-read on window focus).</summary>
    ValueTask SubscribeAccentAsync<T>(string id, DotNetObjectReference<T> dotNetRef, CancellationToken ct = default) where T : class;

    /// <summary>Unsubscribe from OS accent-color changes.</summary>
    ValueTask UnsubscribeAccentAsync(string id, CancellationToken ct = default);
}
