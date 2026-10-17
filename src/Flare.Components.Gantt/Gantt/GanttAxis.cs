using System.Globalization;

namespace Flare.Components.Gantt;

/// <summary>One cell of the axis header.</summary>
/// <param name="Span">Time covered by the cell (clipped to the axis range).</param>
/// <param name="Offset">Left edge in axis units.</param>
/// <param name="Width">Width in axis units.</param>
/// <param name="Label">Main caption.</param>
/// <param name="Caption">Secondary caption, such as the day of week; may be null.</param>
public sealed record GanttTick(GanttSpan Span, double Offset, double Width, string Label, string? Caption);

/// <summary>
/// The time axis: converts moments to positions measured in axis units (one unit per step of
/// <see cref="Scale"/>) and lists the header cells. Months, quarters and years have unequal length, and each
/// still counts as exactly one unit, with a moment inside it placed by the fraction of the unit elapsed.
/// </summary>
public sealed class GanttAxis
{
    /// <summary>Creates an axis over <paramref name="range"/>.</summary>
    public GanttAxis(GanttSpan range, GanttScale scale, CultureInfo? culture = null)
    {
        if (range.End < range.Start) throw new ArgumentException("The axis range ends before it starts.", nameof(range));
        Range = range;
        Scale = scale;
        Culture = culture ?? CultureInfo.CurrentCulture;
        Units = Offset(range.End);
    }

    /// <summary>Visible time.</summary>
    public GanttSpan Range { get; }

    /// <summary>Step of one unit.</summary>
    public GanttScale Scale { get; }

    /// <summary>Culture for captions and the first day of week.</summary>
    public CultureInfo Culture { get; }

    /// <summary>Length of the range in units.</summary>
    public double Units { get; }

    /// <summary>Position of <paramref name="moment"/> in units from the start of the range; may be negative or past <see cref="Units"/>.</summary>
    public double Offset(DateTime moment) => Position(Scale, moment) - Position(Scale, Range.Start);

    /// <summary>The moment at <paramref name="offset"/> units from the start of the range.</summary>
    public DateTime At(double offset)
    {
        var target = Position(Scale, Range.Start) + offset;
        var index = Math.Floor(target);
        var unit = UnitAt(Scale, index);
        return unit.Start + TimeSpan.FromTicks((long)Math.Round((unit.End - unit.Start).Ticks * (target - index)));
    }

    /// <summary>The unit boundary nearest to <paramref name="moment"/>.</summary>
    public DateTime Snap(DateTime moment)
    {
        var unit = UnitOf(Scale, moment);
        return moment - unit.Start <= unit.End - moment ? unit.Start : unit.End;
    }

    /// <summary>The whole unit that contains <paramref name="moment"/>.</summary>
    public GanttSpan UnitOf(DateTime moment) => UnitOf(Scale, moment);

    /// <summary>Header cells, one per unit.</summary>
    public IReadOnlyList<GanttTick> Ticks() => Cells(Scale);

    /// <summary>Group header cells above the units (days above hours, months above days and weeks, years above months and quarters); empty for years.</summary>
    public IReadOnlyList<GanttTick> MajorTicks() => Major(Scale) is { } major ? Cells(major) : [];

    /// <summary>The group step drawn above <paramref name="scale"/>, or null.</summary>
    public static GanttScale? Major(GanttScale scale) => scale switch
    {
        GanttScale.Hour => GanttScale.Day,
        GanttScale.Day or GanttScale.Week => GanttScale.Month,
        GanttScale.Month or GanttScale.Quarter => GanttScale.Year,
        _ => null
    };

    private List<GanttTick> Cells(GanttScale scale)
    {
        var cells = new List<GanttTick>();
        if (Range.End <= Range.Start) return cells;
        for (var unit = UnitOf(scale, Range.Start); unit.Start < Range.End; unit = UnitOf(scale, unit.End))
        {
            var clipped = new GanttSpan(unit.Start < Range.Start ? Range.Start : unit.Start, unit.End > Range.End ? Range.End : unit.End);
            var (label, caption) = Caption(scale, unit.Start);
            cells.Add(new GanttTick(clipped, Offset(clipped.Start), Offset(clipped.End) - Offset(clipped.Start), label, caption));
        }
        return cells;
    }

    private (string Label, string? Caption) Caption(GanttScale scale, DateTime start)
    {
        var major = scale != Scale;
        return scale switch
        {
            GanttScale.Hour => (start.ToString("HH:mm", Culture), null),
            GanttScale.Day when major => (start.ToString("d MMM", Culture), Culture.DateTimeFormat.GetAbbreviatedDayName(start.DayOfWeek)),
            GanttScale.Day => (start.Day.ToString(Culture), Culture.DateTimeFormat.GetAbbreviatedDayName(start.DayOfWeek)),
            GanttScale.Week => (start.ToString("dd.MM", Culture), null),
            GanttScale.Month when major => (Capitalize(start.ToString("MMMM yyyy", Culture)), null),
            GanttScale.Month => (Capitalize(Culture.DateTimeFormat.GetAbbreviatedMonthName(start.Month).TrimEnd('.')), null),
            GanttScale.Quarter => ($"Q{(start.Month - 1) / 3 + 1}", null),
            _ => (start.Year.ToString(Culture), null)
        };
    }

    private string Capitalize(string text) => text.Length == 0 ? text : char.ToUpper(text[0], Culture) + text[1..];

    // Continuous position of a moment counted in units from a fixed origin.
    private double Position(GanttScale scale, DateTime moment)
    {
        var unit = UnitOf(scale, moment);
        var fraction = unit.End > unit.Start ? (moment - unit.Start).Ticks / (double)(unit.End - unit.Start).Ticks : 0;
        return Index(scale, unit.Start) + fraction;
    }

    private double Index(GanttScale scale, DateTime unitStart) => scale switch
    {
        GanttScale.Hour => Math.Floor((unitStart - DateTime.MinValue).TotalHours),
        GanttScale.Day => unitStart.Date.Subtract(DateTime.MinValue).Days,
        GanttScale.Week => Math.Floor((unitStart - WeekOrigin).TotalDays / 7),
        GanttScale.Month => unitStart.Year * 12 + unitStart.Month - 1,
        GanttScale.Quarter => unitStart.Year * 4 + (unitStart.Month - 1) / 3,
        _ => unitStart.Year
    };

    private GanttSpan UnitAt(GanttScale scale, double index)
    {
        var i = (long)index;
        return scale switch
        {
            GanttScale.Hour => UnitOf(scale, DateTime.MinValue.AddHours(i)),
            GanttScale.Day => UnitOf(scale, DateTime.MinValue.AddDays(i)),
            GanttScale.Week => UnitOf(scale, WeekOrigin.AddDays(i * 7)),
            GanttScale.Month => UnitOf(scale, new DateTime((int)(i / 12), (int)(i % 12) + 1, 1)),
            GanttScale.Quarter => UnitOf(scale, new DateTime((int)(i / 4), (int)(i % 4) * 3 + 1, 1)),
            _ => UnitOf(scale, new DateTime((int)i, 1, 1))
        };
    }

    // First day of week on or after DateTime.MinValue, so that week indices count whole weeks.
    private DateTime WeekOrigin
    {
        get
        {
            var origin = DateTime.MinValue;
            while (origin.DayOfWeek != Culture.DateTimeFormat.FirstDayOfWeek) origin = origin.AddDays(1);
            return origin;
        }
    }

    private GanttSpan UnitOf(GanttScale scale, DateTime moment)
    {
        switch (scale)
        {
            case GanttScale.Hour:
                var hour = new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, 0, 0);
                return new(hour, hour.AddHours(1));
            case GanttScale.Day:
                return new(moment.Date, moment.Date.AddDays(1));
            case GanttScale.Week:
                var back = ((int)moment.DayOfWeek - (int)Culture.DateTimeFormat.FirstDayOfWeek + 7) % 7;
                var week = moment.Date.AddDays(-back);
                return new(week, week.AddDays(7));
            case GanttScale.Month:
                var month = new DateTime(moment.Year, moment.Month, 1);
                return new(month, month.AddMonths(1));
            case GanttScale.Quarter:
                var quarter = new DateTime(moment.Year, (moment.Month - 1) / 3 * 3 + 1, 1);
                return new(quarter, quarter.AddMonths(3));
            default:
                var year = new DateTime(moment.Year, 1, 1);
                return new(year, year.AddYears(1));
        }
    }
}
