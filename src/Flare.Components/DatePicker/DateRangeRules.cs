namespace Flare.Components;

/// <summary>
/// Length and blackout rules of a date range pick, shared by the range calendar, its fields and its presets:
/// an inclusive length of <paramref name="MinDays"/>..<paramref name="MaxDays"/> days and, unless
/// <paramref name="AllowDisabledInside"/>, no day <paramref name="IsDisabled"/> rejects between the ends.
/// </summary>
internal readonly record struct DateRangeRules(int? MinDays, int? MaxDays, bool AllowDisabledInside,
    Func<DateOnly, bool>? IsDisabled)
{
    // How far Reach looks for a blocking day when no MaxDays bounds it; a longer pick is still judged by Allows.
    private const int SearchLimit = 3660;

    private int MinLength => MinDays is > 1 ? MinDays.Value : 1;
    private int? MaxLength => MaxDays is >= 1 ? MaxDays : null;
    private bool ChecksInside => !AllowDisabledInside && IsDisabled is not null;

    /// <summary>True when <paramref name="a"/>..<paramref name="b"/> (either order) is a range the rules accept.</summary>
    public bool Allows(DateOnly a, DateOnly b)
    {
        var (lo, hi) = a <= b ? (a.DayNumber, b.DayNumber) : (b.DayNumber, a.DayNumber);
        var length = hi - lo + 1;
        if (length < MinLength || (MaxLength is { } max && length > max)) return false;
        if (!ChecksInside) return true;
        for (var n = lo + 1; n < hi; n++)
            if (IsDisabled!(DateOnly.FromDayNumber(n))) return false;
        return true;
    }

    /// <summary>True when <paramref name="end"/> may close a range started at <paramref name="anchor"/>, judged from
    /// the precomputed <paramref name="reach"/> of that anchor (see <see cref="Reach"/>).</summary>
    public bool CanEnd(DateOnly anchor, DateOnly end, (DateOnly? Lo, DateOnly? Hi) reach)
    {
        var length = Math.Abs(end.DayNumber - anchor.DayNumber) + 1;
        if (length < MinLength) return false;
        return (reach.Lo is not { } lo || end >= lo) && (reach.Hi is not { } hi || end <= hi);
    }

    /// <summary>The farthest day on each side of <paramref name="anchor"/> the other end may take (null = no limit
    /// found): MaxDays, and the first rejected day when disabled days may not lie inside.</summary>
    public (DateOnly? Lo, DateOnly? Hi) Reach(DateOnly anchor) => (Far(anchor, -1), Far(anchor, 1));

    private DateOnly? Far(DateOnly anchor, int direction)
    {
        var steps = MaxLength is { } max ? max - 1 : (int?)null;
        if (ChecksInside)
        {
            var last = DateOnly.MaxValue.DayNumber;
            for (var i = 1; i <= (steps ?? SearchLimit); i++)
            {
                var n = anchor.DayNumber + direction * i;
                if (n < 0 || n > last) return null;
                // A range past this day would hold it inside; the day itself is rejected as an end anyway.
                if (IsDisabled!(DateOnly.FromDayNumber(n))) return DateOnly.FromDayNumber(n);
            }
        }
        return steps is { } s ? FromDayNumberClamped(anchor.DayNumber + direction * s) : null;
    }

    private static DateOnly FromDayNumberClamped(int n) =>
        DateOnly.FromDayNumber(Math.Clamp(n, 0, DateOnly.MaxValue.DayNumber));
}
