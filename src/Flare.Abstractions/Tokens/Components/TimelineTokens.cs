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
    [CssVar(Timeline.DotBg)] public required string DotBg { get; init; }

    /// <summary>Border width of the node dot.</summary>
    [CssVar(Timeline.DotBorderWidth)] public required string DotBorderWidth { get; init; }

    /// <summary>Glyph size of an icon rendered inside the node dot.</summary>
    [CssVar(Timeline.DotIconSize)] public required string DotIconSize { get; init; }

    /// <summary>Thickness of the connector line between nodes.</summary>
    [CssVar(Timeline.LineWidth)] public required string LineWidth { get; init; }

    /// <summary>Color of the connector line between nodes.</summary>
    [CssVar(Timeline.LineColor)] public required string LineColor { get; init; }

    /// <summary>Width of the connector column (dot + line).</summary>
    [CssVar(Timeline.ConnectorWidth)] public required string ConnectorWidth { get; init; }
}
