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

    /// <summary>
    /// Colour of the control-well border on all four sides. The border width is reserved on every variant
    /// in the CSS, so the field's height never shifts between filled and outlined; the theme sets only the
    /// colour, and <c>transparent</c> hides a side (the filled variant's top/left/right).
    /// </summary>
    [CssVar(InputField.BorderColor)] public required string BorderColor { get; init; }

    /// <summary>Border radius of the control well (rounded-top for the filled variant).</summary>
    [CssVar(InputField.OutlinedRadius)] public required string OutlinedRadius { get; init; }

    /// <summary>Resting bottom-border colour (the filled variant's active indicator). Overrides
    /// <see cref="BorderColor"/> on the bottom edge only.</summary>
    [CssVar(InputField.BorderBottomColor)] public required string BorderBottomColor { get; init; }

    /// <summary>
    /// Focus indicator drawn as a <c>box-shadow</c> on the field well when focused (mouse or keyboard).
    /// A <c>box-shadow</c> is used so the indicator is layout-neutral (the field never grows/jumps): an
    /// <c>inset</c> shadow offset onto the bottom edge gives a bottom active indicator, and a spread-only
    /// <c>inset</c> shadow gives a full ring. Set to <c>none</c> when the theme uses
    /// <see cref="FocusOutline"/> instead. The filled/outlined per-variant classes override it.
    /// </summary>
    [CssVar(InputField.FocusRing)] public required string FocusRing { get; init; }

    /// <summary>
    /// Focus indicator drawn as a real CSS <c>outline</c> on the field well when focused - an alternative to
    /// <see cref="FocusRing"/> for themes that want a browser-native focus rectangle, as an <c>outline</c>
    /// shorthand. Use <c>none</c> to opt out (ring-only themes).
    /// </summary>
    [CssVar(InputField.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Offset of the focus <see cref="FocusOutline"/> from the field edge (CSS <c>outline-offset</c>).</summary>
    [CssVar(InputField.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Hover bottom-border colour.</summary>
    [CssVar(InputField.HoverBorderBottomColor)] public required string HoverBorderBottomColor { get; init; }

    /// <summary>Hover state-layer overlay for the filled variant.</summary>
    [CssVar(InputField.HoverStateLayer)] public required string HoverStateLayer { get; init; }

    /// <summary>Control padding at the extra-small size.</summary>
    [CssVar(InputField.PaddingXs)] public required string PaddingXs { get; init; }

    /// <summary>Control padding at the small size.</summary>
    [CssVar(InputField.PaddingSm)] public required string PaddingSm { get; init; }

    /// <summary>Control padding at the default (medium) size - the field a component renders when no
    /// size is asked for.</summary>
    [CssVar(InputField.PaddingMd)] public required string PaddingMd { get; init; }

    /// <summary>Control padding at the large size.</summary>
    [CssVar(InputField.PaddingLg)] public required string PaddingLg { get; init; }

    /// <summary>Control padding at the extra-large size. Padding no longer decides how tall a field is -
    /// <see cref="HeightMd"/> and its siblings do - so these five only place the content inside that
    /// height.</summary>
    [CssVar(InputField.PaddingXl)] public required string PaddingXl { get; init; }

    /// <summary>Height of the field well at the extra-small size.</summary>
    [CssVar(InputField.HeightXs)] public required string HeightXs { get; init; }

    /// <summary>Height of the field well at the small size.</summary>
    [CssVar(InputField.HeightSm)] public required string HeightSm { get; init; }

    /// <summary>
    /// Height of the field well at the default (medium) size - the height a field takes when no size is
    /// asked for, and the one every control in the family has to agree on. The WELL is what is measured,
    /// border included, because that is the box the caller sees beside a button or another field.
    /// </summary>
    [CssVar(InputField.HeightMd)] public required string HeightMd { get; init; }

    /// <summary>Height of the field well at the large size.</summary>
    [CssVar(InputField.HeightLg)] public required string HeightLg { get; init; }

    /// <summary>
    /// Height of the field well at the extra-large size. These five are the family height ramp: a
    /// single-line well is exactly this tall whatever it holds - text, a chevron, a clear button, a
    /// picker toggle, a numeric stepper - and the wells that legitimately grow (TextArea, TagField) take
    /// it as their floor. The theme owns the ordering; a large step shorter than the medium one is a theme
    /// bug the stylesheet cannot correct. Leave room for the content: a step shorter than its own padding
    /// plus a line of text makes the control box overflow the well it is centred in.
    /// </summary>
    [CssVar(InputField.HeightXl)] public required string HeightXl { get; init; }

    /// <summary>
    /// Size of the leading/trailing icons, including the expand toggle the date and time pickers put in the
    /// trailing slot - they are the same field affordance, so one token drives all of them.
    /// </summary>
    [CssVar(InputField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Color of the placeholder text.</summary>
    [CssVar(InputField.PlaceholderColor)] public required string PlaceholderColor { get; init; }

    /// <summary>Disabled background color.</summary>
    [CssVar(InputField.DisabledBg)] public required string DisabledBg { get; init; }

    /// <summary>Disabled border/indicator color.</summary>
    [CssVar(InputField.DisabledIndicator)] public required string DisabledIndicator { get; init; }

    /// <summary>Bottom-border color of an errored field on hover.</summary>
    [CssVar(InputField.ErrorHoverIndicator)] public required string ErrorHoverIndicator { get; init; }

    /// <summary>How far a disabled field's content fades - label, helper, adornments, icons and the
    /// placeholder, plus the numeric stepper. A language that repaints them in a flat palette leaves this
    /// opaque and carries the change in its own stylesheet, since a foreground colour has no value
    /// meaning "leave this as painted".</summary>
    [CssVar(InputField.DisabledOpacity)] public required string DisabledOpacity { get; init; }
}
