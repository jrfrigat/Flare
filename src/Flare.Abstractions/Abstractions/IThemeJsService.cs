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
    /// Sets the root element's theme classes, including the style-family class that a theme derived
    /// from another needs for the base theme's stylesheets to keep applying to it. Defaults to the
    /// single-id overload, which emits the theme's own class only.
    /// </summary>
    /// <param name="themeId">The active theme's id.</param>
    /// <param name="styleFamilyId">The active theme's <see cref="ITheme.StyleFamilyId"/>.</param>
    /// <param name="paletteId">The active palette's id.</param>
    /// <param name="isDark">Whether the dark scheme is active.</param>
    /// <param name="ct">A cancellation token.</param>
    ValueTask SetThemeClassesAsync(string themeId, string styleFamilyId, string paletteId, bool isDark, CancellationToken ct = default)
        => SetThemeClassesAsync(themeId, paletteId, isDark, ct);

    /// <summary>
    /// Ensures a stylesheet link is present and completes only once it has finished loading (or a
    /// safety timeout elapses), so callers can reveal styled UI without flashing unstyled content.
    /// </summary>
    ValueTask EnsureStylesheetAsync(string href, CancellationToken ct = default);

    /// <summary>
    /// Ensures a theme JavaScript module is loaded once. Defaults to doing nothing, so that an
    /// adapter written before <see cref="ITheme.ScriptAssets"/> existed still compiles; such an
    /// adapter serves themes that ship no modules, which is all of the built-in ones.
    /// </summary>
    /// <param name="src">Module URL, resolved against the document base URI.</param>
    /// <param name="ct">A cancellation token.</param>
    ValueTask EnsureModuleAsync(string src, CancellationToken ct = default) => ValueTask.CompletedTask;

    /// <summary>
    /// Completes once the document's web fonts have loaded (text typefaces and icon glyphs), or after
    /// the given safety timeout elapses. Used to gate the startup splash so text appears in its final
    /// typeface without a font-swap flash.
    /// </summary>
    /// <param name="timeoutMs">Maximum time to wait before completing anyway, in milliseconds.</param>
    /// <param name="ct">A cancellation token.</param>
    ValueTask WhenFontsReadyAsync(int timeoutMs = 3000, CancellationToken ct = default);

    /// <summary>
    /// Signals app-readiness after the freshly applied theme has painted: calls
    /// <c>window.hideFlareSplash()</c> (from <c>flare-bootstrap.js</c>), which dispatches a
    /// <c>flare:ready</c> event and fades out the app's own tagged splash element. A safe no-op when
    /// the bootstrap script is absent.
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
