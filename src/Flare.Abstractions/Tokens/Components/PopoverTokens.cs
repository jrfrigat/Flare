using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the popover panel: its surface, text and shadow, plus the corner radius and anchor
/// distance the popover-like surfaces (popover, pickers) share. Padding, width, scrim and motion are NOT
/// tokens here - the CSS reuses the shared spacing/motion scales directly. The arrow takes the panel
/// background.
/// </summary>
public sealed record PopoverTokens
{
    /// <summary>Corner radius shared by all popover-like surfaces.</summary>
    [CssVar(PopoverPopup.Radius)] public required string Radius { get; init; }
    /// <summary>Distance between a popover and its anchor (a CSS length).</summary>
    [CssVar(PopoverPopup.Offset)] public required string Offset { get; init; }
    /// <summary>Background of the popover panel and its arrow.</summary>
    [CssVar(PopoverPopup.Bg)] public required string Bg { get; init; }
    /// <summary>Text color of the popover panel.</summary>
    [CssVar(PopoverPopup.Color)] public required string Color { get; init; }
    /// <summary>Shadow (<c>box-shadow</c>) of the popover panel; <c>none</c> for a flat one.</summary>
    [CssVar(PopoverPopup.Shadow)] public required string Shadow { get; init; }
}
