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
    [CssVar(SwitchField.TrackWidth)] public string TrackWidth { get; init; } = "52px";

    /// <summary>Height of the track (default/md).</summary>
    [CssVar(SwitchField.TrackHeight)] public string TrackHeight { get; init; } = "32px";

    /// <summary>Width of the track (sm).</summary>
    [CssVar(SwitchField.TrackWidthSm)] public string TrackWidthSm { get; init; } = "40px";

    /// <summary>Height of the track (sm).</summary>
    [CssVar(SwitchField.TrackHeightSm)] public string TrackHeightSm { get; init; } = "24px";

    /// <summary>Width of the track (lg).</summary>
    [CssVar(SwitchField.TrackWidthLg)] public string TrackWidthLg { get; init; } = "64px";

    /// <summary>Height of the track (lg).</summary>
    [CssVar(SwitchField.TrackHeightLg)] public string TrackHeightLg { get; init; } = "40px";

    /// <summary>Border radius of the track.</summary>
    [CssVar(SwitchField.TrackRadius)] public string TrackRadius { get; init; } = Vars.Var(Shape.Full);

    /// <summary>Background color of the track (unchecked).</summary>
    [CssVar(SwitchField.TrackColor)] public string TrackColor { get; init; } = Vars.Var(Color.SurfaceContainerHighest);

    /// <summary>Border color of the track (unchecked).</summary>
    [CssVar(SwitchField.TrackBorderColor)] public string TrackBorderColor { get; init; } = Vars.Var(Color.Outline);

    /// <summary>Border width of the track (unchecked).</summary>
    [CssVar(SwitchField.TrackBorderWidth)] public string TrackBorderWidth { get; init; } = "2px";

    /// <summary>Background color of the track (checked).</summary>
    [CssVar(SwitchField.TrackColorSelected)] public string TrackColorSelected { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Border color of the track (checked).</summary>
    [CssVar(SwitchField.TrackBorderColorSelected)] public string TrackBorderColorSelected { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Diameter of the thumb (default/md).</summary>
    [CssVar(SwitchField.ThumbSize)] public string ThumbSize { get; init; } = "24px";

    /// <summary>Diameter of the thumb (sm).</summary>
    [CssVar(SwitchField.ThumbSizeSm)] public string ThumbSizeSm { get; init; } = "16px";

    /// <summary>Diameter of the thumb (lg).</summary>
    [CssVar(SwitchField.ThumbSizeLg)] public string ThumbSizeLg { get; init; } = "32px";

    /// <summary>Background color of the thumb (unchecked).</summary>
    [CssVar(SwitchField.ThumbColor)] public string ThumbColor { get; init; } = Vars.Var(Color.Outline);

    /// <summary>Background color of the thumb (checked).</summary>
    [CssVar(SwitchField.ThumbColorSelected)] public string ThumbColorSelected { get; init; } = Vars.Var(Color.OnPrimary);

    /// <summary>Color of the thumb icon (unchecked).</summary>
    [CssVar(SwitchField.ThumbIconColor)] public string ThumbIconColor { get; init; } = Vars.Var(Color.SurfaceContainerHighest);

    /// <summary>Color of the thumb icon (checked).</summary>
    [CssVar(SwitchField.ThumbIconColorSelected)] public string ThumbIconColorSelected { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Shadow of the thumb.</summary>
    [CssVar(SwitchField.ThumbShadow)] public string ThumbShadow { get; init; } = "0 1px 3px 1px rgba(0,0,0,0.15), 0 1px 2px rgba(0,0,0,0.3)";

    /// <summary>Width of the focus outline.</summary>
    [CssVar(SwitchField.FocusOutlineWidth)] public string FocusOutlineWidth { get; init; } = "2px";

    /// <summary>Color of the focus outline.</summary>
    [CssVar(SwitchField.FocusOutlineColor)] public string FocusOutlineColor { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Offset of the focus outline.</summary>
    [CssVar(SwitchField.FocusOutlineOffset)] public string FocusOutlineOffset { get; init; } = "2px";

    /// <summary>Transition duration for state changes.</summary>
    [CssVar(SwitchField.TransitionDuration)] public string TransitionDuration { get; init; } = Vars.Var(Motion.DurationShort2);

    /// <summary>Transition easing for state changes.</summary>
    [CssVar(SwitchField.TransitionEasing)] public string TransitionEasing { get; init; } = Vars.Var(Motion.EasingStandard);

    /// <summary>Background color of the pressed state layer.</summary>
    [CssVar(SwitchField.PressedLayerColor)] public string PressedLayerColor { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Opacity of the pressed state layer.</summary>
    [CssVar(SwitchField.PressedLayerOpacity)] public string PressedLayerOpacity { get; init; } = Vars.Var(State.PressedOpacity);

    /// <summary>Disabled opacity of the switch.</summary>
    [CssVar(SwitchField.DisabledOpacity)] public string DisabledOpacity { get; init; } = Vars.Var(State.DisabledOpacity);
}
