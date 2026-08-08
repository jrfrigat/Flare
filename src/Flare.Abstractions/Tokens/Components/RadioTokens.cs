using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareRadio</c> (ring + state-layer).</summary>
public sealed record RadioTokens
{
    /// <summary>Diameter of the radio control itself, excluding the state-layer halo around it.</summary>
    [CssVar(Radio.Size)] public required string Size { get; init; }
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
