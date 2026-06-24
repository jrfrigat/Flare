namespace Flare.Core.Tokens.Components;

/// <summary>
/// Design tokens for Input, Select, TextArea, and other form field components.
/// These control the geometry, spacing, and appearance of form controls.
/// </summary>
public sealed record InputTokens
{
    /// <summary>Background color of the filled variant control.</summary>
    public string FilledBg { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Border for the outlined variant.</summary>
    public string OutlinedBorder { get; init; } = "1px solid var(--flare-color-outline)";

    /// <summary>Border radius for the outlined variant.</summary>
    public string OutlinedRadius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Bottom border for the filled variant.</summary>
    public string FilledBorderBottom { get; init; } = "1px solid var(--flare-color-on-surface-variant)";

    /// <summary>Border radius for the filled variant (top-left, top-right, bottom-right, bottom-left).</summary>
    public string FilledRadius { get; init; } = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0";

    /// <summary>Focus border for outlined variant.</summary>
    public string FocusBorder { get; init; } = "2px solid var(--flare-color-primary)";

    /// <summary>Focus bottom border for filled variant.</summary>
    public string FocusBorderBottom { get; init; } = "2px solid var(--flare-color-primary)";

    /// <summary>Padding inside the control (top, right, bottom, left).</summary>
    public string Padding { get; init; } = "0.75rem 1rem";

    /// <summary>Font family for the input text.</summary>
    public string FontFamily { get; init; } = "var(--flare-typescale-body-large-font)";

    /// <summary>Font size for the input text.</summary>
    public string FontSize { get; init; } = "var(--flare-typescale-body-large-size)";

    /// <summary>Color of the input text.</summary>
    public string TextColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Color of the placeholder text.</summary>
    public string PlaceholderColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Color of the caret (cursor).</summary>
    public string CaretColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Error border color.</summary>
    public string ErrorBorder { get; init; } = "var(--flare-color-error)";

    /// <summary>Error text color.</summary>
    public string ErrorColor { get; init; } = "var(--flare-color-error)";

    /// <summary>Disabled background color.</summary>
    public string DisabledBg { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)";

    /// <summary>Disabled border/indicator color.</summary>
    public string DisabledIndicator { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)";

    /// <summary>Helper text font size.</summary>
    public string HelperFontSize { get; init; } = "var(--flare-typescale-body-small-size, 0.75rem)";

    /// <summary>Helper text color.</summary>
    public string HelperColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Label font family.</summary>
    public string LabelFontFamily { get; init; } = "var(--flare-typescale-label-medium-font, var(--flare-typescale-label-large-font))";

    /// <summary>Label font size.</summary>
    public string LabelFontSize { get; init; } = "var(--flare-typescale-label-medium-size, 0.75rem)";

    /// <summary>Label font weight.</summary>
    public string LabelFontWeight { get; init; } = "var(--flare-typescale-label-medium-weight, 400)";

    /// <summary>Label color.</summary>
    public string LabelColor { get; init; } = "var(--flare-color-on-surface-variant)";
}
