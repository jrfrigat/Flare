namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for <c>FlareButtonGroup</c>. The base <c>buttongroup.css</c> is theme-agnostic:
/// it reads these tokens (all defaulting to a plain, un-connected row of buttons) and each theme SETS
/// the bundle it wants - a connected look (zero gap, a small negative overlap that collapses adjacent
/// borders, flat inner corners) or a separated look (a positive gap, no overlap, rounded inner corners).
/// </summary>
public static class ButtonGroup
{
    private const string FlareGroup = $"{Vars.Flare}-btn-group";

    /// <summary>CSS custom-property name for the gap between segments (positive = separated segments).</summary>
    public const string Gap = $"{FlareGroup}-gap";
    /// <summary>CSS custom-property name for the segment overlap: a (usually negative) inline/block margin on
    /// non-first segments that collapses two adjacent 1px borders into one shared seam. <c>0</c> = no overlap.</summary>
    public const string Overlap = $"{FlareGroup}-overlap";
    /// <summary>CSS custom-property name for the OUTER (leading/trailing group-end) corner radius.</summary>
    public const string OuterRadius = $"{FlareGroup}-outer-radius";
    /// <summary>CSS custom-property name for the INNER (interior seam) corner radius.</summary>
    public const string InnerRadius = $"{FlareGroup}-inner-radius";
    /// <summary>CSS custom-property name for the z-index applied to a hovered/focused segment so its border
    /// and focus ring are not clipped by an overlapping neighbour.</summary>
    public const string ZActive = $"{FlareGroup}-z-active";
}
