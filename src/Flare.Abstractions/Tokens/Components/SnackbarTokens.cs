using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Snackbar component.
/// These control the geometry, spacing, and appearance of snackbar notifications.
/// </summary>
public sealed record SnackbarTokens
{
    /// <summary>Background color of the snackbar.</summary>
    [CssVar(SnackbarPanel.SurfaceColor)] public string SurfaceColor { get; init; } = "var(--flare-color-inverse-surface)";

    /// <summary>Color of the snackbar text.</summary>
    [CssVar(SnackbarPanel.TextColor)] public string TextColor { get; init; } = "var(--flare-color-inverse-on-surface)";

    /// <summary>Color of the action button text.</summary>
    [CssVar(SnackbarPanel.ActionColor)] public string ActionColor { get; init; } = "var(--flare-color-inverse-primary)";

    /// <summary>Border radius of the snackbar.</summary>
    [CssVar(SnackbarPanel.Radius)] public string Radius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Minimum width of the snackbar.</summary>
    [CssVar(SnackbarPanel.MinWidth)] public string MinWidth { get; init; } = "344px";

    /// <summary>Maximum width of the snackbar.</summary>
    [CssVar(SnackbarPanel.MaxWidth)] public string MaxWidth { get; init; } = "560px";

    /// <summary>Height of the single-line snackbar.</summary>
    [CssVar(SnackbarPanel.Height)] public string Height { get; init; } = "48px";

    /// <summary>Height of the multi-line snackbar.</summary>
    [CssVar(SnackbarPanel.HeightMultiLine)] public string HeightMultiLine { get; init; } = "68px";

    /// <summary>Padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.Padding)] public string Padding { get; init; } = "0 16px";

    /// <summary>Gap between text and action button.</summary>
    [CssVar(SnackbarPanel.Gap)] public string Gap { get; init; } = "8px";

    /// <summary>Elevation (box-shadow) of the snackbar.</summary>
    [CssVar(SnackbarPanel.Elevation)] public string Elevation { get; init; } = "var(--flare-elevation-3)";

    /// <summary>Font family of the snackbar text.</summary>
    [CssVar(SnackbarPanel.FontFamily)] public string FontFamily { get; init; } = "var(--flare-typescale-body-medium-font)";

    /// <summary>Font size of the snackbar text.</summary>
    [CssVar(SnackbarPanel.FontSize)] public string FontSize { get; init; } = "var(--flare-typescale-body-medium-size)";

    /// <summary>Font weight of the action button.</summary>
    [CssVar(SnackbarPanel.ActionFontWeight)] public string ActionFontWeight { get; init; } = "var(--flare-typescale-label-large-weight, 500)";

    /// <summary>Font size of the action button.</summary>
    [CssVar(SnackbarPanel.ActionFontSize)] public string ActionFontSize { get; init; } = "var(--flare-typescale-label-large-size)";

    /// <summary>Transition duration for snackbar show/hide.</summary>
    [CssVar(SnackbarPanel.TransitionDuration)] public string TransitionDuration { get; init; } = "var(--flare-motion-duration-short2)";

    /// <summary>Transition easing for snackbar show/hide.</summary>
    [CssVar(SnackbarPanel.TransitionEasing)] public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Delay before auto-dismiss (in milliseconds).</summary>
    [CssVar(SnackbarPanel.AutoHideDelay)] public int AutoHideDelay { get; init; } = 5000;

    /// <summary>Distance from the bottom edge of the viewport.</summary>
    [CssVar(SnackbarPanel.BottomOffset)] public string BottomOffset { get; init; } = "16px";

    /// <summary>Distance from the left edge of the viewport.</summary>
    [CssVar(SnackbarPanel.LeftOffset)] public string LeftOffset { get; init; } = "16px";

    /// <summary>Distance from the right edge of the viewport.</summary>
    [CssVar(SnackbarPanel.RightOffset)] public string RightOffset { get; init; } = "16px";

    /// <summary>Gap between stacked snackbars.</summary>
    [CssVar(SnackbarPanel.StackGap)] public string StackGap { get; init; } = "8px";
}
