namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Tooltip component.
/// These control the geometry, spacing, and appearance of tooltips.
/// </summary>
public sealed record TooltipTokens
{
    /// <summary>Background color of the tooltip.</summary>
    public string SurfaceColor { get; init; } = "var(--flare-color-inverse-surface)";

    /// <summary>Color of the tooltip text.</summary>
    public string TextColor { get; init; } = "var(--flare-color-inverse-on-surface)";

    /// <summary>Border radius of the tooltip.</summary>
    public string Radius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Padding inside the tooltip.</summary>
    public string Padding { get; init; } = "6px 8px";

    /// <summary>Maximum width of the tooltip.</summary>
    public string MaxWidth { get; init; } = "300px";

    /// <summary>Font family of the tooltip text.</summary>
    public string FontFamily { get; init; } = "var(--flare-typescale-body-small-font)";

    /// <summary>Font size of the tooltip text.</summary>
    public string FontSize { get; init; } = "var(--flare-typescale-body-small-size)";

    /// <summary>Font weight of the tooltip text.</summary>
    public string FontWeight { get; init; } = "var(--flare-typescale-body-small-weight, 400)";

    /// <summary>Line height of the tooltip text.</summary>
    public string LineHeight { get; init; } = "var(--flare-typescale-body-small-height)";

    /// <summary>Distance from the anchor element.</summary>
    public string Offset { get; init; } = "8px";

    /// <summary>Arrow size (width/height).</summary>
    public string ArrowSize { get; init; } = "8px";

    /// <summary>Transition duration for show/hide.</summary>
    public string TransitionDuration { get; init; } = "var(--flare-motion-duration-short1)";

    /// <summary>Transition easing for show/hide.</summary>
    public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Delay before showing (in milliseconds).</summary>
    public int ShowDelay { get; init; } = 100;

    /// <summary>Delay before hiding (in milliseconds).</summary>
    public int HideDelay { get; init; } = 0;
}
