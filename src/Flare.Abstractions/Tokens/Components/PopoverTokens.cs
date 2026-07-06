using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for popover-like surfaces (popover, menu, pickers). Surface color, elevation, padding,
/// width, offset, scrim and motion are NOT tokens here - the CSS reuses the shared color/elevation/spacing/
/// motion scales directly. Only the shared corner radius is a token (themes vary it).
/// </summary>
public sealed record PopoverTokens
{
    /// <summary>Corner radius shared by all popover-like surfaces.</summary>
    [CssVar(PopoverPopup.Radius)] public required string Radius { get; init; }
}
