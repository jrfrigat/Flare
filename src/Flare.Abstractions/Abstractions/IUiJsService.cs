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
    /// <summary>Observes a tab bar and reports overflow state via <c>OnTabScrollState</c>.</summary>
    /// <param name="bar">The scrollable tab-bar element.</param>
    /// <param name="dotNetRef">The component reference whose <c>OnTabScrollState(bool,bool,bool)</c> is invoked.</param>
    ValueTask RegisterTabScrollerAsync<T>(ElementReference bar, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Scrolls the tab bar by ~80% of its width in <paramref name="direction"/> (-1 left, +1 right).</summary>
    ValueTask ScrollTabsAsync(ElementReference bar, int direction);

    /// <summary>Removes the tab-scroller observer for <paramref name="bar"/>.</summary>
    ValueTask RemoveTabScrollerAsync(ElementReference bar);

    /// <summary>Watches a collapsible button group and folds the segments that no longer fit into its
    /// overflow menu, reporting how many were folded via <c>OnOverflowChanged</c>.</summary>
    /// <param name="root">The button-group element.</param>
    /// <param name="dotNetRef">The component reference whose <c>OnOverflowChanged(int)</c> is invoked.</param>
    ValueTask RegisterButtonGroupCollapseAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Re-runs the fold for <paramref name="root"/>. Needed after every render of the group: the
    /// decision lives in an attribute on segments the component may have just replaced.</summary>
    ValueTask ApplyButtonGroupOverflowAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Removes the collapse observer for <paramref name="root"/>.</summary>
    ValueTask RemoveButtonGroupCollapseAsync(ElementReference root);

    /// <summary>Registers a global keydown listener that invokes <c>HandleKeyDown</c> with the combo string.</summary>
    /// <param name="dotNetRef">The component reference whose <c>HandleKeyDown(string)</c> is invoked.</param>
    ValueTask RegisterShortcutsAsync<T>(DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Removes the global keyboard-shortcut listener.</summary>
    ValueTask RemoveShortcutsAsync();

    /// <summary>Returns true when the browser supports the EyeDropper API.</summary>
    ValueTask<bool> SupportsEyeDropperAsync();

    /// <summary>Opens the system eyedropper and returns the picked color (sRGB hex), or null if cancelled.</summary>
    ValueTask<string?> OpenEyeDropperAsync();

    /// <summary>
    /// Makes the browser confirm before the tab is closed or navigated away from. The message is the
    /// caller's, but browsers have shown their own wording since 2016 and none of them will show this
    /// one - it exists for the in-app half, which CAN say what it likes.
    /// </summary>
    ValueTask SetUnloadPromptAsync(bool enabled);
}
