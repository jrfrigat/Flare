namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for <c>FlareButtonGroup</c>. The base <c>buttongroup.css</c> is theme-agnostic:
/// it reads these tokens and each theme states the geometry of BOTH group models.
///
/// The two models need different amounts of description, and that asymmetry is the point rather than
/// an oversight. A CONNECTED group is one control with seams, so it needs the whole seam vocabulary -
/// how far the segments overlap, what the group's ends are shaped like, and what the interior corners
/// look like in each state. A STANDARD group is separate buttons standing next to each other: each
/// keeps its own shape, so there is nothing for the group to say about corners at all, and a gap is the
/// whole description. Giving standard a set of corner tokens would invite a theme to describe a shape
/// the model does not have.
///
/// Some families ramp per size and some do not, and which is which follows from whether the height can
/// answer the question. A capsule is half the segment's own height, and the segment's size class puts
/// that height in scope, so one token spells it at every size. Interior corners, pressed corners and
/// standard gaps are ramps a design language picks freely - one of them may even tighten as the buttons
/// grow - so no arithmetic on a height reproduces them and each is stated per size.
/// </summary>
public static class ButtonGroup
{
    /// <summary>Gaps between the separate buttons of a STANDARD group, per size. Their corners are the
    /// buttons' own, so this is the only geometry the group contributes to that model.</summary>
    public static class StandardGap
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-group-standard-gap-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-group-standard-gap-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-group-standard-gap-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-group-standard-gap-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-group-standard-gap-xl";
    }

    /// <summary>CSS custom-property name for the gap between the segments of a CONNECTED group. Often
    /// zero (segments touching) or a hairline that reads as a seam. One value at every size.</summary>
    public const string ConnectedGap = "--flare-btn-group-connected-gap";
    /// <summary>CSS custom-property name for the CONNECTED segment overlap: a (usually negative)
    /// inline/block margin on non-first segments that collapses two adjacent 1px borders into one
    /// shared seam. <c>0</c> = no overlap.</summary>
    public const string ConnectedOverlap = "--flare-btn-group-connected-overlap";
    /// <summary>CSS custom-property name for the CONNECTED group's OUTER (leading/trailing end) corner
    /// radius - the shape of the whole control.</summary>
    public const string ConnectedOuterRadius = "--flare-btn-group-connected-outer-radius";
    /// <summary>CSS custom-property name for the corner radius a SELECTED segment of a CONNECTED group
    /// takes.</summary>
    public const string ConnectedSelectedRadius = "--flare-btn-group-connected-selected-radius";

    /// <summary>CONNECTED interior (seam) corner radii at rest, per size.</summary>
    public static class ConnectedInnerRadius
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-group-connected-inner-radius-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-group-connected-inner-radius-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-group-connected-inner-radius-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-group-connected-inner-radius-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-group-connected-inner-radius-xl";
    }

    /// <summary>CONNECTED corner radii while a segment is pressed, per size.</summary>
    public static class ConnectedPressedRadius
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-group-connected-pressed-radius-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-group-connected-pressed-radius-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-group-connected-pressed-radius-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-group-connected-pressed-radius-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-group-connected-pressed-radius-xl";
    }

    /// <summary>CSS custom-property name for the z-index applied to a hovered/focused segment so its
    /// border and focus ring are not clipped by an overlapping neighbour. Applies to either model.</summary>
    public const string ZActive = "--flare-btn-group-z-active";
}
