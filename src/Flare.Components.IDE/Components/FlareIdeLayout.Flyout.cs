using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

public partial class FlareIdeLayout
{
    // One registry id per layout instance, so two layouts on a page do not replace each other's
    // flyout registrations.
    private readonly string _flyoutId = Guid.NewGuid().ToString("N");
    private IJSObjectReference? _flyoutModule;
    private DotNetObjectReference<FlareIdeLayout>? _flyoutReference;

    // Elements the JS registry needs to tell an inside press from an outside one.
    private readonly Dictionary<ToolPanelPosition, ElementReference> _panelBodies = new();
    private readonly Dictionary<ToolPanelPosition, ElementReference> _toolRails = new();
    private readonly Dictionary<ToolPanelPosition, ElementReference> _regionRails = new();
    private readonly Dictionary<FlareToolPanel, ElementReference> _railEntries = new();

    // What was last sent to the module, so a render only updates a changed element or state.
    private readonly Dictionary<ToolPanelPosition, (string? BodyId, string? RailId)> _flyoutSynced = new();

    // Set when Escape collapsed a flyout; the next render focuses that tool's rail entry.
    private (ToolPanelPosition Position, string? Key)? _pendingRailFocus;

    private static readonly ToolPanelPosition[] SidePositions =
        [ToolPanelPosition.Left, ToolPanelPosition.Right];

    // Registers the open flyouts with the module and removes the ones that closed. Listeners exist
    // in the module only while at least one side is registered.
    private async Task SyncFlyoutDismissalAsync()
    {
        try
        {
            foreach (var position in SidePositions)
            {
                if (IsFloating(position))
                {
                    if (!_panelBodies.TryGetValue(position, out var body) || body.Id is null)
                        continue;
                    _toolRails.TryGetValue(position, out var rail);
                    var railId = rail.Id;
                    if (_flyoutSynced.TryGetValue(position, out var sent) &&
                        sent.BodyId == body.Id && sent.RailId == railId)
                        continue;

                    _flyoutModule ??= await Js.InvokeAsync<IJSObjectReference>("import", IdeInterop.ModulePath);
                    _flyoutReference ??= DotNetObjectReference.Create(this);
                    if (railId is null)
                    {
                        await _flyoutModule.InvokeVoidAsync("setFlyout", _flyoutId, _flyoutReference,
                            position.ToString(), body);
                    }
                    else
                    {
                        await _flyoutModule.InvokeVoidAsync("setFlyout", _flyoutId, _flyoutReference,
                            position.ToString(), body, rail);
                    }
                    _flyoutSynced[position] = (body.Id, railId);
                }
                else if (_flyoutSynced.Remove(position) && _flyoutModule is not null)
                {
                    await _flyoutModule.InvokeVoidAsync("clearFlyout", _flyoutId, position.ToString());
                }
            }

            if (_flyoutSynced.Count == 0 && _flyoutModule is not null)
            {
                // Nothing is open, so this layout has no module state left; a later flyout imports
                // the same module again.
                var module = _flyoutModule;
                _flyoutModule = null;
                try { await module.DisposeAsync(); }
                catch (JSDisconnectedException) { }
                catch (JSException) { }
            }
        }
        catch (JSDisconnectedException) { ResetFlyoutSync(); }
        catch (JSException) { ResetFlyoutSync(); }
        catch (InvalidOperationException) { ResetFlyoutSync(); }
    }

    // A runtime that went away dropped the JS listeners; the next render registers them again.
    private void ResetFlyoutSync()
    {
        _flyoutModule = null;
        _flyoutSynced.Clear();
    }

    private async Task FocusPendingRailAsync()
    {
        if (_pendingRailFocus is not { } focus) return;
        _pendingRailFocus = null;

        ElementReference? match = null;
        ElementReference? first = null;
        foreach (var registration in _panels)
        {
            if (registration.Position != focus.Position || registration.Panel.Group is null)
                continue;
            if (!_railEntries.TryGetValue(registration.Panel, out var entry) || entry.Id is null)
                continue;
            first ??= entry;
            if (focus.Key is not null &&
                string.Equals(registration.Key, focus.Key, StringComparison.Ordinal))
            {
                match = entry;
                break;
            }
        }

        var target = match ?? first;
        if (target is null && _regionRails.TryGetValue(focus.Position, out var railButton) &&
            railButton.Id is not null)
            target = railButton;
        if (target is not { } element) return;

        try { await element.FocusAsync(); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    /// <summary>Called by the IDE's JS module when a pointer press lands outside an open auto-hide
    /// flyout and outside its rail. Collapses the region through
    /// <see cref="LeftCollapsed"/> or <see cref="RightCollapsed"/>; a region the host already
    /// collapsed, and a docked region, are left alone.</summary>
    /// <param name="position">The side whose flyout is open: <c>Left</c> or <c>Right</c>.</param>
    [JSInvokable]
    public async Task OnFlyoutOutsidePressAsync(string position)
    {
        if (!TryParseSide(position, out var side) || !IsFloating(side)) return;
        await SetCollapsedAsync(side, true);
    }

    /// <summary>Called by the IDE's JS module when Escape is pressed while an auto-hide flyout is
    /// open. Collapses the region through <see cref="LeftCollapsed"/> or
    /// <see cref="RightCollapsed"/> and returns focus to the active tool's rail entry on the next
    /// render; a region the host already collapsed, and a docked region, are left alone.</summary>
    /// <param name="position">The side whose flyout is open: <c>Left</c> or <c>Right</c>.</param>
    [JSInvokable]
    public async Task OnFlyoutEscapeAsync(string position)
    {
        if (!TryParseSide(position, out var side) || !IsFloating(side)) return;
        _pendingRailFocus = (side, ActiveToolKey(side));
        await SetCollapsedAsync(side, true);
    }

    private static bool TryParseSide(string position, out ToolPanelPosition side)
    {
        side = position switch
        {
            nameof(ToolPanelPosition.Left) => ToolPanelPosition.Left,
            nameof(ToolPanelPosition.Right) => ToolPanelPosition.Right,
            _ => ToolPanelPosition.Bottom,
        };
        return side != ToolPanelPosition.Bottom;
    }

    private async Task DisposeFlyoutAsync()
    {
        if (_flyoutModule is not null)
        {
            try { await _flyoutModule.InvokeVoidAsync("disposeFlyoutDismissal", _flyoutId); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            catch (ObjectDisposedException) { }
            try { await _flyoutModule.DisposeAsync(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            _flyoutModule = null;
        }
        _flyoutSynced.Clear();
        _flyoutReference?.Dispose();
        _flyoutReference = null;
    }
}
