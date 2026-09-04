using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// The open/close half of an anchored floating panel, for a component that owns its own markup.
///
/// Every popup in the library has the same lifecycle - position and raise it into the top layer when it
/// opens, release it when it closes or the component goes away - and the same three interop exceptions
/// to swallow. Written once here, a component adds a field and one call in
/// <c>OnAfterRenderAsync</c> instead of fifteen lines that have to agree with fourteen other copies.
///
/// It deliberately does NOT own the elements. An <see cref="ElementReference"/> captured by a parent and
/// passed down as a parameter is a render behind, so the panel has to be declared by the component that
/// shows it; only the lifecycle is shared.
/// </summary>
internal sealed class AnchoredLayer
{
    private readonly string _id = $"flare-layer-{Guid.NewGuid():N}";
    private bool _placed;

    /// <summary>Whether the panel is currently positioned and held in the top layer.</summary>
    public bool Placed => _placed;

    /// <summary>
    /// Brings the layer in line with <paramref name="open"/>: positions and raises the panel on the
    /// render that opened it, releases it on the one that closed it, and does nothing in between.
    /// </summary>
    /// <param name="overlay">The overlay interop service.</param>
    /// <param name="open">Whether the panel is showing.</param>
    /// <param name="anchor">The element the panel is placed against.</param>
    /// <param name="panel">The panel element.</param>
    /// <param name="options">Placement options; the defaults place the panel below the anchor.</param>
    public ValueTask SyncAsync(IOverlayJsService overlay, bool open,
        ElementReference anchor, ElementReference panel, AnchoredPanelOptions? options = null)
        => SyncCoreAsync(overlay, open, panel,
            (o, p) => o.PositionAnchoredPanelAsync(_id, anchor, p, options));

    /// <summary>
    /// Same, with the anchor named by its DOM id - for a panel whose anchor is one of many rendered in
    /// a loop, where a captured reference holds whichever one rendered last.
    /// </summary>
    /// <param name="overlay">The overlay interop service.</param>
    /// <param name="open">Whether the panel is showing.</param>
    /// <param name="anchorElementId">The <c>id</c> attribute of the anchor element.</param>
    /// <param name="panel">The panel element.</param>
    /// <param name="options">Placement options; the defaults place the panel below the anchor.</param>
    public ValueTask SyncByIdAsync(IOverlayJsService overlay, bool open,
        string anchorElementId, ElementReference panel, AnchoredPanelOptions? options = null)
        => SyncCoreAsync(overlay, open, panel,
            (o, p) => o.PositionAnchoredPanelByIdAsync(_id, anchorElementId, p, options));

    /// <summary>
    /// Places the panel again even though it is already placed, for a panel that follows something that
    /// moves - a completion list on a caret. Only call it when the anchor has actually moved: the
    /// browser side re-places without leaving the top layer, but it is still a round trip.
    /// </summary>
    /// <param name="overlay">The overlay interop service.</param>
    /// <param name="anchor">The element the panel is placed against.</param>
    /// <param name="panel">The panel element.</param>
    /// <param name="options">Placement options.</param>
    public async ValueTask ForceAsync(IOverlayJsService overlay,
        ElementReference anchor, ElementReference panel, AnchoredPanelOptions? options = null)
    {
        try
        {
            await overlay.PositionAnchoredPanelAsync(_id, anchor, panel, options);
            _placed = true;
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    private async ValueTask SyncCoreAsync(IOverlayJsService overlay, bool open, ElementReference panel,
        Func<IOverlayJsService, ElementReference, ValueTask> place)
    {
        if (open == _placed) return;
        try
        {
            if (open)
            {
                await place(overlay, panel);
                _placed = true;
            }
            else
            {
                await overlay.RemoveAnchoredPanelAsync(_id);
                _placed = false;
            }
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    /// <summary>
    /// Releases the panel without waiting for a render, for a component being disposed. A panel whose
    /// element has already left the DOM leaves the top layer with it, so this only has to detach the
    /// listeners the placement kept alive.
    /// </summary>
    /// <param name="overlay">The overlay interop service.</param>
    public async ValueTask ReleaseAsync(IOverlayJsService overlay)
    {
        if (!_placed) return;
        _placed = false;
        try { await overlay.RemoveAnchoredPanelAsync(_id); }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }
}
