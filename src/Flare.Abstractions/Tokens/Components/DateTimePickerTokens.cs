using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for datetimepicker - component-specific geometry read by datetimepicker.css.</summary>
public sealed record DateTimePickerTokens
{
    /// <summary>Panel Gap.</summary>
    [CssVar(DateTimePickerField.PanelGap)] public required string PanelGap { get; init; }

    /// <summary>Narrowest width of the combined date-time panel (tabs layout).</summary>
    [CssVar(DateTimePickerField.PanelMinWidth)] public required string PanelMinWidth { get; init; }

    /// <summary>Narrowest width of the combined panel while it shows the clock dial, which needs the dial
    /// plus the panel padding.</summary>
    [CssVar(DateTimePickerField.DialPanelMinWidth)] public required string DialPanelMinWidth { get; init; }

    /// <summary>Narrowest width of the side-by-side layout, where the calendar and the time are two cards.</summary>
    [CssVar(DateTimePickerField.SplitPanelMinWidth)] public required string SplitPanelMinWidth { get; init; }

    /// <summary>Starting width (<c>flex-basis</c>) of the calendar card in the side-by-side layout.</summary>
    [CssVar(DateTimePickerField.PaneBasis)] public required string PaneBasis { get; init; }

    /// <summary>Fixed width of the clock-dial card in the side-by-side layout; it must hold the dial.</summary>
    [CssVar(DateTimePickerField.DialPaneWidth)] public required string DialPaneWidth { get; init; }

    /// <summary>Width of the hour and minute number inputs of the time pane.</summary>
    [CssVar(DateTimePickerField.TimeInputWidth)] public required string TimeInputWidth { get; init; }
}
