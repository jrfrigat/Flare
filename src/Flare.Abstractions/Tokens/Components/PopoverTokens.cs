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
    [CssVar(PopoverPopup.SurfaceColor)] public string SurfaceColor { get; init; } = Vars.Var(Color.SurfaceContainer);

    /// <summary>Border radius of the popover.</summary>
    [CssVar(PopoverPopup.Radius)] public string Radius { get; init; } = Vars.Var(Shape.ExtraSmall);

    /// <summary>Elevation (box-shadow) of the popover.</summary>
    [CssVar(PopoverPopup.Elevation)] public string Elevation { get; init; } = Vars.Var(Flare.Css.Tokens.Elevation.Level2);

    /// <summary>Padding inside the popover.</summary>
    [CssVar(PopoverPopup.Padding)] public string Padding { get; init; } = "8px 0";

    /// <summary>Minimum width of the popover.</summary>
    [CssVar(PopoverPopup.MinWidth)] public string MinWidth { get; init; } = "112px";

    /// <summary>Maximum width of the popover.</summary>
    [CssVar(PopoverPopup.MaxWidth)] public string MaxWidth { get; init; } = "calc(100vw - 32px)";

    /// <summary>Maximum height of the popover before scrolling.</summary>
    [CssVar(PopoverPopup.MaxHeight)] public string MaxHeight { get; init; } = "calc(100vh - 32px)";

    /// <summary>Distance from the anchor element.</summary>
    [CssVar(PopoverPopup.Offset)] public string Offset { get; init; } = "4px";

    /// <summary>Arrow size (width/height).</summary>
    [CssVar(PopoverPopup.ArrowSize)] public string ArrowSize { get; init; } = "12px";

    /// <summary>Background color of the scrim (backdrop).</summary>
    [CssVar(PopoverPopup.ScrimColor)] public string ScrimColor { get; init; } = "transparent";

    /// <summary>Transition duration for show/hide.</summary>
    [CssVar(PopoverPopup.TransitionDuration)] public string TransitionDuration { get; init; } = Vars.Var(Motion.DurationShort2);

    /// <summary>Transition easing for show/hide.</summary>
    [CssVar(PopoverPopup.TransitionEasing)] public string TransitionEasing { get; init; } = Vars.Var(Motion.EasingStandard);
}
