using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for datetimepicker - component-specific geometry read by datetimepicker.css.</summary>
public sealed record DateTimePickerTokens
{
    /// <summary>Panel Gap.</summary>
    [CssVar(DateTimePickerField.PanelGap)] public required string PanelGap { get; init; }
}
