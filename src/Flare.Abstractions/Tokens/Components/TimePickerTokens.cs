using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for timepicker - component-specific geometry read by timepicker.css.</summary>
public sealed record TimePickerTokens
{
    /// <summary>Columns Sep Size.</summary>
    [CssVar(TimePickerField.ColumnsSepSize)] public required string ColumnsSepSize { get; init; }

    /// <summary>Display Size.</summary>
    [CssVar(TimePickerField.DisplaySize)] public required string DisplaySize { get; init; }

    /// <summary>Headline Tracking.</summary>
    [CssVar(TimePickerField.HeadlineTracking)] public required string HeadlineTracking { get; init; }

    /// <summary>Panel Radius.</summary>
    [CssVar(TimePickerField.PanelRadius)] public required string PanelRadius { get; init; }

    /// <summary>Time Sep Size.</summary>
    [CssVar(TimePickerField.TimeSepSize)] public required string TimeSepSize { get; init; }
}
