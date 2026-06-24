namespace Flare.Components;

/// <summary>
/// A named quick-select range offered by <see cref="FlareDateRangePicker"/> (e.g. "Last 7 days").
/// </summary>
/// <param name="Label">Text shown on the preset chip.</param>
/// <param name="Range">
/// Produces the (start, end) pair for the preset. Receives the current date ("today") so the
/// range is computed relative to it.
/// </param>
public sealed record DateRangePreset(string Label, Func<DateOnly, (DateOnly Start, DateOnly End)> Range);
