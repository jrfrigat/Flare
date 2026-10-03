using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// Keeps a picker's anchored popup in step with its open state: while open the popup is placed under the field
/// and holds Tab inside (it is a modal dialog); on close both are undone and focus goes back to the toggle, or to
/// the field after Escape (TASK-134). One instance per picker; the date, date-time and time pickers share it.
/// </summary>
internal sealed class PickerPopup(IOverlayJsService overlay, string id)
{
    // Placement belongs to the shared anchored layer; this type adds only the modal focus handling.
    private readonly AnchoredLayer _layer = new();
    private bool _trapped;
    private bool _returnToField;

    private string TrapId => id + "-trap";

    /// <summary>Sends focus to the field rather than the toggle when the popup next closes (Escape).</summary>
    public void ReturnToField() => _returnToField = true;

    /// <summary>
    /// Brings placement and the focus trap in line with <paramref name="open"/>. Call from OnAfterRenderAsync.
    /// <paramref name="focusOnOpen"/> runs once the trap is set; on close focus goes to <paramref name="toggle"/>,
    /// or to <paramref name="field"/> after <see cref="ReturnToField"/> or when there is no toggle. Focus is put
    /// back explicitly: the trap's own restore may point at an element inside the popup that is gone now.
    /// </summary>
    public async Task SyncAsync(bool open, ElementReference anchor, ElementReference panel, AnchoredPanelOptions? options,
        ElementReference field, ElementReference? toggle, Func<Task>? focusOnOpen = null)
    {
        await _layer.SyncAsync(overlay, open, anchor, panel, options);

        if (open == _trapped) return;
        _trapped = open;
        try
        {
            if (open) await overlay.TrapFocusAsync(TrapId, panel);
            else await overlay.ReleaseFocusTrapAsync(TrapId);
        }
        catch (JSDisconnectedException) { }
        catch (JSException) { }

        if (open)
        {
            if (focusOnOpen is not null) await focusOnOpen();
            return;
        }
        var target = _returnToField || toggle is null ? field : toggle.Value;
        _returnToField = false;
        try { await target.FocusAsync(); } catch { /* best-effort */ }
    }

    /// <summary>Releases the trap and the placement of a popup still open when its picker goes away.</summary>
    public async ValueTask DisposeAsync()
    {
        if (_trapped)
        {
            try { await overlay.ReleaseFocusTrapAsync(TrapId); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        await _layer.ReleaseAsync(overlay);
    }
}
