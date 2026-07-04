using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Switch component.
/// These control the geometry, sizing, and appearance of switches.
/// </summary>
public sealed record SwitchTokens
{
    /// <summary>Width of the track (default/md).</summary>
    [CssVar(SwitchField.TrackWidth)] public required string TrackWidth { get; init; }

    /// <summary>Height of the track (default/md).</summary>
    [CssVar(SwitchField.TrackHeight)] public required string TrackHeight { get; init; }

    /// <summary>Width of the track (sm).</summary>
    [CssVar(SwitchField.TrackWidthSm)] public required string TrackWidthSm { get; init; }

    /// <summary>Height of the track (sm).</summary>
    [CssVar(SwitchField.TrackHeightSm)] public required string TrackHeightSm { get; init; }

    /// <summary>Width of the track (lg).</summary>
    [CssVar(SwitchField.TrackWidthLg)] public required string TrackWidthLg { get; init; }

    /// <summary>Height of the track (lg).</summary>
    [CssVar(SwitchField.TrackHeightLg)] public required string TrackHeightLg { get; init; }

    /// <summary>Border radius of the track.</summary>
    [CssVar(SwitchField.TrackRadius)] public required string TrackRadius { get; init; }

    /// <summary>Background color of the track (unchecked).</summary>
    [CssVar(SwitchField.TrackColor)] public required string TrackColor { get; init; }

    /// <summary>Border color of the track (unchecked).</summary>
    [CssVar(SwitchField.TrackBorderColor)] public required string TrackBorderColor { get; init; }

    /// <summary>Border width of the track (unchecked).</summary>
    [CssVar(SwitchField.TrackBorderWidth)] public required string TrackBorderWidth { get; init; }

    /// <summary>Background color of the track (checked).</summary>
    [CssVar(SwitchField.TrackColorSelected)] public required string TrackColorSelected { get; init; }

    /// <summary>Border color of the track (checked).</summary>
    [CssVar(SwitchField.TrackBorderColorSelected)] public required string TrackBorderColorSelected { get; init; }

    /// <summary>Diameter of the thumb (default/md).</summary>
    [CssVar(SwitchField.ThumbSize)] public required string ThumbSize { get; init; }

    /// <summary>Diameter of the thumb (sm).</summary>
    [CssVar(SwitchField.ThumbSizeSm)] public required string ThumbSizeSm { get; init; }

    /// <summary>Diameter of the thumb (lg).</summary>
    [CssVar(SwitchField.ThumbSizeLg)] public required string ThumbSizeLg { get; init; }

    /// <summary>Background color of the thumb (unchecked).</summary>
    [CssVar(SwitchField.ThumbColor)] public required string ThumbColor { get; init; }

    /// <summary>Background color of the thumb (checked).</summary>
    [CssVar(SwitchField.ThumbColorSelected)] public required string ThumbColorSelected { get; init; }

    /// <summary>Color of the thumb icon (unchecked).</summary>
    [CssVar(SwitchField.ThumbIconColor)] public required string ThumbIconColor { get; init; }

    /// <summary>Color of the thumb icon (checked).</summary>
    [CssVar(SwitchField.ThumbIconColorSelected)] public required string ThumbIconColorSelected { get; init; }

    /// <summary>Shadow of the thumb. Defaults to the shared level-1 elevation token rather than a
    /// hardcoded shadow, so the thumb lift follows the theme's elevation scale (and shadow color tokens).</summary>
    [CssVar(SwitchField.ThumbShadow)] public required string ThumbShadow { get; init; }

    /// <summary>Width of the focus outline.</summary>
    [CssVar(SwitchField.FocusOutlineWidth)] public required string FocusOutlineWidth { get; init; }

    /// <summary>Color of the focus outline.</summary>
    [CssVar(SwitchField.FocusOutlineColor)] public required string FocusOutlineColor { get; init; }

    /// <summary>Offset of the focus outline.</summary>
    [CssVar(SwitchField.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Transition duration for state changes.</summary>
    [CssVar(SwitchField.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for state changes.</summary>
    [CssVar(SwitchField.TransitionEasing)] public required string TransitionEasing { get; init; }

    /// <summary>Background color of the pressed state layer.</summary>
    [CssVar(SwitchField.PressedLayerColor)] public required string PressedLayerColor { get; init; }

    /// <summary>Opacity of the pressed state layer.</summary>
    [CssVar(SwitchField.PressedLayerOpacity)] public required string PressedLayerOpacity { get; init; }

    /// <summary>Disabled opacity of the switch.</summary>
    [CssVar(SwitchField.DisabledOpacity)] public required string DisabledOpacity { get; init; }
}
