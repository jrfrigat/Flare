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

    /// <summary>
    /// Positions <paramref name="panel"/> as a fixed popup anchored to <paramref name="anchor"/>,
    /// flipping to the opposite side when there is not enough room, re-positioning on scroll/resize,
    /// and promoting the panel to the browser's top layer so no ancestor can clip or overpaint it.
    /// This is the single positioning path for every floating surface in the library.
    /// </summary>
    /// <param name="id">A stable id identifying this anchored panel.</param>
    /// <param name="anchor">
    /// The element the panel is positioned against. Ignored when
    /// <see cref="AnchoredPanelOptions.AnchorRect"/> supplies viewport coordinates instead.
    /// </param>
    /// <param name="panel">The popup element to position.</param>
    /// <param name="options">Placement options; the defaults place the panel below the anchor.</param>
    ValueTask PositionAnchoredPanelAsync(string id, ElementReference anchor, ElementReference panel,
        AnchoredPanelOptions? options = null);

    /// <summary>
    /// Same as <see cref="PositionAnchoredPanelAsync"/>, with the anchor named by its DOM id. For a
    /// panel whose anchor is one of many rendered in a loop - a per-column filter button - where a
    /// single captured <see cref="ElementReference"/> would hold whichever one rendered last.
    /// </summary>
    /// <param name="id">A stable id identifying this anchored panel.</param>
    /// <param name="anchorElementId">The <c>id</c> attribute of the anchor element.</param>
    /// <param name="panel">The popup element to position.</param>
    /// <param name="options">Placement options; the defaults place the panel below the anchor.</param>
    ValueTask PositionAnchoredPanelByIdAsync(string id, string anchorElementId, ElementReference panel,
        AnchoredPanelOptions? options = null);

    /// <summary>Stops positioning, releases the top layer and detaches listeners for the panel under <paramref name="id"/>.</summary>
    ValueTask RemoveAnchoredPanelAsync(string id);

    /// <summary>
    /// Promotes <paramref name="panel"/> to the browser's top layer without positioning it, for a
    /// panel that computes its own coordinates. A browser without the popover API is left unchanged.
    /// </summary>
    /// <param name="panel">The popup element to raise.</param>
    ValueTask RaiseToTopLayerAsync(ElementReference panel);

    /// <summary>Returns <paramref name="panel"/> from the top layer to the normal painting order.</summary>
    /// <param name="panel">The popup element to lower.</param>
    ValueTask DropFromTopLayerAsync(ElementReference panel);

    /// <summary>
    /// Installs the page-wide delegated listeners that place tooltip bubbles when their trigger is
    /// hovered or focused. Idempotent, and cheap to call from every tooltip instance: the browser side
    /// installs one pair of listeners for the whole document however many tooltips ask for it.
    /// </summary>
    ValueTask InitFloatingTooltipsAsync();

    /// <summary>
    /// Scrolls the option element with the given id into view within its scroll container. Used to keep the
    /// keyboard-highlighted option visible as the active descendant moves.
    /// </summary>
    /// <param name="optionId">The id of the option element to reveal.</param>
    /// <param name="block">Vertical alignment: <c>nearest</c> (default), <c>center</c>, <c>start</c> or <c>end</c>.</param>
    ValueTask ScrollIntoViewAsync(string optionId, string block = "nearest");

    /// <summary>
    /// Registers a single unified dismissal handler for a popup: a document capture-phase
    /// <c>pointerdown</c> outside <paramref name="element"/> and a <c>focusout</c> that escapes it (tabbing
    /// away), both invoking <paramref name="method"/>. Replaces the blur-timer + outside-click pair with one
    /// reliable mechanism (no SignalR blur race).
    /// </summary>
    /// <param name="id">A stable id identifying this handler.</param>
    /// <param name="element">The widget root; interactions inside it are ignored, outside it dismiss.</param>
    /// <param name="dotNetRef">The component reference whose <paramref name="method"/> is invoked.</param>
    /// <param name="method">The <c>[JSInvokable]</c> method name to invoke on dismissal.</param>
    ValueTask RegisterDismissAsync<T>(string id, ElementReference element,
        DotNetObjectReference<T> dotNetRef, string method) where T : class;

    /// <summary>Removes the dismissal handler registered under <paramref name="id"/>.</summary>
    ValueTask RemoveDismissAsync(string id);
}
