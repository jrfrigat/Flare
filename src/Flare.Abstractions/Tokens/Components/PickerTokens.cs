using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for picker - component-specific geometry read by picker.css.</summary>
public sealed record PickerTokens
{
    /// <summary>Outside Opacity.</summary>
    [CssVar(PickerField.OutsideOpacity)] public required string OutsideOpacity { get; init; }

    /// <summary>Disabled Opacity.</summary>
    [CssVar(PickerField.DisabledOpacity)] public required string DisabledOpacity { get; init; }

    /// <summary>Week-number column opacity (the leading week-of-year cells in the calendar grid).</summary>
    [CssVar(PickerField.WeekNumberOpacity)] public required string WeekNumberOpacity { get; init; }

    /// <summary>
    /// Minimum width of the calendar panel. Capped at the width of its containing block, so a panel wider
    /// than a phone screen shrinks the day columns instead of overflowing.
    /// </summary>
    [CssVar(PickerField.PanelMinWidth)] public required string PanelMinWidth { get; init; }

    /// <summary>Corner radius of the date and date-time picker panels.</summary>
    [CssVar(PickerField.PanelRadius)] public required string PanelRadius { get; init; }

    /// <summary>Minimum height of the month header that holds the previous/next buttons and the month label.</summary>
    [CssVar(PickerField.HeaderHeight)] public required string HeaderHeight { get; init; }

    /// <summary>Size of the previous/next month icons in the header.</summary>
    [CssVar(PickerField.NavIconSize)] public required string NavIconSize { get; init; }

    /// <summary>Minimum height of the weekday-name row above the day grid.</summary>
    [CssVar(PickerField.WeekdayHeight)] public required string WeekdayHeight { get; init; }

    /// <summary>Font size of the weekday names.</summary>
    [CssVar(PickerField.WeekdayFontSize)] public required string WeekdayFontSize { get; init; }

    /// <summary>
    /// Height of one row of the day grid - the touch target a day sits in. Columns share the panel width
    /// equally, and a range highlight runs through them without gaps.
    /// </summary>
    [CssVar(PickerField.DaySize)] public required string DaySize { get; init; }

    /// <summary>
    /// Diameter of the circle a day paints for hover, today and selection, centred in its cell. A range
    /// highlight is this tall.
    /// </summary>
    [CssVar(PickerField.DayLayerSize)] public required string DayLayerSize { get; init; }

    /// <summary>Font size of the day numbers.</summary>
    [CssVar(PickerField.DayFontSize)] public required string DayFontSize { get; init; }
}
