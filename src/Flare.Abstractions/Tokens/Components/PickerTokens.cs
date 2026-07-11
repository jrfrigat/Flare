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
}
