using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareTimeline</c> / <c>FlareTimelineItem</c> (node dot, connector line and
/// its column). The dot border color follows the per-instance <c>Color</c> (via <c>currentColor</c>),
/// so it is not a token here; item time/title/content typography and spacing reuse the shared
/// typescale/spacing tokens.
/// </summary>
public sealed record TimelineTokens
{
    /// <summary>Diameter of the node dot.</summary>
    [CssVar(Timeline.DotSize)] public required string DotSize { get; init; }

    /// <summary>Background fill of the node dot.</summary>
    [CssVar(Timeline.DotBg)] public string DotBg { get; init; } = "var(--flare-color-surface)";

    /// <summary>Border width of the node dot.</summary>
    [CssVar(Timeline.DotBorderWidth)] public string DotBorderWidth { get; init; } = "var(--flare-border-width-emphasis)";

    /// <summary>Glyph size of an icon rendered inside the node dot.</summary>
    [CssVar(Timeline.DotIconSize)] public required string DotIconSize { get; init; }

    /// <summary>Thickness of the connector line between nodes.</summary>
    [CssVar(Timeline.LineWidth)] public string LineWidth { get; init; } = "var(--flare-border-width-emphasis)";

    /// <summary>Color of the connector line between nodes.</summary>
    [CssVar(Timeline.LineColor)] public string LineColor { get; init; } = "var(--flare-color-outline-variant)";

    /// <summary>Width of the connector column (dot + line).</summary>
    [CssVar(Timeline.ConnectorWidth)] public required string ConnectorWidth { get; init; }
}
