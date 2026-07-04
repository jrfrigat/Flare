using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Popover component.
/// These control the geometry, spacing, and appearance of popovers.
/// </summary>
public sealed record PopoverTokens
{
    /// <summary>Background color of the popover surface.</summary>
    [CssVar(PopoverPopup.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Border radius of the popover.</summary>
    [CssVar(PopoverPopup.Radius)] public required string Radius { get; init; }

    /// <summary>Elevation (box-shadow) of the popover.</summary>
    [CssVar(PopoverPopup.Elevation)] public required string Elevation { get; init; }

    /// <summary>Padding inside the popover.</summary>
    [CssVar(PopoverPopup.Padding)] public required string Padding { get; init; }

    /// <summary>Minimum width of the popover.</summary>
    [CssVar(PopoverPopup.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Maximum width of the popover.</summary>
    [CssVar(PopoverPopup.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Maximum height of the popover before scrolling.</summary>
    [CssVar(PopoverPopup.MaxHeight)] public required string MaxHeight { get; init; }

    /// <summary>Distance from the anchor element.</summary>
    [CssVar(PopoverPopup.Offset)] public required string Offset { get; init; }

    /// <summary>Arrow size (width/height).</summary>
    [CssVar(PopoverPopup.ArrowSize)] public required string ArrowSize { get; init; }

    /// <summary>Background color of the scrim (backdrop).</summary>
    [CssVar(PopoverPopup.ScrimColor)] public required string ScrimColor { get; init; }

    /// <summary>Transition duration for show/hide.</summary>
    [CssVar(PopoverPopup.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for show/hide.</summary>
    [CssVar(PopoverPopup.TransitionEasing)] public required string TransitionEasing { get; init; }
}
