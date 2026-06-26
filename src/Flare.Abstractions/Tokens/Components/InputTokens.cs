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
    [CssVar(InputField.FilledBg)] public string FilledBg { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Border for the outlined variant.</summary>
    [CssVar(InputField.OutlinedBorder)] public string OutlinedBorder { get; init; } = "1px solid var(--flare-color-outline)";

    /// <summary>Border radius for the outlined variant.</summary>
    [CssVar(InputField.OutlinedRadius)] public string OutlinedRadius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Bottom border for the filled variant.</summary>
    [CssVar(InputField.FilledBorderBottom)] public string FilledBorderBottom { get; init; } = "1px solid var(--flare-color-on-surface-variant)";

    /// <summary>Border radius for the filled variant (top-left, top-right, bottom-right, bottom-left).</summary>
    [CssVar(InputField.FilledRadius)] public string FilledRadius { get; init; } = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0";

    /// <summary>Focus border for outlined variant.</summary>
    [CssVar(InputField.FocusBorder)] public string FocusBorder { get; init; } = "2px solid var(--flare-color-primary)";

    /// <summary>Focus bottom border for filled variant.</summary>
    [CssVar(InputField.FocusBorderBottom)] public string FocusBorderBottom { get; init; } = "2px solid var(--flare-color-primary)";

    /// <summary>Padding inside the control (top, right, bottom, left).</summary>
    [CssVar(InputField.Padding)] public string Padding { get; init; } = "0.75rem 1rem";

    /// <summary>Font family for the input text.</summary>
    [CssVar(InputField.FontFamily)] public string FontFamily { get; init; } = "var(--flare-typescale-body-large-font)";

    /// <summary>Font size for the input text.</summary>
    [CssVar(InputField.FontSize)] public string FontSize { get; init; } = "var(--flare-typescale-body-large-size)";

    /// <summary>Color of the input text.</summary>
    [CssVar(InputField.TextColor)] public string TextColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Color of the placeholder text.</summary>
    [CssVar(InputField.PlaceholderColor)] public string PlaceholderColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Color of the caret (cursor).</summary>
    [CssVar(InputField.CaretColor)] public string CaretColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Error border color.</summary>
    [CssVar(InputField.ErrorBorder)] public string ErrorBorder { get; init; } = "var(--flare-color-error)";

    /// <summary>Error text color.</summary>
    [CssVar(InputField.ErrorColor)] public string ErrorColor { get; init; } = "var(--flare-color-error)";

    /// <summary>Disabled background color.</summary>
    [CssVar(InputField.DisabledBg)] public string DisabledBg { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)";

    /// <summary>Disabled border/indicator color.</summary>
    [CssVar(InputField.DisabledIndicator)] public string DisabledIndicator { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)";

    /// <summary>Helper text font size.</summary>
    [CssVar(InputField.HelperFontSize)] public string HelperFontSize { get; init; } = "var(--flare-typescale-body-small-size, 0.75rem)";

    /// <summary>Helper text color.</summary>
    [CssVar(InputField.HelperColor)] public string HelperColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Label font family.</summary>
    [CssVar(InputField.LabelFontFamily)] public string LabelFontFamily { get; init; } = "var(--flare-typescale-label-medium-font, var(--flare-typescale-label-large-font))";

    /// <summary>Label font size.</summary>
    [CssVar(InputField.LabelFontSize)] public string LabelFontSize { get; init; } = "var(--flare-typescale-label-medium-size, 0.75rem)";

    /// <summary>Label font weight.</summary>
    [CssVar(InputField.LabelFontWeight)] public string LabelFontWeight { get; init; } = "var(--flare-typescale-label-medium-weight, 400)";

    /// <summary>Label color.</summary>
    [CssVar(InputField.LabelColor)] public string LabelColor { get; init; } = "var(--flare-color-on-surface-variant)";
}
