using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Tooltip. Color, radius, padding, font and motion are intentionally NOT tokens
/// here - the tooltip reuses the shared inverse-surface role, shape, spacing, typescale and motion scales
/// directly. Only the two tooltip-specific geometry values are tokens.
/// </summary>
public sealed record TooltipTokens
{
    /// <summary>Maximum width of a rich (multi-line) tooltip.</summary>
    [CssVar(TooltipPopup.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Distance between the tooltip and its anchor element.</summary>
    [CssVar(TooltipPopup.Offset)] public required string Offset { get; init; }
}
