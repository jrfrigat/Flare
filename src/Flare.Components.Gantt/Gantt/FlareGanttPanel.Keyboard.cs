using Flare.Components.Gantt;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

public partial class FlareGanttPanel<TRow, TItem>
{
    [Inject] private IUiJsService RowKeyboard { get; set; } = default!;
    private ElementReference _rowKeyboardRoot;
    private DotNetObjectReference<FlareGanttPanel<TRow, TItem>>? _rowKeyboardRef;
    private Task? _rowKeyboardRegistration;
    private bool _rowKeyboardRegistered;
    private bool _rowKeyboardDisposed;
    private string? _activeRowKey;
    private string? _rowFocusKey;
    private string? _rowFocusInFlight;
    private string? RowFocusOffset => _rowFocusKey is { } key
        && _rows.FirstOrDefault(row => row.Key == key) is { } row ? GanttFormat.Number(row.Top) : null;

    /// <summary>Navigates a panel's visible rows without taking keys from controls inside a row.</summary>
    /// <param name="index">Index in the flattened visible row list.</param>
    /// <param name="key">An arrow, Home, End or Space key.</param>
    [JSInvokable]
    public Task HandleTreeKeyAsync(int index, string key)
    {
        if (index < 0 || index >= _rows.Count) return Task.CompletedTask;
        var row = _rows[index];
        var next = index;
        switch (key)
        {
            case "ArrowUp": next = Math.Max(0, index - 1); break;
            case "ArrowDown": next = Math.Min(_rows.Count - 1, index + 1); break;
            case "Home": next = 0; break;
            case "End": next = _rows.Count - 1; break;
            case "ArrowRight":
                if (row.HasChildren && !row.Expanded) Toggle(row.Key);
                else if (index + 1 < _rows.Count && _rows[index + 1].Depth > row.Depth) next++;
                break;
            case "ArrowLeft":
                if (row.HasChildren && row.Expanded) Toggle(row.Key);
                else if (row.Depth > 0)
                    while (next > 0 && _rows[next].Depth >= row.Depth) next--;
                break;
            case " ": if (row.HasChildren) Toggle(row.Key); break;
            default: return Task.CompletedTask;
        }
        _activeRowKey = _rows[next].Key;
        _rowFocusKey = _activeRowKey;
        return InvokeAsync(StateHasChanged);
    }

    private async Task RegisterRowKeyboardAsync()
    {
        _rowKeyboardRef = DotNetObjectReference.Create(this);
        try
        {
            await RowKeyboard.RegisterTreeKeyboardAsync(_rowKeyboardRoot, _rowKeyboardRef);
            _rowKeyboardRegistered = true;
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    private async Task SyncRowKeyboardAsync()
    {
        if (_rowKeyboardDisposed) return;
        await (_rowKeyboardRegistration ??= RegisterRowKeyboardAsync());
        if (_rowFocusKey is not { } key || _rowFocusInFlight == key) return;
        var row = _rows.FirstOrDefault(candidate => candidate.Key == key);
        if (row is null) { _rowFocusKey = null; return; }
        _rowFocusInFlight = key;
        try { await RowKeyboard.FocusTreeItemAsync(_rowKeyboardRoot, row.Index); }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        finally
        {
            if (_rowFocusKey == key) _rowFocusKey = null;
            if (_rowFocusInFlight == key) _rowFocusInFlight = null;
        }
    }

    private async ValueTask DisposeRowKeyboardAsync()
    {
        _rowKeyboardDisposed = true;
        if (_rowKeyboardRegistration is not null) await _rowKeyboardRegistration;
        if (_rowKeyboardRegistered)
        {
            try { await RowKeyboard.RemoveTreeKeyboardAsync(_rowKeyboardRoot); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        _rowKeyboardRef?.Dispose();
    }
}
