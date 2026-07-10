using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for miscellaneous UI utilities backed by <c>flare-ui.js</c>: a scroll-to-top
/// button, tab-bar overflow scrolling, global keyboard shortcuts and the EyeDropper API. Wraps the
/// module so components inject a service instead of importing it and calling string identifiers
/// themselves. Responsive breakpoint / viewport detection lives in <see cref="IBrowserViewportService"/>.
/// </summary>
public interface IUiJsService : IAsyncDisposable
{
    /// <summary>Watches a scroll container and invokes <c>SetVisible</c> on the reference past <paramref name="threshold"/>.</summary>
    /// <param name="id">A stable id identifying this handler.</param>
    /// <param name="dotNetRef">The component reference whose <c>SetVisible(bool)</c> is invoked.</param>
    /// <param name="threshold">Scroll offset (px) past which the button becomes visible.</param>
    /// <param name="selector">Optional scroll-container selector; null watches the window.</param>
    ValueTask RegisterScrollTopAsync<T>(string id, DotNetObjectReference<T> dotNetRef, double threshold, string? selector) where T : class;

    /// <summary>Removes the scroll-top handler registered under <paramref name="id"/>.</summary>
    ValueTask RemoveScrollTopAsync(string id);

    /// <summary>Smoothly scrolls the matched container (or the window) to the top.</summary>
    ValueTask ScrollToTopAsync(string? selector);

    /// <summary>Observes a tab bar and reports overflow state via <c>OnTabScrollState</c>.</summary>
    /// <param name="bar">The scrollable tab-bar element.</param>
    /// <param name="dotNetRef">The component reference whose <c>OnTabScrollState(bool,bool,bool)</c> is invoked.</param>
    ValueTask RegisterTabScrollerAsync<T>(ElementReference bar, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Scrolls the tab bar by ~80% of its width in <paramref name="direction"/> (-1 left, +1 right).</summary>
    ValueTask ScrollTabsAsync(ElementReference bar, int direction);

    /// <summary>Removes the tab-scroller observer for <paramref name="bar"/>.</summary>
    ValueTask RemoveTabScrollerAsync(ElementReference bar);

    /// <summary>Registers a global keydown listener that invokes <c>HandleKeyDown</c> with the combo string.</summary>
    /// <param name="dotNetRef">The component reference whose <c>HandleKeyDown(string)</c> is invoked.</param>
    ValueTask RegisterShortcutsAsync<T>(DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Removes the global keyboard-shortcut listener.</summary>
    ValueTask RemoveShortcutsAsync();

    /// <summary>Returns true when the browser supports the EyeDropper API.</summary>
    ValueTask<bool> SupportsEyeDropperAsync();

    /// <summary>Opens the system eyedropper and returns the picked color (sRGB hex), or null if cancelled.</summary>
    ValueTask<string?> OpenEyeDropperAsync();
}
