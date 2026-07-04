namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlareCalendar</c>.</summary>
public static class Calendar
{
    private const string FlareCalendar = $"{Vars.Flare}-calendar";

    /// <summary>CSS custom-property name for the single-month max width token.</summary>
    public const string MaxWidth = $"{FlareCalendar}-max-width";
    /// <summary>CSS custom-property name for the per-month minimum width token.</summary>
    public const string MonthMinWidth = $"{FlareCalendar}-month-min-width";
    /// <summary>CSS custom-property name for the prev/next nav button square size token.</summary>
    public const string NavBtnSize = $"{FlareCalendar}-nav-btn-size";
    /// <summary>CSS custom-property name for the day cell minimum height token.</summary>
    public const string CellMinHeight = $"{FlareCalendar}-cell-min-height";
    /// <summary>CSS custom-property name for the day number circle size token.</summary>
    public const string DayNumSize = $"{FlareCalendar}-day-num-size";
    /// <summary>CSS custom-property name for the "today" marker background token.</summary>
    public const string TodayBg = $"{FlareCalendar}-today-bg";
    /// <summary>CSS custom-property name for the "today" marker foreground token.</summary>
    public const string TodayColor = $"{FlareCalendar}-today-color";
    /// <summary>CSS custom-property name for the selected day background token.</summary>
    public const string SelectedBg = $"{FlareCalendar}-selected-bg";
    /// <summary>CSS custom-property name for the adjacent-month day opacity token.</summary>
    public const string OtherMonthOpacity = $"{FlareCalendar}-other-month-opacity";
}
