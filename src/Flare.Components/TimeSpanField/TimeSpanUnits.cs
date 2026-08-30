namespace Flare.Components;

/// <summary>
/// Which segments a <see cref="FlareTimeSpanPicker"/> shows. Flags rather than a "smallest unit" enum:
/// a field can legitimately show days and minutes and skip hours, and an ordered scale cannot say that.
/// </summary>
[Flags]
public enum TimeSpanUnits
{
    /// <summary>No segments - an empty field, which is never what a caller means.</summary>
    None = 0,
    /// <summary>Whole days.</summary>
    Days = 1,
    /// <summary>Hours.</summary>
    Hours = 2,
    /// <summary>Minutes.</summary>
    Minutes = 4,
    /// <summary>Seconds.</summary>
    Seconds = 8,

    /// <summary>Hours and minutes - the working-time shape.</summary>
    HoursMinutes = Hours | Minutes,
    /// <summary>Every segment.</summary>
    All = Days | Hours | Minutes | Seconds,
}
