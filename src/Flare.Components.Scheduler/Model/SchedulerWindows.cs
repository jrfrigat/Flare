using System.Globalization;

namespace Flare.Components.Scheduler;

/// <summary>Gregorian navigation windows shared by built-in and custom scheduler views.</summary>
public static class SchedulerWindows
{
    /// <summary>Returns the half-open date window to request from a provider.</summary>
    /// <param name="date">Navigation anchor.</param>
    /// <param name="view">Projection.</param>
    /// <param name="culture">First day of the week.</param>
    /// <returns>Visible date bounds.</returns>
    public static SchedulerDateRange Dates(DateOnly date, SchedulerView view, CultureInfo culture)
    {
        var first = new DateOnly(date.Year, date.Month, 1);
        var week = date.AddDays(-((7 + (int)date.DayOfWeek - (int)culture.DateTimeFormat.FirstDayOfWeek) % 7));
        return view switch
        {
            SchedulerView.Day => new(date, date.AddDays(1)),
            SchedulerView.Week or SchedulerView.WorkWeek or SchedulerView.Timeline => new(week, week.AddDays(7)),
            SchedulerView.Month => new(CalendarGrid.Days(first, culture.DateTimeFormat.FirstDayOfWeek)[0],
                CalendarGrid.Days(first, culture.DateTimeFormat.FirstDayOfWeek)[^1].AddDays(1)),
            SchedulerView.Year => new(new(date.Year, 1, 1), new(date.Year + 1, 1, 1)),
            SchedulerView.Agenda => new(first, first.AddMonths(1)),
            _ => throw new ArgumentOutOfRangeException(nameof(view))
        };
    }
    /// <summary>Advances the navigation anchor by one view window.</summary>
    public static DateOnly Advance(DateOnly date, SchedulerView view, int direction) => view switch
    {
        SchedulerView.Day => date.AddDays(direction),
        SchedulerView.Week or SchedulerView.WorkWeek or SchedulerView.Timeline => date.AddDays(7 * direction),
        SchedulerView.Year => date.AddYears(direction),
        _ => date.AddMonths(direction)
    };
}
