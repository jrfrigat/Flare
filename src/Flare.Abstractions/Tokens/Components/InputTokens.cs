using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Input, Select, TextArea, and other form field components.
/// These control the geometry, spacing, and appearance of form controls.
/// </summary>
public sealed record InputTokens
{
    /// <summary>Background color of the filled variant control.</summary>
    [CssVar(InputField.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Border for the outlined variant.</summary>
    [CssVar(InputField.OutlinedBorder)] public required string OutlinedBorder { get; init; }

    /// <summary>Border radius for the outlined variant.</summary>
    [CssVar(InputField.OutlinedRadius)] public required string OutlinedRadius { get; init; }

    /// <summary>Bottom border for the filled variant.</summary>
    [CssVar(InputField.FilledBorderBottom)] public required string FilledBorderBottom { get; init; }

    /// <summary>Border radius for the filled variant (top-left, top-right, bottom-right, bottom-left).</summary>
    [CssVar(InputField.FilledRadius)] public required string FilledRadius { get; init; }

    /// <summary>Focus border for outlined variant.</summary>
    [CssVar(InputField.FocusBorder)] public required string FocusBorder { get; init; }

    /// <summary>Focus bottom border for filled variant.</summary>
    [CssVar(InputField.FocusBorderBottom)] public required string FocusBorderBottom { get; init; }

    /// <summary>Hover bottom border for the filled variant. Defaults to the resting bottom border.</summary>
    [CssVar(InputField.HoverBorderBottom)] public required string HoverBorderBottom { get; init; }

    /// <summary>Hover state-layer overlay for the filled variant. Default none.</summary>
    [CssVar(InputField.HoverStateLayer)] public required string HoverStateLayer { get; init; }

    /// <summary>Padding inside the control (top, right, bottom, left).</summary>
    [CssVar(InputField.Padding)] public required string Padding { get; init; }

    /// <summary>Font family for the input text.</summary>
    [CssVar(InputField.FontFamily)] public required string FontFamily { get; init; }

    /// <summary>Font size for the input text.</summary>
    [CssVar(InputField.FontSize)] public required string FontSize { get; init; }

    /// <summary>Color of the input text.</summary>
    [CssVar(InputField.TextColor)] public required string TextColor { get; init; }

    /// <summary>Color of the placeholder text.</summary>
    [CssVar(InputField.PlaceholderColor)] public required string PlaceholderColor { get; init; }

    /// <summary>Color of the caret (cursor).</summary>
    [CssVar(InputField.CaretColor)] public required string CaretColor { get; init; }

    /// <summary>Error border color.</summary>
    [CssVar(InputField.ErrorBorder)] public required string ErrorBorder { get; init; }

    /// <summary>Error text color.</summary>
    [CssVar(InputField.ErrorColor)] public required string ErrorColor { get; init; }

    /// <summary>Disabled background color.</summary>
    [CssVar(InputField.DisabledBg)] public required string DisabledBg { get; init; }

    /// <summary>Disabled border/indicator color.</summary>
    [CssVar(InputField.DisabledIndicator)] public required string DisabledIndicator { get; init; }

    /// <summary>Helper text font size.</summary>
    [CssVar(InputField.HelperFontSize)] public required string HelperFontSize { get; init; }

    /// <summary>Helper text color.</summary>
    [CssVar(InputField.HelperColor)] public required string HelperColor { get; init; }

    /// <summary>Label font family.</summary>
    [CssVar(InputField.LabelFontFamily)] public required string LabelFontFamily { get; init; }

    /// <summary>Label font size.</summary>
    [CssVar(InputField.LabelFontSize)] public required string LabelFontSize { get; init; }

    /// <summary>Label font weight.</summary>
    [CssVar(InputField.LabelFontWeight)] public required string LabelFontWeight { get; init; }

    /// <summary>Label color.</summary>
    [CssVar(InputField.LabelColor)] public required string LabelColor { get; init; }
}
