using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme geometry tokens for <c>FlareButtonGroup</c>, which has two models rather than one look.
/// A STANDARD group is separate buttons standing together and responding to each other; a CONNECTED
/// group is a single seamed control whose segments are independent. A theme states both, because the
/// choice between them belongs to whoever is building the screen, and a language that could only
/// describe one would leave the other unstyled.
///
/// The two are described unevenly on purpose. Connected needs the seam vocabulary below. Standard
/// needs a gap and nothing else: its segments keep the shape the button already has, which is also
/// what makes a standard group's buttons free to be round or square independently of the group.
/// </summary>
public sealed record ButtonGroupTokens
{
    /// <summary>Space between the separate buttons of a STANDARD group. This is the only geometry the
    /// group contributes to that model - the corners belong to the buttons.</summary>
    [CssVar(ButtonGroup.StandardGap)] public required string StandardGap { get; init; }

    /// <summary>Space between the segments of a CONNECTED group. <c>0</c> makes them touch; a small
    /// positive value leaves a visible seam without breaking the control into separate buttons.</summary>
    [CssVar(ButtonGroup.ConnectedGap)] public required string ConnectedGap { get; init; }

    /// <summary>CONNECTED segment overlap: a (usually negative) margin on non-first segments that pulls
    /// each one back onto its neighbour, collapsing the two adjacent borders into a single shared seam.
    /// <c>0</c> = no overlap, each segment keeping its own border.</summary>
    [CssVar(ButtonGroup.ConnectedOverlap)] public required string ConnectedOverlap { get; init; }

    /// <summary>CONNECTED outer (leading/trailing group-end) corner radius - the shape of the control as
    /// a whole. May be a size-adaptive capsule (<c>calc(var(--_flare-btn-height) / 2)</c>), since the
    /// button's size class supplies that height.</summary>
    [CssVar(ButtonGroup.ConnectedOuterRadius)] public required string ConnectedOuterRadius { get; init; }

    /// <summary>CONNECTED inner (interior seam) corner radius. <c>0</c> = flat/segmented; a shape token
    /// = softened seams.</summary>
    [CssVar(ButtonGroup.ConnectedInnerRadius)] public required string ConnectedInnerRadius { get; init; }

    /// <summary>Z-index applied to a hovered/focused segment so its border and ring are not clipped by an
    /// overlapping neighbour. Shared: both models can raise the segment under the pointer.</summary>
    [CssVar(ButtonGroup.ZActive)] public required string ZActive { get; init; }
}
