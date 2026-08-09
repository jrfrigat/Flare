namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for <c>FlareButtonGroup</c>. The base <c>buttongroup.css</c> is theme-agnostic:
/// it reads these tokens and each theme states the geometry of BOTH group models.
///
/// The two models need different amounts of description, and that asymmetry is the point rather than
/// an oversight. A CONNECTED group is one control with seams, so it needs the whole seam vocabulary -
/// how far the segments overlap, what the group's ends are shaped like, and what the interior corners
/// look like. A STANDARD group is separate buttons standing next to each other: each keeps its own
/// shape, so there is nothing for the group to say about corners at all, and a gap is the whole
/// description. Giving standard a set of corner tokens would invite a theme to describe a shape the
/// model does not have.
/// </summary>
public static class ButtonGroup
{
    /// <summary>CSS custom-property name for the gap between the separate buttons of a STANDARD group.
    /// Their corners are the buttons' own, so this is the only geometry the group contributes.</summary>
    public const string StandardGap = "--flare-btn-group-standard-gap";
    /// <summary>CSS custom-property name for the gap between the segments of a CONNECTED group. Often
    /// zero (segments touching) or a hairline that reads as a seam.</summary>
    public const string ConnectedGap = "--flare-btn-group-connected-gap";
    /// <summary>CSS custom-property name for the CONNECTED segment overlap: a (usually negative)
    /// inline/block margin on non-first segments that collapses two adjacent 1px borders into one
    /// shared seam. <c>0</c> = no overlap.</summary>
    public const string ConnectedOverlap = "--flare-btn-group-connected-overlap";
    /// <summary>CSS custom-property name for the CONNECTED group's OUTER (leading/trailing end) corner
    /// radius - the shape of the whole control.</summary>
    public const string ConnectedOuterRadius = "--flare-btn-group-connected-outer-radius";
    /// <summary>CSS custom-property name for the CONNECTED group's INNER (interior seam) corner
    /// radius.</summary>
    public const string ConnectedInnerRadius = "--flare-btn-group-connected-inner-radius";
    /// <summary>CSS custom-property name for the z-index applied to a hovered/focused segment so its
    /// border and focus ring are not clipped by an overlapping neighbour. Applies to either model.</summary>
    public const string ZActive = "--flare-btn-group-z-active";
}
