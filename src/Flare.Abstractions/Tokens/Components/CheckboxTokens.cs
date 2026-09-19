using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCheckbox</c> (box, state-layer, focus ring).</summary>
public sealed record CheckboxTokens
{
    /// <summary>Extra-small box size.</summary>
    [CssVar(Checkbox.SizeXs)] public required string SizeXs { get; init; }

    /// <summary>Small box size.</summary>
    [CssVar(Checkbox.SizeSm)] public required string SizeSm { get; init; }

    /// <summary>
    /// Medium box size - the default step. There is no separate unsuffixed size token beside it: one
    /// value for "the size" and another for "the medium size" are two ways to say the same thing, and
    /// the ramp is what the size classes read.
    /// </summary>
    [CssVar(Checkbox.SizeMd)] public required string SizeMd { get; init; }

    /// <summary>Large box size.</summary>
    [CssVar(Checkbox.SizeLg)] public required string SizeLg { get; init; }

    /// <summary>Extra-large box size.</summary>
    [CssVar(Checkbox.SizeXl)] public required string SizeXl { get; init; }

    /// <summary>Diameter of the state-layer halo drawn around the box. Core used to fix it, so a theme
    /// enlarging the control got a halo still sized for the medium step.</summary>
    [CssVar(Checkbox.StateLayerSize)] public required string StateLayerSize { get; init; }
    /// <summary>Border (outline) thickness of the box.</summary>
    [CssVar(Checkbox.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Corner radius of the box.</summary>
    [CssVar(Checkbox.Radius)] public required string Radius { get; init; }

    /// <summary>State-layer background on hover (unchecked). Set transparent to suppress the halo.</summary>
    [CssVar(Checkbox.StateLayerHover)] public required string StateLayerHover { get; init; }

    /// <summary>State-layer background on hover when checked (accent tint). Set transparent to suppress the halo.</summary>
    [CssVar(Checkbox.StateLayerHoverChecked)] public required string StateLayerHoverChecked { get; init; }

    /// <summary>Focus-visible outline shorthand.</summary>
    [CssVar(Checkbox.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Focus outline offset.</summary>
    [CssVar(Checkbox.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Extra focus shadow (e.g. an outer focus ring); set to none for no shadow.</summary>
    [CssVar(Checkbox.FocusShadow)] public required string FocusShadow { get; init; }
    /// <summary>How far the control fades when disabled. A language that signals disabled by dimming sets
    /// a fraction; one that repaints it in a flat palette leaves it opaque and carries the change in its own
    /// stylesheet - the indicator has no spare layer to take a fill, and neither `color` nor `border-color`
    /// has a value meaning "leave this alone".</summary>
    [CssVar(Checkbox.DisabledOpacity)] public required string DisabledOpacity { get; init; }
}
