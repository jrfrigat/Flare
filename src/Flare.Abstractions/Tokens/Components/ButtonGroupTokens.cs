using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme geometry tokens for <c>FlareButtonGroup</c>, which has two models rather than one look.
/// A STANDARD group is separate buttons standing together and responding to each other; a CONNECTED
/// group is a single seamed control whose segments are independent. A theme states both, because the
/// choice between them belongs to whoever is building the screen, and a language that could only
/// describe one would leave the other unstyled. Either model can be worn by any button variant - the
/// group describes the spacing and the seams, never the fill.
///
/// The two are described unevenly on purpose. Connected needs the seam vocabulary below. Standard
/// needs a gap and nothing else: its segments keep the shape the button already has, which is also
/// what makes a standard group's buttons free to be round or square independently of the group.
///
/// Which families ramp per size follows from whether a height can answer the question. A capsule is
/// half the segment's own height and the size class puts that height in scope, so the outer and
/// selected radii are one token each. Interior corners, pressed corners and standard gaps are ramps a
/// design language chooses freely - and at least one of them ramps DOWNWARD as the buttons grow - so no
/// arithmetic on a height reproduces them and each is stated per size.
/// </summary>
public sealed record ButtonGroupTokens
{
    /// <summary>Space between the separate buttons of a STANDARD group at the xs size. This is the only
    /// geometry the group contributes to that model - the corners belong to the buttons.</summary>
    [CssVar(ButtonGroup.StandardGap.Xs)] public required string StandardGapXs { get; init; }
    /// <summary>Space between the separate buttons of a STANDARD group at the sm size.</summary>
    [CssVar(ButtonGroup.StandardGap.Sm)] public required string StandardGapSm { get; init; }
    /// <summary>Space between the separate buttons of a STANDARD group at the md size.</summary>
    [CssVar(ButtonGroup.StandardGap.Md)] public required string StandardGapMd { get; init; }
    /// <summary>Space between the separate buttons of a STANDARD group at the lg size.</summary>
    [CssVar(ButtonGroup.StandardGap.Lg)] public required string StandardGapLg { get; init; }
    /// <summary>Space between the separate buttons of a STANDARD group at the xl size.</summary>
    [CssVar(ButtonGroup.StandardGap.Xl)] public required string StandardGapXl { get; init; }

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

    /// <summary>Corner radius a SELECTED segment of a CONNECTED group takes. Material makes it fully
    /// round, which is the same capsule arithmetic the outer radius uses; a theme that signals selection
    /// with colour alone points this at its inner radius instead.</summary>
    [CssVar(ButtonGroup.ConnectedSelectedRadius)] public required string ConnectedSelectedRadius { get; init; }

    /// <summary>CONNECTED inner (interior seam) corner radius at the xs size. <c>0</c> = flat/segmented;
    /// a shape token = softened seams.</summary>
    [CssVar(ButtonGroup.ConnectedInnerRadius.Xs)] public required string ConnectedInnerRadiusXs { get; init; }
    /// <summary>CONNECTED inner corner radius at the sm size.</summary>
    [CssVar(ButtonGroup.ConnectedInnerRadius.Sm)] public required string ConnectedInnerRadiusSm { get; init; }
    /// <summary>CONNECTED inner corner radius at the md size.</summary>
    [CssVar(ButtonGroup.ConnectedInnerRadius.Md)] public required string ConnectedInnerRadiusMd { get; init; }
    /// <summary>CONNECTED inner corner radius at the lg size.</summary>
    [CssVar(ButtonGroup.ConnectedInnerRadius.Lg)] public required string ConnectedInnerRadiusLg { get; init; }
    /// <summary>CONNECTED inner corner radius at the xl size.</summary>
    [CssVar(ButtonGroup.ConnectedInnerRadius.Xl)] public required string ConnectedInnerRadiusXl { get; init; }

    /// <summary>Corner radius of a PRESSED segment of a CONNECTED group at the xs size. A theme whose
    /// press does not reshape points this at the rest inner radius.</summary>
    [CssVar(ButtonGroup.ConnectedPressedRadius.Xs)] public required string ConnectedPressedRadiusXs { get; init; }
    /// <summary>Corner radius of a pressed CONNECTED segment at the sm size.</summary>
    [CssVar(ButtonGroup.ConnectedPressedRadius.Sm)] public required string ConnectedPressedRadiusSm { get; init; }
    /// <summary>Corner radius of a pressed CONNECTED segment at the md size.</summary>
    [CssVar(ButtonGroup.ConnectedPressedRadius.Md)] public required string ConnectedPressedRadiusMd { get; init; }
    /// <summary>Corner radius of a pressed CONNECTED segment at the lg size.</summary>
    [CssVar(ButtonGroup.ConnectedPressedRadius.Lg)] public required string ConnectedPressedRadiusLg { get; init; }
    /// <summary>Corner radius of a pressed CONNECTED segment at the xl size.</summary>
    [CssVar(ButtonGroup.ConnectedPressedRadius.Xl)] public required string ConnectedPressedRadiusXl { get; init; }

    /// <summary>Z-index applied to a hovered/focused segment so its border and ring are not clipped by an
    /// overlapping neighbour. Shared: both models can raise the segment under the pointer.</summary>
    [CssVar(ButtonGroup.ZActive)] public required string ZActive { get; init; }
}
