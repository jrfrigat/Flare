using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Tooltip component.
/// These control the geometry, spacing, and appearance of tooltips.
/// </summary>
public sealed record TooltipTokens
{
    /// <summary>Background color of the tooltip.</summary>
    [CssVar(TooltipPopup.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Color of the tooltip text.</summary>
    [CssVar(TooltipPopup.TextColor)] public required string TextColor { get; init; }

    /// <summary>Border radius of the tooltip.</summary>
    [CssVar(TooltipPopup.Radius)] public required string Radius { get; init; }

    /// <summary>Padding inside the tooltip.</summary>
    [CssVar(TooltipPopup.Padding)] public required string Padding { get; init; }

    /// <summary>Maximum width of the tooltip.</summary>
    [CssVar(TooltipPopup.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Font family of the tooltip text.</summary>
    [CssVar(TooltipPopup.FontFamily)] public required string FontFamily { get; init; }

    /// <summary>Font size of the tooltip text.</summary>
    [CssVar(TooltipPopup.FontSize)] public required string FontSize { get; init; }

    /// <summary>Font weight of the tooltip text.</summary>
    [CssVar(TooltipPopup.FontWeight)] public required string FontWeight { get; init; }

    /// <summary>Line height of the tooltip text.</summary>
    [CssVar(TooltipPopup.LineHeight)] public required string LineHeight { get; init; }

    /// <summary>Distance from the anchor element.</summary>
    [CssVar(TooltipPopup.Offset)] public required string Offset { get; init; }

    /// <summary>Arrow size (width/height).</summary>
    [CssVar(TooltipPopup.ArrowSize)] public required string ArrowSize { get; init; }

    /// <summary>Transition duration for show/hide.</summary>
    [CssVar(TooltipPopup.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for show/hide.</summary>
    [CssVar(TooltipPopup.TransitionEasing)] public required string TransitionEasing { get; init; }

    /// <summary>Delay before showing (in milliseconds).</summary>
    [CssVar(TooltipPopup.ShowDelay)] public required int ShowDelay { get; init; }

    /// <summary>Delay before hiding (in milliseconds).</summary>
    [CssVar(TooltipPopup.HideDelay)] public required int HideDelay { get; init; }
}
