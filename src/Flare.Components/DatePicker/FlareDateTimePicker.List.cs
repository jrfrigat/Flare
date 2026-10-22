using Flare.Components.Combobox;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

// The List time pane: the column of times the standalone picker shows as a combobox, rendered inside the
// combined popup instead of a separate dropdown. Choosing a time edits the draft; OK commits it, as every
// other pane of this picker does.
public partial class FlareDateTimePicker
{
    private IReadOnlyList<ComboboxRow<TimeOnly>> _timeRows = [];
    private int _timeStep;
    private int _timeActive = -1;

    // Rebuilt only when the step changes: opening, closing and moving the highlight reuse the rows.
    private IReadOnlyList<ComboboxRow<TimeOnly>> TimeRows
    {
        get
        {
            var step = MinuteStep < 1 ? 1 : MinuteStep;
            if (_timeStep != step)
            {
                _timeStep = step;
                var rows = new List<ComboboxRow<TimeOnly>>(24 * 60 / step);
                for (var minute = 0; minute < 24 * 60; minute += step)
                    rows.Add(new ComboboxRow<TimeOnly>(false, null, new TimeOnly(minute / 60, minute % 60), rows.Count));
                _timeRows = rows;
                _timeActive = Math.Clamp(_timeActive, -1, rows.Count - 1);
            }

            return _timeRows;
        }
    }

    private string TimeListId => $"{ControlId}-time-list";

    private string? TimeListActiveId =>
        TimeVariant == TimePickerVariant.List && _open && TimeActive >= 0
            ? $"{TimeListId}-opt-{TimeActive}"
            : null;

    // The row under the highlight: the position the user moved to, otherwise the draft's own time.
    private int TimeActive
    {
        get
        {
            var rows = TimeRows;
            if (_timeActive >= 0 && _timeActive < rows.Count) return _timeActive;
            for (var i = 0; i < rows.Count; i++)
                if (rows[i].Item.Hour == _hour && rows[i].Item.Minute == _minute)
                    return i;
            return rows.Count == 0 ? -1 : 0;
        }
    }

    private bool IsListTimeSelected(TimeOnly time) => time.Hour == _hour && time.Minute == _minute;

    private string ListTimeLabel(TimeOnly time) =>
        time.ToString(_is24Hour ? "HH:mm" : "h:mm tt", _culture);

    private void SelectListTime(TimeOnly time)
    {
        // Seconds are not a row of the list; a value that has them keeps them, as the standalone list does.
        _hour = time.Hour;
        _minute = time.Minute;
        _timeActive = -1;
    }

    // Keys move the highlight, Enter takes it. The listbox itself is the focus target, so the calendar's
    // arrow keys never reach this handler.
    private async Task HandleTimeListKeyDown(KeyboardEventArgs e)
    {
        var rows = TimeRows;
        if (rows.Count == 0) return;

        var active = TimeActive;
        int? target = e.Key switch
        {
            "ArrowDown" => Math.Min(active + 1, rows.Count - 1),
            "ArrowUp" => Math.Max(active - 1, 0),
            "Home" => 0,
            "End" => rows.Count - 1,
            "PageDown" => Math.Min(active + 10, rows.Count - 1),
            "PageUp" => Math.Max(active - 10, 0),
            _ => null,
        };
        if (target is { } index)
        {
            _timeActive = index;
            await ScrollTimeIntoViewAsync(index);
            return;
        }

        if (e.Key is "Enter" or " " && active >= 0)
            SelectListTime(rows[active].Item);
    }

    // A long list scrolls; the option listbox is the scroll container, so the moved highlight has to be
    // brought into view explicitly.
    private async ValueTask ScrollTimeIntoViewAsync(int index)
    {
        try { await Overlay.ScrollIntoViewAsync($"{TimeListId}-opt-{index}"); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }
}
