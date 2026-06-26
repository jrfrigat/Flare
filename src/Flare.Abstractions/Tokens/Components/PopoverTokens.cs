namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Popover component.
/// These control the geometry, spacing, and appearance of popovers.
/// </summary>
public sealed record PopoverTokens
{
    /// <summary>Background color of the popover surface.</summary>
    public string SurfaceColor { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Border radius of the popover.</summary>
    public string Radius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Elevation (box-shadow) of the popover.</summary>
    public string Elevation { get; init; } = "var(--flare-elevation-2)";

    /// <summary>Padding inside the popover.</summary>
    public string Padding { get; init; } = "8px 0";

    /// <summary>Minimum width of the popover.</summary>
    public string MinWidth { get; init; } = "112px";

    /// <summary>Maximum width of the popover.</summary>
    public string MaxWidth { get; init; } = "calc(100vw - 32px)";

    /// <summary>Maximum height of the popover before scrolling.</summary>
    public string MaxHeight { get; init; } = "calc(100vh - 32px)";

    /// <summary>Distance from the anchor element.</summary>
    public string Offset { get; init; } = "4px";

    /// <summary>Arrow size (width/height).</summary>
    public string ArrowSize { get; init; } = "12px";

    /// <summary>Background color of the scrim (backdrop).</summary>
    public string ScrimColor { get; init; } = "transparent";

    /// <summary>Transition duration for show/hide.</summary>
    public string TransitionDuration { get; init; } = "var(--flare-motion-duration-short2)";

    /// <summary>Transition easing for show/hide.</summary>
    public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";
}
