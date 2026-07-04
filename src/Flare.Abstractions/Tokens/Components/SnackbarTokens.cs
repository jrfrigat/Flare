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
    [CssVar(SnackbarPanel.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Color of the snackbar text.</summary>
    [CssVar(SnackbarPanel.TextColor)] public required string TextColor { get; init; }

    /// <summary>Color of the action button text.</summary>
    [CssVar(SnackbarPanel.ActionColor)] public required string ActionColor { get; init; }

    /// <summary>Border radius of the snackbar.</summary>
    [CssVar(SnackbarPanel.Radius)] public required string Radius { get; init; }

    /// <summary>Minimum width of the snackbar.</summary>
    [CssVar(SnackbarPanel.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Maximum width of the snackbar.</summary>
    [CssVar(SnackbarPanel.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Height of the single-line snackbar.</summary>
    [CssVar(SnackbarPanel.Height)] public required string Height { get; init; }

    /// <summary>Height of the multi-line snackbar.</summary>
    [CssVar(SnackbarPanel.HeightMultiLine)] public required string HeightMultiLine { get; init; }

    /// <summary>Padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.Padding)] public required string Padding { get; init; }

    /// <summary>Gap between text and action button.</summary>
    [CssVar(SnackbarPanel.Gap)] public required string Gap { get; init; }

    /// <summary>Elevation (box-shadow) of the snackbar.</summary>
    [CssVar(SnackbarPanel.Elevation)] public required string Elevation { get; init; }

    /// <summary>Font family of the snackbar text.</summary>
    [CssVar(SnackbarPanel.FontFamily)] public required string FontFamily { get; init; }

    /// <summary>Font size of the snackbar text.</summary>
    [CssVar(SnackbarPanel.FontSize)] public required string FontSize { get; init; }

    /// <summary>Font weight of the action button.</summary>
    [CssVar(SnackbarPanel.ActionFontWeight)] public required string ActionFontWeight { get; init; }

    /// <summary>Font size of the action button.</summary>
    [CssVar(SnackbarPanel.ActionFontSize)] public required string ActionFontSize { get; init; }

    /// <summary>Transition duration for snackbar show/hide.</summary>
    [CssVar(SnackbarPanel.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for snackbar show/hide.</summary>
    [CssVar(SnackbarPanel.TransitionEasing)] public required string TransitionEasing { get; init; }

    /// <summary>Delay before auto-dismiss (in milliseconds).</summary>
    [CssVar(SnackbarPanel.AutoHideDelay)] public required int AutoHideDelay { get; init; }

    /// <summary>Distance from the bottom edge of the viewport.</summary>
    [CssVar(SnackbarPanel.BottomOffset)] public required string BottomOffset { get; init; }

    /// <summary>Distance from the left edge of the viewport.</summary>
    [CssVar(SnackbarPanel.LeftOffset)] public required string LeftOffset { get; init; }

    /// <summary>Distance from the right edge of the viewport.</summary>
    [CssVar(SnackbarPanel.RightOffset)] public required string RightOffset { get; init; }

    /// <summary>Gap between stacked snackbars.</summary>
    [CssVar(SnackbarPanel.StackGap)] public required string StackGap { get; init; }
}
