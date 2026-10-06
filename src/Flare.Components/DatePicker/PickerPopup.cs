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
    private bool _dismissRegistered;
    private Func<Task>? _dismiss;
    private DotNetObjectReference<PickerPopup>? _selfRef;
    private Task? _pendingSync;
    private bool _requestedOpen;
    private bool _disposed;

    private string TrapId => id + "-trap";

    /// <summary>Sends focus to the field rather than the toggle when the popup next closes (Escape).</summary>
    public void ReturnToField() => _returnToField = true;

    /// <summary>
    /// Whether the popup was opened by a click in the field: it is then not modal - no focus trap, the field keeps
    /// focus and the user can go on typing - until <see cref="EnterAsync"/> moves focus into it.
    /// </summary>
    public bool FromField { get; private set; }

    /// <summary>Marks the next open as coming from the field (see <see cref="FromField"/>).</summary>
    public void OpenFromField() => FromField = true;

    /// <summary>Makes a popup opened from the field modal and puts focus inside it, as a toggle-opened one has.</summary>
    /// <param name="panel">The popup panel.</param>
    /// <param name="focus">Moves focus to the popup's starting element.</param>
    public async Task EnterAsync(ElementReference panel, Func<Task> focus)
    {
        if (_pendingSync is { IsCompleted: false } pending) await pending;
        if (_disposed || !_requestedOpen) return;
        if (!FromField) return;
        FromField = false;
        await RemoveDismissAsync();
        _trapped = true;
        try { await overlay.TrapFocusAsync(TrapId, panel); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        if (!_disposed && _requestedOpen) await focus();
    }

    /// <summary>
    /// Brings placement and the focus trap in line with <paramref name="open"/>. Call from OnAfterRenderAsync.
    /// <paramref name="focusOnOpen"/> runs once the trap is set; on close focus goes to <paramref name="toggle"/>,
    /// or to <paramref name="field"/> after <see cref="ReturnToField"/> or when there is no toggle. Focus is put
    /// back explicitly: the trap's own restore may point at an element inside the popup that is gone now.
    /// </summary>
    public Task SyncAsync(bool open, ElementReference anchor, ElementReference panel, AnchoredPanelOptions? options,
        ElementReference field, ElementReference? toggle, Func<Task>? focusOnOpen = null,
        ElementReference dismissRoot = default, Func<Task>? dismiss = null)
    {
        if (_disposed) return Task.CompletedTask;
        _requestedOpen = open;
        if (!open) FromField = false;
        // A render can arrive while placement is awaiting JS. Queue it behind the entire popup sync,
        // so placement, dismissal and focus cannot be installed twice or finish after a newer close.
        return _pendingSync = _pendingSync is { IsCompleted: false } pending
            ? SyncAfterAsync(pending, open, anchor, panel, options, field, toggle, focusOnOpen, dismissRoot, dismiss)
            : SyncCoreAsync(open, anchor, panel, options, field, toggle, focusOnOpen, dismissRoot, dismiss);
    }

    private async Task SyncAfterAsync(Task pending, bool open, ElementReference anchor, ElementReference panel,
        AnchoredPanelOptions? options, ElementReference field, ElementReference? toggle,
        Func<Task>? focusOnOpen, ElementReference dismissRoot, Func<Task>? dismiss)
    {
        await pending;
        await SyncCoreAsync(open, anchor, panel, options, field, toggle, focusOnOpen, dismissRoot, dismiss);
    }

    private async Task SyncCoreAsync(bool open, ElementReference anchor, ElementReference panel,
        AnchoredPanelOptions? options, ElementReference field, ElementReference? toggle,
        Func<Task>? focusOnOpen, ElementReference dismissRoot, Func<Task>? dismiss)
    {
        if (_disposed || open != _requestedOpen) return;
        await _layer.SyncAsync(overlay, open, anchor, panel, options);
        if (_disposed || open != _requestedOpen) return;

        _dismiss = dismiss;
        if (open && FromField && !_dismissRegistered && dismiss is not null)
        {
            try
            {
                _selfRef ??= DotNetObjectReference.Create(this);
                await overlay.RegisterDismissAsync(id + "-dismiss", dismissRoot, _selfRef, nameof(DismissFromJs));
                _dismissRegistered = true;
            }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        else if (!open || !FromField) await RemoveDismissAsync();

        // Opened from the field: placed, but focus stays where the user is typing. Closing it needs no focus move.
        if (open && FromField) return;
        if (!open) FromField = false;
        if (open == _trapped) return;
        _trapped = open;
        try
        {
            if (open) await overlay.TrapFocusAsync(TrapId, panel);
            else await overlay.ReleaseFocusTrapAsync(TrapId);
        }
        catch (JSDisconnectedException) { }
        catch (JSException) { }

        if (_disposed || open != _requestedOpen) return;
        if (open)
        {
            if (focusOnOpen is not null) await focusOnOpen();
            return;
        }
        var target = _returnToField || toggle is null ? field : toggle.Value;
        _returnToField = false;
        try { await target.FocusAsync(); } catch { /* best-effort */ }
    }

    /// <summary>Closes the field-open popup when a pointer or focus leaves its owning widget.</summary>
    [JSInvokable]
    public Task DismissFromJs() => FromField ? _dismiss?.Invoke() ?? Task.CompletedTask : Task.CompletedTask;

    private async ValueTask RemoveDismissAsync()
    {
        if (!_dismissRegistered) return;
        try { await overlay.RemoveDismissAsync(id + "-dismiss"); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        _dismissRegistered = false;
    }

    /// <summary>Releases the trap and the placement of a popup still open when its picker goes away.</summary>
    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_pendingSync is { IsCompleted: false } pending) await pending;
        await RemoveDismissAsync();
        if (_trapped)
        {
            try { await overlay.ReleaseFocusTrapAsync(TrapId); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        await _layer.ReleaseAsync(overlay);
        _selfRef?.Dispose();
    }
}
