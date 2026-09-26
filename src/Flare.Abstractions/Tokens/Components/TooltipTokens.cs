using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Tooltip: surface, text, shadow and padding for the plain bubble and the rich one
/// separately, plus the tooltip geometry. Radius, font and motion are NOT tokens here - the tooltip reuses
/// the shared shape, typescale and motion scales directly. The arrow takes the background of its bubble.
/// </summary>
public sealed record TooltipTokens
{
    /// <summary>Maximum width of a rich (multi-line) tooltip.</summary>
    [CssVar(TooltipPopup.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Distance between the tooltip and its anchor element.</summary>
    [CssVar(TooltipPopup.Offset)] public required string Offset { get; init; }

    /// <summary>Background of the plain (single-line) tooltip and its arrow.</summary>
    [CssVar(TooltipPopup.Bg)] public required string Bg { get; init; }

    /// <summary>Text color of the plain tooltip.</summary>
    [CssVar(TooltipPopup.Color)] public required string Color { get; init; }

    /// <summary>Padding of the plain tooltip, as a <c>padding</c> shorthand.</summary>
    [CssVar(TooltipPopup.Padding)] public required string Padding { get; init; }

    /// <summary>Shadow (<c>box-shadow</c>) under the plain tooltip; <c>none</c> for a language whose tooltip
    /// is a dark chip that stands out on its own, a real shadow for one whose tooltip is a light surface.</summary>
    [CssVar(TooltipPopup.Shadow)] public required string Shadow { get; init; }

    /// <summary>Background of the rich tooltip and its arrow.</summary>
    [CssVar(TooltipPopup.RichBg)] public required string RichBg { get; init; }

    /// <summary>Text color of the rich tooltip.</summary>
    [CssVar(TooltipPopup.RichColor)] public required string RichColor { get; init; }

    /// <summary>Shadow (<c>box-shadow</c>) of the rich tooltip; <c>none</c> for a flat one.</summary>
    [CssVar(TooltipPopup.RichShadow)] public required string RichShadow { get; init; }

    /// <summary>Padding of the rich tooltip, as a <c>padding</c> shorthand.</summary>
    [CssVar(TooltipPopup.RichPadding)] public required string RichPadding { get; init; }
}
