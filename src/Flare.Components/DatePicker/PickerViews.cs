using System.Globalization;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>The view a date picker's popup shows: the days of a month, the months of a year or the years of a decade.</summary>
internal enum PickerView { Day, Month, Year }

/// <summary>
/// The calendar views a date picker pages through (TASK-183, out of FlareDatePicker): the day grid's month, the
/// month view's year and the year view's decade, numbered on the calendar the grid is drawn on (TASK-182), with
/// the header label, the paging bounds and the month and year picks that respect Min/Max (TASK-176). Shared by
/// the date, multi-date and week pickers, so each keeps only its own value.
/// </summary>
internal sealed class PickerViews(Func<CultureInfo> culture, Func<DateOnly?> min, Func<DateOnly?> max, Func<bool> disabled)
{
    /// <summary>The view shown.</summary>
    public PickerView View { get; set; } = PickerView.Day;
    /// <summary>The year shown, on the grid calendar.</summary>
    public int Year { get; private set; }
    /// <summary>The month shown in the day view, on the grid calendar.</summary>
    public int Month { get; private set; } = 1;

    /// <summary>The calendar the grid is drawn on.</summary>
    public Calendar Grid => CalendarMath.GridCalendar(culture());
    /// <summary>The month the day view shows.</summary>
    public CalendarMonth Shown => CalendarMonth.Create(Grid, Year, Month);
    /// <summary>The view the shared month and year grid lays out when the month or the year view is shown.</summary>
    public MonthYearGridView GridView => View == PickerView.Month ? MonthYearGridView.Months : MonthYearGridView.Years;

    private (int First, int Last) Years => CalendarMonth.Years(Grid);
    private int Decade => Year / 12 * 12;

    /// <summary>Shows the month that holds <paramref name="day"/>, keeping the view.</summary>
    public void ShowMonthOf(DateOnly day) => Show(CalendarMonth.Of(day, Grid));

    private void Show(CalendarMonth month)
    {
        Year = month.Year;
        Month = month.Month;
    }

    /// <summary>What the header button switches to next: day -> month -> year -> day.</summary>
    public string SwitchHint => View switch
    {
        PickerView.Day => FlareStrings.Picker_ChooseMonth,
        PickerView.Month => FlareStrings.Picker_ChooseYear,
        _ => FlareStrings.Picker_ShowDays,
    };

    /// <summary>The header: the month and year, the year, or the decade.</summary>
    public string HeaderLabel => View switch
    {
        PickerView.Month => CalendarMath.YearLabel(Year, culture()),
        PickerView.Year => $"{CalendarMath.YearLabel(Math.Max(Decade, Years.First), culture())}-" +
            $"{CalendarMath.YearLabel(Math.Min(Decade + 11, Years.Last), culture())}",
        _ => MonthLabel,
    };

    /// <summary>The shown month and its year, as the culture writes them.</summary>
    public string MonthLabel => CalendarMath.FormatSafe(Shown.Start, "MMMM yyyy", culture());

    /// <summary>Switches day -> month -> year -> day.</summary>
    public void Cycle() => View = View switch
    {
        PickerView.Day => PickerView.Month,
        PickerView.Month => PickerView.Year,
        _ => PickerView.Day,
    };

    // Paging stays inside the days both the grid calendar and DateOnly can name: year 0 and 10000+ would throw
    // when the header or the grid builds a date from them (TASK-105), Um al-Qura ends in 2077.
    /// <summary>Whether there is a month, year or decade before the shown one.</summary>
    public bool CanPrev => View switch
    {
        PickerView.Day => Shown.Prev is not null,
        PickerView.Month => Year > Years.First,
        _ => Decade > Years.First,
    };

    /// <summary>Whether there is a month, year or decade after the shown one.</summary>
    public bool CanNext => View switch
    {
        PickerView.Day => Shown.Next is not null,
        PickerView.Month => Year < Years.Last,
        _ => Decade + 12 <= Years.Last,
    };

    /// <summary>Shows the previous month, year or decade.</summary>
    public void Prev()
    {
        if (!CanPrev) return;
        Show(View switch
        {
            PickerView.Day => Shown.Prev!.Value,
            PickerView.Month => CalendarMonth.Create(Grid, Year - 1, Month),
            _ => CalendarMonth.Create(Grid, Math.Max(Year - 12, Years.First), Month),
        });
    }

    /// <summary>Shows the next month, year or decade.</summary>
    public void Next()
    {
        if (!CanNext) return;
        Show(View switch
        {
            PickerView.Day => Shown.Next!.Value,
            PickerView.Month => CalendarMonth.Create(Grid, Year + 1, Month),
            _ => CalendarMonth.Create(Grid, Math.Min(Year + 12, Years.Last), Month),
        });
    }

    /// <summary>A pick in the month or the year view: a month opens its days, a year its months. A month or a year
    /// wholly outside [Min;Max], or outside the calendar's range, is refused (TASK-176).</summary>
    public void Pick(int value)
    {
        if (View == PickerView.Month)
        {
            if (disabled() || CalendarMonth.MonthUnavailable(Grid, Year, value, min(), max())) return;
            Month = value;
            View = PickerView.Day;
        }
        else
        {
            if (disabled() || CalendarMonth.YearUnavailable(Grid, value, min(), max())) return;
            Show(CalendarMonth.Create(Grid, value, Month));
            View = PickerView.Month;
        }
    }

    /// <summary>PageUp/PageDown in the month or year view page the year or the decade.</summary>
    public void PageKey(KeyboardEventArgs e)
    {
        if (e.Key == "PageUp") Prev();
        else if (e.Key == "PageDown") Next();
    }

    /// <summary>Shows the days of the month that holds <paramref name="day"/>.</summary>
    public void GoTo(DateOnly day)
    {
        ShowMonthOf(day);
        View = PickerView.Day;
    }
}
