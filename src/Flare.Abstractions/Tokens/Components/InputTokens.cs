using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the field control well (Input/Select/TextArea/...). Font, text/caret color, error and
/// label/helper styling are NOT tokens here - they are owned by the shared field frame and typescale/color
/// scales. What remains is the control-well geometry and per-variant/-state borders the CSS reads.
/// </summary>
public sealed record InputTokens
{
    /// <summary>Background color of the filled variant control.</summary>
    [CssVar(InputField.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Border for the outlined variant.</summary>
    [CssVar(InputField.OutlinedBorder)] public required string OutlinedBorder { get; init; }

    /// <summary>Border radius of the control well (rounded-top for the filled variant).</summary>
    [CssVar(InputField.OutlinedRadius)] public required string OutlinedRadius { get; init; }

    /// <summary>Resting bottom border for the filled variant.</summary>
    [CssVar(InputField.FilledBorderBottom)] public required string FilledBorderBottom { get; init; }

    /// <summary>Focus border for the outlined variant.</summary>
    [CssVar(InputField.FocusBorder)] public required string FocusBorder { get; init; }

    /// <summary>Focus bottom border for the filled variant.</summary>
    [CssVar(InputField.FocusBorderBottom)] public required string FocusBorderBottom { get; init; }

    /// <summary>
    /// Focus indicator drawn as a <c>box-shadow</c> on the field well when focused (mouse or keyboard).
    /// A <c>box-shadow</c> is used so the indicator is layout-neutral (the field never grows/jumps): e.g.
    /// <c>inset 0 -2px 0 0 var(--fc-main, var(--flare-color-primary))</c> for a bottom active indicator, or
    /// <c>inset 0 0 0 1px ...</c> for a full ring. Set to <c>none</c> when the theme uses <see cref="FocusOutline"/>
    /// instead. The filled/outlined per-variant classes override it.
    /// </summary>
    [CssVar(InputField.FocusRing)] public required string FocusRing { get; init; }

    /// <summary>
    /// Focus indicator drawn as a real CSS <c>outline</c> on the field well when focused - an alternative to
    /// <see cref="FocusRing"/> for themes that want a browser-native focus rectangle:
    /// <c>2px solid var(--fc-main, var(--flare-color-primary))</c>. Use <c>none</c> to opt out (ring-only themes).
    /// </summary>
    [CssVar(InputField.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Offset of the focus <see cref="FocusOutline"/> from the field edge (CSS <c>outline-offset</c>).</summary>
    [CssVar(InputField.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Hover bottom border for the filled variant.</summary>
    [CssVar(InputField.HoverBorderBottom)] public required string HoverBorderBottom { get; init; }

    /// <summary>Hover state-layer overlay for the filled variant.</summary>
    [CssVar(InputField.HoverStateLayer)] public required string HoverStateLayer { get; init; }

    /// <summary>Padding inside the control.</summary>
    [CssVar(InputField.Padding)] public required string Padding { get; init; }

    /// <summary>Color of the placeholder text.</summary>
    [CssVar(InputField.PlaceholderColor)] public required string PlaceholderColor { get; init; }

    /// <summary>Disabled background color.</summary>
    [CssVar(InputField.DisabledBg)] public required string DisabledBg { get; init; }

    /// <summary>Disabled border/indicator color.</summary>
    [CssVar(InputField.DisabledIndicator)] public required string DisabledIndicator { get; init; }

    /// <summary>Bottom-border color of an errored field on hover.</summary>
    [CssVar(InputField.ErrorHoverIndicator)] public required string ErrorHoverIndicator { get; init; }
}
