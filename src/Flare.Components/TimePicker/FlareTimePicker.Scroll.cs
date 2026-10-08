using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

public partial class FlareTimePicker
{
    private int _columnGeneration;
    private bool _columnsBound;
    private ElementReference _boundColumns;
    private DotNetObjectReference<ColumnReceiver>? _columnReceiver;

    private sealed class ColumnReceiver(FlareTimePicker owner)
    {
        [JSInvokable]
        public Task SelectColumn(int column, int value, int generation) =>
            owner.InvokeAsync(() => owner.SelectScrolledColumn(column, value, generation));
    }

    // The browser sends a proposed cell, never a time. Re-check against the current render's constraints.
    internal void SelectScrolledColumn(int column, int value, int generation)
    {
        if (!_open || _locked || PopupVariant != TimePickerVariant.Dropdown || generation != _columnGeneration
            || column < 0 || column > _lastColumn) return;
        var previous = _dropActive;
        _dropActive = column;
        if (!ColumnCells().Contains(value)) { _dropActive = previous; return; }
        _dropBuf = string.Empty;
        SetColumn(value);
        StateHasChanged();
    }

    private async Task SyncColumnsAsync()
    {
        if (!_open || _locked || PopupVariant != TimePickerVariant.Dropdown)
        {
            await ReleaseColumnsAsync();
            return;
        }
        _columnReceiver ??= DotNetObjectReference.Create(new ColumnReceiver(this));
        _boundColumns = _dropRef;
        _columnsBound = true;
        try { await ElementJs.SyncTimeColumnsAsync(_dropRef, _columnReceiver, _columnGeneration); }
        catch (JSException) { } // Older PWA modules keep the clickable columns usable.
        catch (InvalidOperationException) { } // Prerender has no browser.
    }

    private async Task ReleaseColumnsAsync()
    {
        if (!_columnsBound) return;
        _columnGeneration++;
        _columnsBound = false;
        try { await ElementJs.ReleaseTimeColumnsAsync(_boundColumns); }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }
}
