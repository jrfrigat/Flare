namespace Flare.Components;

/// <summary>
/// The initial calendar view a date picker opens to. <see cref="Year"/> jumps straight to the year
/// grid - handy for far-back dates like a date of birth so the user does not page through months.
/// </summary>
public enum PickerOpenTo
{
    /// <summary>Open on the day grid (the default).</summary>
    Day,
    /// <summary>Open on the month grid (pick a month first).</summary>
    Month,
    /// <summary>Open on the year grid (pick a year first).</summary>
    Year,
}
