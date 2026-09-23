using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for popover-like surfaces (popover, menu, pickers). Surface color, elevation, padding,
/// width, scrim and motion are NOT tokens here - the CSS reuses the shared color/elevation/spacing/
/// motion scales directly. The corner radius and the distance from the anchor are tokens (themes vary them).
/// </summary>
public sealed record PopoverTokens
{
    /// <summary>Corner radius shared by all popover-like surfaces.</summary>
    [CssVar(PopoverPopup.Radius)] public required string Radius { get; init; }
    /// <summary>Distance between a popover and its anchor (a CSS length).</summary>
    [CssVar(PopoverPopup.Offset)] public required string Offset { get; init; }
}
