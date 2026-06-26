using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for overlay/popup behaviours shared by dialogs, drawers, selects and pickers:
/// body scroll-lock, dialog Escape handling, focus trapping, outside-click dismissal and
/// fixed-position anchored panels. Wraps the <c>flare-overlay.js</c> module so components inject a
/// service instead of importing the module and calling string identifiers themselves.
/// </summary>
public interface IOverlayJsService : IAsyncDisposable
{
    /// <summary>Locks <c>&lt;body&gt;</c> scrolling (reference-counted; nestable across overlays).</summary>
    ValueTask LockBodyScrollAsync();

    /// <summary>Releases one body scroll-lock taken by <see cref="LockBodyScrollAsync"/>.</summary>
    ValueTask UnlockBodyScrollAsync();

    /// <summary>Registers a document Escape handler that invokes <c>CloseFromEsc</c> on the reference.</summary>
    /// <param name="id">A stable id identifying this overlay's handler.</param>
    /// <param name="dotNetRef">The component reference whose <c>CloseFromEsc</c> is invoked on Escape.</param>
    ValueTask RegisterDialogEscAsync<T>(string id, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Removes the Escape handler registered under <paramref name="id"/>.</summary>
    ValueTask RemoveDialogEscAsync(string id);

    /// <summary>Traps Tab focus within <paramref name="container"/> and focuses its first focusable element.</summary>
    /// <param name="id">A stable id identifying this focus trap.</param>
    /// <param name="container">The element to trap focus within.</param>
    ValueTask TrapFocusAsync(string id, ElementReference container);

    /// <summary>Releases the focus trap registered under <paramref name="id"/> and restores prior focus.</summary>
    ValueTask ReleaseFocusTrapAsync(string id);

    /// <summary>Focuses the first focusable element inside <paramref name="container"/>.</summary>
    ValueTask FocusFirstAsync(ElementReference container);

    /// <summary>Registers a document outside-click handler that invokes <paramref name="method"/> on the reference.</summary>
    /// <param name="id">A stable id identifying this handler.</param>
    /// <param name="element">The element whose interior clicks are ignored; clicks outside it dismiss.</param>
    /// <param name="dotNetRef">The component reference whose <paramref name="method"/> is invoked.</param>
    /// <param name="method">The <c>[JSInvokable]</c> method name to invoke on an outside click.</param>
    ValueTask RegisterOutsideClickAsync<T>(string id, ElementReference element,
        DotNetObjectReference<T> dotNetRef, string method) where T : class;

    /// <summary>Removes the outside-click handler registered under <paramref name="id"/>.</summary>
    ValueTask RemoveOutsideClickAsync(string id);

    /// <summary>
    /// Positions <paramref name="panel"/> as a fixed popup anchored to <paramref name="anchor"/>,
    /// flipping above when there is not enough room below and re-positioning on scroll/resize.
    /// </summary>
    /// <param name="id">A stable id identifying this anchored panel.</param>
    /// <param name="anchor">The element the panel is positioned against.</param>
    /// <param name="panel">The popup element to position.</param>
    /// <param name="options">Optional placement options (e.g. <c>matchWidth</c>, <c>gap</c>).</param>
    ValueTask PositionAnchoredPanelAsync(string id, ElementReference anchor, ElementReference panel, object? options = null);

    /// <summary>Stops positioning and detaches listeners for the anchored panel under <paramref name="id"/>.</summary>
    ValueTask RemoveAnchoredPanelAsync(string id);
}
