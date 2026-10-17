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
    [CssVar(PopoverPopup.Radius)] public string Radius { get; init; } = "var(--flare-shape-medium)";
    /// <summary>Distance between a popover and its anchor (a CSS length).</summary>
    [CssVar(PopoverPopup.Offset)] public string Offset { get; init; } = "var(--flare-spacing-4)";
    /// <summary>Background of the popover panel and its arrow.</summary>
    [CssVar(PopoverPopup.Bg)] public string Bg { get; init; } = "var(--flare-color-surface-container)";
    /// <summary>Text color of the popover panel.</summary>
    [CssVar(PopoverPopup.Color)] public string Color { get; init; } = "var(--flare-color-on-surface)";
    /// <summary>Shadow (<c>box-shadow</c>) of the popover panel; <c>none</c> for a flat one.</summary>
    [CssVar(PopoverPopup.Shadow)] public string Shadow { get; init; } = "var(--flare-elevation-2)";
}
