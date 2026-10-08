using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

// Keyboard of the column popup (TASK-174): one listbox per unit. Left/Right pick the column, Up/Down step its value over
// the cells that are offered (the step and Min/Max), Home/End jump to the first and last, digits type it, and Enter
// confirms on key-up - on key-down the closing popup hands focus to the toggle, which the browser then activates.
public partial class FlareTimePicker
{
    private bool _scrollDropActive;
    private bool _dropEnterPressed;
    private int _lastColumn => ShowSeconds ? 2 : 1;

    private void OnDropKey(KeyboardEventArgs e)
    {
        var key = e.Key;
        if (key == "Enter") { _dropEnterPressed = true; return; }
        if (key.Length == 1 && key[0] is >= '0' and <= '9') { FeedColumn(key[0]); return; }
        switch (key)
        {
            case "ArrowLeft": _dropActive = Math.Max(0, _dropActive - 1); _dropBuf = string.Empty; break;
            case "ArrowRight": _dropActive = Math.Min(_lastColumn, _dropActive + 1); _dropBuf = string.Empty; break;
            case "ArrowDown": StepColumn(1); break;
            case "ArrowUp": StepColumn(-1); break;
            case "Home": JumpColumn(first: true); break;
            case "End": JumpColumn(first: false); break;
            case "Backspace": _dropBuf = string.Empty; break;
        }
    }

    private Task OnDropKeyUp(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return Task.CompletedTask;
        var confirm = _dropEnterPressed;
        _dropEnterPressed = false;
        // Opening on the toggle can move focus here before that key is released.
        return confirm ? Confirm() : Task.CompletedTask;
    }

    private void FeedColumn(char c)
    {
        var (v, buf, complete) = TimeKeyboardEntry.Feed(_dropBuf, c, _dropActive == 0 ? 23 : 59);
        _dropBuf = buf;
        SetColumn(v);
        if (!complete) return;
        _dropBuf = string.Empty;
        if (_dropActive < _lastColumn) _dropActive++;
    }

    private int ColumnValue => _dropActive switch { 0 => _tempHour, 1 => _tempMinute, _ => _tempSecond };

    private void SetColumn(int value)
    {
        switch (_dropActive)
        {
            case 0: _tempHour = value; break;
            case 1: _tempMinute = value; break;
            default: _tempSecond = value; break;
        }
        _scrollDropActive = true;
    }

    // The cells the active column offers, in order.
    private List<int> ColumnCells()
    {
        var (count, step, disabled) = _dropActive switch
        {
            0 => (24, _hourStep, (Func<int, bool>)HourDisabled),
            1 => (60, MinuteStep, MinuteDisabled),
            _ => (60, _secondStep, SecondDisabled),
        };
        var cells = new List<int>();
        for (var i = 0; i < count; i += step)
            if (!disabled(i)) cells.Add(i);
        return cells;
    }

    private void StepColumn(int direction)
    {
        _dropBuf = string.Empty;
        var current = ColumnValue;
        var cells = ColumnCells();
        var next = direction > 0 ? cells.FindIndex(c => c > current) : cells.FindLastIndex(c => c < current);
        if (next >= 0) SetColumn(cells[next]);
    }

    private void JumpColumn(bool first)
    {
        _dropBuf = string.Empty;
        var cells = ColumnCells();
        if (cells.Count > 0) SetColumn(first ? cells[0] : cells[^1]);
    }

    // Keeps the active cell in view after the keyboard moved it.
    private async Task ScrollDropActiveAsync()
    {
        if (!_scrollDropActive) return;
        _scrollDropActive = false;
        if (_columnsBound) return; // The scroll picker centers the selected cell after render.
        if (_columnsActiveId is not { } id) return;
        try { await Overlay.ScrollIntoViewAsync(id); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }
}
