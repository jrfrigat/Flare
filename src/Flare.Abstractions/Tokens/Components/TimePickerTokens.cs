using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for timepicker - component-specific geometry read by timepicker.css.</summary>
public sealed record TimePickerTokens
{
    /// <summary>Columns Sep Size.</summary>
    [CssVar(TimePickerField.ColumnsSepSize)] public required string ColumnsSepSize { get; init; }

    /// <summary>Diameter of the dot at the centre of the clock dial, where the hand is pinned.</summary>
    [CssVar(TimePickerField.DialCenterSize)] public required string DialCenterSize { get; init; }

    /// <summary>Diameter of a number cell on the clock dial; the selected number fills it as the hand's handle.</summary>
    [CssVar(TimePickerField.DialHandleSize)] public required string DialHandleSize { get; init; }

    /// <summary>Diameter of the clock dial. Pointer input is measured against the rendered dial, so any length works.</summary>
    [CssVar(TimePickerField.DialSize)] public required string DialSize { get; init; }

    /// <summary>Thickness of the clock hand joining the centre dot to the selected number.</summary>
    [CssVar(TimePickerField.DialTrackWidth)] public required string DialTrackWidth { get; init; }

    /// <summary>Minimum height of the stacked AM/PM selector beside the time fields.</summary>
    [CssVar(TimePickerField.PeriodHeight)] public required string PeriodHeight { get; init; }

    /// <summary>Minimum width of the stacked AM/PM selector; longer localized designators widen it.</summary>
    [CssVar(TimePickerField.PeriodWidth)] public required string PeriodWidth { get; init; }

    /// <summary>Minimum height of the hour and minute fields above the clock dial.</summary>
    [CssVar(TimePickerField.TimeFieldHeight)] public required string TimeFieldHeight { get; init; }

    /// <summary>Minimum width of the hour and minute fields above the clock dial.</summary>
    [CssVar(TimePickerField.TimeFieldWidth)] public required string TimeFieldWidth { get; init; }

    /// <summary>Display Size.</summary>
    [CssVar(TimePickerField.DisplaySize)] public required string DisplaySize { get; init; }

    /// <summary>Headline Tracking.</summary>
    [CssVar(TimePickerField.HeadlineTracking)] public required string HeadlineTracking { get; init; }

    /// <summary>Panel Radius.</summary>
    [CssVar(TimePickerField.PanelRadius)] public required string PanelRadius { get; init; }

    /// <summary>Time Sep Size.</summary>
    [CssVar(TimePickerField.TimeSepSize)] public required string TimeSepSize { get; init; }
}
