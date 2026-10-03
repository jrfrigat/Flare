using System.Globalization;
using Flare.Components.Resources;

namespace Flare.Components;

// The calendar views of the date picker - the day grid's month, the month view's year and the year view's
// decade - numbered on the calendar the grid is drawn on (TASK-182): Persian years and months for fa-IR,
// 13 months in a Hebrew leap year, the Gregorian ones for Gregorian, Thai Buddhist and Japanese cultures.
public partial class FlareDatePicker
{
    private enum CalendarView { Day, Month, Year }

    private int _viewYear;
    private int _viewMonth;
    private CalendarView _calView = CalendarView.Day;

    private Calendar _gridCalendar => CalendarMath.GridCalendar(_culture);
    private CalendarMonth _view => CalendarMonth.Create(_gridCalendar, _viewYear, _viewMonth);
    private (int First, int Last) _years => CalendarMonth.Years(_gridCalendar);
    private int _decadeStart => (_viewYear / 12) * 12;
    private int _monthsInView => _gridCalendar.GetMonthsInYear(Math.Clamp(_viewYear, _years.First, _years.Last));

    private void ShowMonthOf(DateOnly day)
    {
        var month = CalendarMonth.Of(day, _gridCalendar);
        _viewYear = month.Year;
        _viewMonth = month.Month;
    }

    private void ShowMonth(CalendarMonth month)
    {
        _viewYear = month.Year;
        _viewMonth = month.Month;
    }

    // What the header button does next: it cycles day -> month -> year -> day.
    private string ViewSwitchHint => _calView switch
    {
        CalendarView.Day => FlareStrings.Picker_ChooseMonth,
        CalendarView.Month => FlareStrings.Picker_ChooseYear,
        _ => FlareStrings.Picker_ShowDays,
    };

    private string HeaderLabel => _calView switch
    {
        CalendarView.Month => CalendarMath.YearLabel(_viewYear, _culture),
        CalendarView.Year => $"{CalendarMath.YearLabel(Math.Max(_decadeStart, _years.First), _culture)}-" +
            $"{CalendarMath.YearLabel(Math.Min(_decadeStart + 11, _years.Last), _culture)}",
        _ => MonthLabel,
    };

    private string MonthLabel => CalendarMath.FormatSafe(_view.Start, "MMMM yyyy", _culture);

    // The short name of month m of the viewed year; a Hebrew leap year names Adar I and Adar II.
    private string MonthName(int month) => CalendarMath.FormatSafe(CalendarMonth.Create(_gridCalendar, _viewYear, month).Start, "MMM", _culture);

    private void CycleCalendarView()
    {
        _calView = _calView switch
        {
            CalendarView.Day => CalendarView.Month,
            CalendarView.Month => CalendarView.Year,
            _ => CalendarView.Day,
        };
    }

    // Navigation stays inside the days both the grid calendar and DateOnly can name: year 0 and 10000+ would
    // throw when the header or the grid builds a date from them (TASK-105), Um al-Qura ends in 2077.
    private bool CanPrev => _calView switch
    {
        CalendarView.Day => _view.Prev is not null,
        CalendarView.Month => _viewYear > _years.First,
        _ => _decadeStart > _years.First,
    };

    private bool CanNext => _calView switch
    {
        CalendarView.Day => _view.Next is not null,
        CalendarView.Month => _viewYear < _years.Last,
        _ => _decadeStart + 12 <= _years.Last,
    };

    private void PrevView()
    {
        if (!CanPrev) return;
        switch (_calView)
        {
            case CalendarView.Day: ShowMonth(_view.Prev!.Value); break;
            case CalendarView.Month: ShowMonth(CalendarMonth.Create(_gridCalendar, _viewYear - 1, _viewMonth)); break;
            case CalendarView.Year: ShowMonth(CalendarMonth.Create(_gridCalendar, Math.Max(_viewYear - 12, _years.First), _viewMonth)); break;
        }
    }

    private void NextView()
    {
        if (!CanNext) return;
        switch (_calView)
        {
            case CalendarView.Day: ShowMonth(_view.Next!.Value); break;
            case CalendarView.Month: ShowMonth(CalendarMonth.Create(_gridCalendar, _viewYear + 1, _viewMonth)); break;
            case CalendarView.Year: ShowMonth(CalendarMonth.Create(_gridCalendar, Math.Min(_viewYear + 12, _years.Last), _viewMonth)); break;
        }
    }

    // A month or a year wholly outside [Min;Max] - or outside the calendar's own range - cannot be picked from
    // the month and year views (TASK-176).
    private bool MonthUnavailable(int month)
    {
        if (Disabled) return true;
        var m = CalendarMonth.Create(_gridCalendar, _viewYear, month);
        if (m.Year != _viewYear || m.Month != month) return true;
        return (Min is { } min && m.End < min) || (Max is { } max && m.Start > max);
    }

    private bool YearUnavailable(int year)
    {
        if (Disabled || year < _years.First || year > _years.Last) return true;
        var first = CalendarMonth.Create(_gridCalendar, year, 1);
        var last = CalendarMonth.Create(_gridCalendar, year, _gridCalendar.GetMonthsInYear(year));
        return (Min is { } min && last.End < min) || (Max is { } max && first.Start > max);
    }

    private void SelectMonth(int m)
    {
        if (MonthUnavailable(m)) return;
        _viewMonth = m;
        _calView = CalendarView.Day;
    }

    private void SelectYear(int y)
    {
        if (YearUnavailable(y)) return;
        ShowMonth(CalendarMonth.Create(_gridCalendar, y, _viewMonth));
        _calView = CalendarView.Month;
    }

    private void GoToToday()
    {
        ShowMonthOf(Today);
        _calView = CalendarView.Day;
    }
}
