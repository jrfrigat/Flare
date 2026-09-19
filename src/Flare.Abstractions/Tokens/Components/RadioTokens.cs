using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareRadio</c> (ring + state-layer).</summary>
public sealed record RadioTokens
{
    /// <summary>Extra-small ring diameter, excluding the state-layer halo around it.</summary>
    [CssVar(Radio.SizeXs)] public required string SizeXs { get; init; }

    /// <summary>Small ring diameter.</summary>
    [CssVar(Radio.SizeSm)] public required string SizeSm { get; init; }

    /// <summary>
    /// Medium ring diameter - the default step. There is no separate unsuffixed size token beside it:
    /// one value for "the size" and another for "the medium size" are two ways to say the same thing,
    /// and the ramp is what the size classes read.
    /// </summary>
    [CssVar(Radio.SizeMd)] public required string SizeMd { get; init; }

    /// <summary>Large ring diameter.</summary>
    [CssVar(Radio.SizeLg)] public required string SizeLg { get; init; }

    /// <summary>Extra-large ring diameter.</summary>
    [CssVar(Radio.SizeXl)] public required string SizeXl { get; init; }

    /// <summary>Diameter of the state-layer halo drawn around the ring. Core used to fix it, so a theme
    /// enlarging the control got a halo still sized for the medium step.</summary>
    [CssVar(Radio.StateLayerSize)] public required string StateLayerSize { get; init; }
    /// <summary>Background of the state-layer halo on hover while unselected.</summary>
    [CssVar(Radio.StateLayerHover)] public required string StateLayerHover { get; init; }

    /// <summary>State-layer background on hover when selected (accent tint).</summary>
    [CssVar(Radio.StateLayerHoverChecked)] public required string StateLayerHoverChecked { get; init; }

    /// <summary>Focus-visible outline shorthand drawn around the ring.</summary>
    /// <remarks>
    /// The radio used to draw this from a literal in the core stylesheet while its siblings read
    /// tokens, so a theme could restyle a checkbox's focus and not a radio's - and the two ended up
    /// disagreeing on both width and role. These three mirror the checkbox's, so the selection
    /// controls are configured the same way.
    /// </remarks>
    [CssVar(Radio.FocusOutline)] public required string FocusOutline { get; init; }
    /// <summary>Distance the focus outline sits away from the ring.</summary>
    [CssVar(Radio.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }
    /// <summary>Shadow drawn on keyboard focus, in addition to the outline. A theme that signals focus
    /// with the outline alone parks this at a no-op.</summary>
    [CssVar(Radio.FocusShadow)] public required string FocusShadow { get; init; }
    /// <summary>How far the control fades when disabled. A language that signals disabled by dimming sets
    /// a fraction; one that repaints it in a flat palette leaves it opaque and carries the change in its own
    /// stylesheet - the indicator has no spare layer to take a fill, and neither `color` nor `border-color`
    /// has a value meaning "leave this alone".</summary>
    [CssVar(Radio.DisabledOpacity)] public required string DisabledOpacity { get; init; }
}
