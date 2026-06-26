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
    [CssVar(TooltipPopup.SurfaceColor)] public string SurfaceColor { get; init; } = Vars.Var(Color.InverseSurface);

    /// <summary>Color of the tooltip text.</summary>
    [CssVar(TooltipPopup.TextColor)] public string TextColor { get; init; } = Vars.Var(Color.InverseOnSurface);

    /// <summary>Border radius of the tooltip.</summary>
    [CssVar(TooltipPopup.Radius)] public string Radius { get; init; } = Vars.Var(Shape.ExtraSmall);

    /// <summary>Padding inside the tooltip.</summary>
    [CssVar(TooltipPopup.Padding)] public string Padding { get; init; } = "6px 8px";

    /// <summary>Maximum width of the tooltip.</summary>
    [CssVar(TooltipPopup.MaxWidth)] public string MaxWidth { get; init; } = "300px";

    /// <summary>Font family of the tooltip text.</summary>
    [CssVar(TooltipPopup.FontFamily)] public string FontFamily { get; init; } = Vars.Var(Typography.Font("body-small"));

    /// <summary>Font size of the tooltip text.</summary>
    [CssVar(TooltipPopup.FontSize)] public string FontSize { get; init; } = Vars.Var(Typography.Size("body-small"));

    /// <summary>Font weight of the tooltip text.</summary>
    [CssVar(TooltipPopup.FontWeight)] public string FontWeight { get; init; } = "var(--flare-typescale-body-small-weight, 400)";

    /// <summary>Line height of the tooltip text.</summary>
    [CssVar(TooltipPopup.LineHeight)] public string LineHeight { get; init; } = Vars.Var(Typography.Height("body-small"));

    /// <summary>Distance from the anchor element.</summary>
    [CssVar(TooltipPopup.Offset)] public string Offset { get; init; } = "8px";

    /// <summary>Arrow size (width/height).</summary>
    [CssVar(TooltipPopup.ArrowSize)] public string ArrowSize { get; init; } = "8px";

    /// <summary>Transition duration for show/hide.</summary>
    [CssVar(TooltipPopup.TransitionDuration)] public string TransitionDuration { get; init; } = Vars.Var(Motion.DurationShort1);

    /// <summary>Transition easing for show/hide.</summary>
    [CssVar(TooltipPopup.TransitionEasing)] public string TransitionEasing { get; init; } = Vars.Var(Motion.EasingStandard);

    /// <summary>Delay before showing (in milliseconds).</summary>
    [CssVar(TooltipPopup.ShowDelay)] public int ShowDelay { get; init; } = 100;

    /// <summary>Delay before hiding (in milliseconds).</summary>
    [CssVar(TooltipPopup.HideDelay)] public int HideDelay { get; init; } = 0;
}
