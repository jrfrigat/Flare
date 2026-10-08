using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// The calendar views of the date picker - the day grid's month, the month view's year and the year view's decade -
// kept by the shared PickerViews (TASK-183) on the calendar the grid is drawn on (TASK-182).
public partial class FlareDatePicker
{
    private PickerViews? _viewState;
    private FlareMonthYearGrid? _viewGrid;
    private ElementReference _viewHeaderEl;
    private bool _focusPickedView;
    private PickerViews _views => _viewState ??= new PickerViews(() => _culture, () => Min, () => Max, () => Disabled);

    private PickerView _calView { get => _views.View; set => _views.View = value; }
    private int _viewYear => _views.Year;
    private int _viewMonth => _views.Month;
    private Calendar _gridCalendar => _views.Grid;
    private CalendarMonth _view => _views.Shown;

    private void ShowMonthOf(DateOnly day) => _views.ShowMonthOf(day);
    private string ViewSwitchHint => _views.SwitchHint;
    private string HeaderLabel => _views.HeaderLabel;
    private void CycleCalendarView() => _views.Cycle();
    private bool CanPrev => _views.CanPrev;
    private bool CanNext => _views.CanNext;
    private void PrevView() => _views.Prev();
    private void NextView() => _views.Next();
    private void PickInView(int value)
    {
        var before = _calView;
        _views.Pick(value);
        _focusPickedView = _calView != before;
    }

    private async Task FocusPickedViewAsync()
    {
        if (!_focusPickedView) return;
        _focusPickedView = false;
        if (Disabled || (!Inline && !_open)) return;
        if (_calView != PickerView.Day)
        {
            if (_viewGrid is not null) await _viewGrid.FocusAsync();
        }
        else if (_grid?.HasFocusableDay == true) await _grid.FocusCursorAsync();
        else await _viewHeaderEl.FocusAsync();
    }
    private void HandleViewKeyDown(KeyboardEventArgs e) => _views.PageKey(e);
    private void GoToToday() => _views.GoTo(Today);
}
