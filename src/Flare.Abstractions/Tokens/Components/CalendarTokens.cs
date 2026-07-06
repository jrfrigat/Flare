using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareCalendar</c> (month layout, day cells, the today marker and selection).
/// Surface/border/nav backgrounds reference the theme's semantic color roles, event chips follow the
/// per-instance <c>Color</c> (via <c>--fc-container</c>), and the container/cell radii reuse the shared
/// shape scale; day-label and day-number typography reuse the shared typescale tokens.
/// </summary>
public sealed record CalendarTokens
{
    /// <summary>Event pad y.</summary>
    [CssVar(Calendar.EventPadY)] public required string EventPadY { get; init; }
    /// <summary>Maximum width of a single-month calendar.</summary>
    [CssVar(Calendar.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Minimum width reserved for each month panel.</summary>
    [CssVar(Calendar.MonthMinWidth)] public required string MonthMinWidth { get; init; }

    /// <summary>Square size of the previous/next navigation buttons.</summary>
    [CssVar(Calendar.NavBtnSize)] public required string NavBtnSize { get; init; }

    /// <summary>Minimum height of a day cell.</summary>
    [CssVar(Calendar.CellMinHeight)] public required string CellMinHeight { get; init; }

    /// <summary>Diameter of the day-number circle (today marker sizing).</summary>
    [CssVar(Calendar.DayNumSize)] public required string DayNumSize { get; init; }

    /// <summary>Background of the "today" day marker.</summary>
    [CssVar(Calendar.TodayBg)] public required string TodayBg { get; init; }

    /// <summary>Foreground of the "today" day marker.</summary>
    [CssVar(Calendar.TodayColor)] public required string TodayColor { get; init; }

    /// <summary>Background of a selected day cell.</summary>
    [CssVar(Calendar.SelectedBg)] public required string SelectedBg { get; init; }

    /// <summary>Opacity applied to days belonging to an adjacent month.</summary>
    [CssVar(Calendar.OtherMonthOpacity)] public required string OtherMonthOpacity { get; init; }
}
