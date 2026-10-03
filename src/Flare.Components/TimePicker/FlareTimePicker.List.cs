using System.Globalization;
using Flare.Components.Combobox;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// The List popup (TASK-131): the field is a combobox over a list of times, the time picker of design systems built on a combobox.
// The list is the select family's own popup and option list, so it looks like a FlareSelect dropdown in every
// theme; focus stays in the field and the active option is announced through aria-activedescendant.
public partial class FlareTimePicker
{
    private FlareFieldChrome? _chrome;
    private FlarePopup? _listPopup;
    private readonly string _listId = $"flare-timepicker-list-{Guid.NewGuid():N}";
    private int _listActive = -1;
    private bool _scrollListActive;
    private bool _focusInputOnOpen;
    private IReadOnlyList<ComboboxRow<TimeOnly>> _listRows = Array.Empty<ComboboxRow<TimeOnly>>();
    private (TimeOnly? Min, TimeOnly? Max, int Step)? _listKey;

    private bool _isList => PopupVariant == TimePickerVariant.List;
    private int _listStep => MinuteStep < 1 ? 1 : MinuteStep;
    private string? _listActiveId => _isList && _open && _listActive >= 0 ? $"{_listId}-opt-{_listActive}" : null;

    // Rebuilt only when the bounds or the step change: opening and moving the highlight reuse the rows.
    private IReadOnlyList<ComboboxRow<TimeOnly>> ListRows
    {
        get
        {
            var key = (Min, Max, _listStep);
            if (_listKey != key)
            {
                _listKey = key;
                _listRows = BuildListRows(Min, Max, _listStep);
            }
            return _listRows;
        }
    }

    // Times from Min (rounded up to the step) to Max, every step minutes of the day.
    private static ComboboxRow<TimeOnly>[] BuildListRows(TimeOnly? min, TimeOnly? max, int step)
    {
        var from = min is { } lo ? (int)Math.Ceiling(lo.ToTimeSpan().TotalMinutes / step) * step : 0;
        var rows = new List<ComboboxRow<TimeOnly>>();
        for (var m = from; m < 24 * 60; m += step)
        {
            var t = new TimeOnly(m / 60, m % 60);
            if (max is { } hi && t > hi) break;
            rows.Add(new ComboboxRow<TimeOnly>(false, null, t, rows.Count));
        }
        return rows.ToArray();
    }

    private string ListLabel(TimeOnly t) => t.ToString(_is24Hour ? "HH:mm" : "h:mm tt", CultureInfo.CurrentUICulture);

    private bool ListSelected(TimeOnly t) => Value is { } v && v.Hour == t.Hour && v.Minute == t.Minute;

    // The picked time is active on opening; without one, the first time not before the value, or the first time.
    private int InitialListActive()
    {
        var rows = ListRows;
        if (rows.Count == 0) return -1;
        if (Value is not { } v) return 0;
        for (var i = 0; i < rows.Count; i++)
            if (ListSelected(rows[i].Item) || rows[i].Item > v) return i;
        return rows.Count - 1;
    }

    private async Task ToggleListAsync()
    {
        if (_open) { await Close(); return; }
        _listActive = InitialListActive();
        _scrollListActive = true;
        _focusInputOnOpen = true;
        _open = true;
        await Opened.InvokeAsync();
        StateHasChanged();
    }

    // An option is an exact time; picking the one already selected leaves the value as it is, seconds included.
    private async Task SelectListAsync(TimeOnly t)
    {
        _open = false;
        await Closed.InvokeAsync();
        if (_locked || ListSelected(t)) { StateHasChanged(); return; }
        await Commit(t);
    }

    // Combobox keys in the field: the arrows open the list and move the active time, Enter picks it and
    // Escape closes the list. Digits still go into the field. Escape closes a list left open when the field was
    // locked after it opened; the other keys do nothing in a locked field (TASK-168, as TASK-162).
    private async Task HandleInputKeyDown(KeyboardEventArgs e)
    {
        if (!_isList) return;
        if (e.Key == "Escape" && _open) { await Close(); return; }
        if (Disabled || ReadOnly) return;
        switch (e.Key)
        {
            case "ArrowDown" or "ArrowUp" when !_open:
                await ToggleListAsync();
                break;
            case "ArrowDown":
                MoveListActive(1);
                break;
            case "ArrowUp":
                MoveListActive(-1);
                break;
            case "Enter" when _open && _listActive >= 0 && _listActive < ListRows.Count:
                await SelectListAsync(ListRows[_listActive].Item);
                break;
        }
    }

    private void MoveListActive(int delta)
    {
        var count = ListRows.Count;
        if (count == 0) return;
        _listActive = Math.Clamp(_listActive + delta, 0, count - 1);
        _scrollListActive = true;
    }

    // Placement and dismissal belong to FlarePopup; this keeps the active time in view and focus in the field.
    private async Task SyncListAsync()
    {
        if (!_open) return;
        if (_focusInputOnOpen)
        {
            _focusInputOnOpen = false;
            try { await _inputEl.FocusAsync(); } catch { /* best-effort */ }
        }
        if (_scrollListActive && _listPopup is not null && _listActiveId is { } id)
        {
            _scrollListActive = false;
            await _listPopup.ScrollOptionIntoViewAsync(id);
        }
    }
}
