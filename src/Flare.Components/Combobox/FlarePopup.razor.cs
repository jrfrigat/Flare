using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// The anchored dropdown panel for the select family (see the markup partial for the rationale). Owns the
/// fixed-position anchoring and the single unified dismissal handler through <see cref="IOverlayJsService"/>,
/// so no shell re-implements the open/position/dismiss lifecycle or a blur timer.
/// </summary>
public partial class FlarePopup
{
    [Inject] private IOverlayJsService Overlay { get; set; } = default!;

    /// <inheritdoc />
    protected override string ComponentCssClass => string.Empty;

    /// <summary>Whether the panel is open (rendered + positioned).</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>The element the panel is positioned under (the trigger).</summary>
    [Parameter, EditorRequired] public ElementReference Anchor { get; set; }

    /// <summary>The widget root used for dismissal containment (interactions inside it do not dismiss).</summary>
    [Parameter, EditorRequired] public ElementReference DismissRoot { get; set; }

    /// <summary>Invoked when a pointer-down outside the widget or a focus-out escaping it should dismiss.</summary>
    [Parameter] public EventCallback OnDismiss { get; set; }

    /// <summary>
    /// Keeps the panel at least as wide as the anchor (default true). It still grows past that when
    /// an option needs the room, since a list that clipped its own values would hide the very thing
    /// being chosen; it never grows past the viewport.
    /// </summary>
    [Parameter] public bool MatchWidth { get; set; } = true;

    // The panel CSS class is the inherited FlareComponentBase.Class.

    /// <summary>The panel content (typically a <c>FlareOptionList</c>).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private ElementReference _panel;
    private readonly string _id = $"flare-popup-{Guid.NewGuid():N}";
    // Placement is the shared anchored layer's; this component adds only the dismissal around it.
    private readonly AnchoredLayer _layer = new();
    private DotNetObjectReference<FlarePopup>? _selfRef;
    private bool _registered;
    private Task? _pendingSync;
    private bool _requestedOpen;
    private long _syncGeneration;
    private bool _disposed;

    /// <summary>The panel element, for callers that need to measure or focus it.</summary>
    public ElementReference Panel => _panel;

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed) return Task.CompletedTask;
        var open = Open;
        if (_pendingSync is not { IsCompleted: false } && open == _requestedOpen && open == _registered)
            return Task.CompletedTask;
        if (open != _requestedOpen) _syncGeneration++;
        _requestedOpen = open;
        var generation = _syncGeneration;
        // Focus and parameter changes can render again while placement or dismissal is awaiting JS.
        // Queue the complete lifecycle, keeping the element belonging to each opening.
        return _pendingSync = _pendingSync is { IsCompleted: false } pending
            ? SyncAfterAsync(pending, generation, open, Anchor, _panel, DismissRoot, MatchWidth)
            : SyncCoreAsync(generation, open, Anchor, _panel, DismissRoot, MatchWidth);
    }

    private async Task SyncAfterAsync(Task pending, long generation, bool open, ElementReference anchor,
        ElementReference panel, ElementReference dismissRoot, bool matchWidth)
    {
        await pending;
        await SyncCoreAsync(generation, open, anchor, panel, dismissRoot, matchWidth);
    }

    private async Task SyncCoreAsync(long generation, bool open, ElementReference anchor,
        ElementReference panel, ElementReference dismissRoot, bool matchWidth)
    {
        // A queued close must release the old element even when a newer opening is already requested.
        if (_disposed || (open && generation != _syncGeneration)) return;
        if (open == _registered && open == _layer.Placed) return;
        await _layer.SyncAsync(Overlay, open, anchor, panel, new AnchoredPanelOptions { MatchWidth = matchWidth });
        if (_disposed || (open && generation != _syncGeneration) || open == _registered) return;
        try
        {
            if (open)
            {
                _selfRef ??= DotNetObjectReference.Create(this);
                await Overlay.RegisterDismissAsync(_id, dismissRoot, _selfRef, nameof(DismissFromJs));
                _registered = true;
            }
            else
            {
                await Overlay.RemoveDismissAsync(_id);
                _registered = false;
            }
        }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    /// <summary>Scrolls the option element with <paramref name="optionId"/> into view within the panel.</summary>
    /// <param name="optionId">The option element id to reveal.</param>
    public async ValueTask ScrollOptionIntoViewAsync(string optionId)
    {
        try { await Overlay.ScrollIntoViewAsync(optionId); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    /// <summary>Invoked from JS when a pointer-down outside the widget or a focus-out escaping it occurs.</summary>
    [JSInvokable]
    public Task DismissFromJs() => OnDismiss.InvokeAsync();

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        if (_pendingSync is { IsCompleted: false } pending) await pending;
        if (_registered)
        {
            try { await Overlay.RemoveDismissAsync(_id); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        await _layer.ReleaseAsync(Overlay);
        _selfRef?.Dispose();
        await base.DisposeAsync();
    }
}
