using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Snackbar: the default and per-severity surfaces, text, action color and shadow, and
/// the snackbar geometry. Fonts, gaps and motion are NOT tokens here - snackbar.css reuses the shared
/// typescale/spacing/motion scales directly.
/// </summary>
public sealed record SnackbarTokens
{
    /// <summary>Corner radius of the snackbar.</summary>
    [CssVar(SnackbarPanel.Radius)] public required string Radius { get; init; }

    /// <summary>Minimum height of a single-line snackbar.</summary>
    [CssVar(SnackbarPanel.MinHeight)] public required string MinHeight { get; init; }

    /// <summary>Vertical (block) padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.PaddingBlock)] public required string PaddingBlock { get; init; }

    /// <summary>Inset of the snackbar stack from the viewport edge.</summary>
    [CssVar(SnackbarPanel.ProviderInset)] public required string ProviderInset { get; init; }

    /// <summary>Opacity of the dismiss button.</summary>
    [CssVar(SnackbarPanel.CloseOpacity)] public required string CloseOpacity { get; init; }

    /// <summary>Narrowest a snackbar gets, however short its message.</summary>
    [CssVar(SnackbarPanel.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Widest a snackbar gets before its message wraps.</summary>
    [CssVar(SnackbarPanel.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Horizontal (inline) padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.PaddingInline)] public required string PaddingInline { get; init; }

    /// <summary>Background of a snackbar without a severity.</summary>
    [CssVar(SnackbarPanel.Bg)] public required string Bg { get; init; }

    /// <summary>Text color of a snackbar without a severity; the dismiss button follows it.</summary>
    [CssVar(SnackbarPanel.Color)] public required string Color { get; init; }

    /// <summary>Label color of the action button on a snackbar without a severity. Severity snackbars
    /// color the action with their own text color instead.</summary>
    [CssVar(SnackbarPanel.ActionColor)] public required string ActionColor { get; init; }

    /// <summary>Shadow (<c>box-shadow</c>) under every snackbar; <c>none</c> for a flat one.</summary>
    [CssVar(SnackbarPanel.Shadow)] public required string Shadow { get; init; }

    // Severity surfaces. The action button and the dismiss button follow each surface's text color.

    /// <summary>Background of an error snackbar.</summary>
    [CssVar(SnackbarPanel.ErrorBg)] public required string ErrorBg { get; init; }
    /// <summary>Text color of an error snackbar.</summary>
    [CssVar(SnackbarPanel.ErrorColor)] public required string ErrorColor { get; init; }
    /// <summary>Background of a success snackbar.</summary>
    [CssVar(SnackbarPanel.SuccessBg)] public required string SuccessBg { get; init; }
    /// <summary>Text color of a success snackbar.</summary>
    [CssVar(SnackbarPanel.SuccessColor)] public required string SuccessColor { get; init; }
    /// <summary>Background of a warning snackbar.</summary>
    [CssVar(SnackbarPanel.WarningBg)] public required string WarningBg { get; init; }
    /// <summary>Text color of a warning snackbar.</summary>
    [CssVar(SnackbarPanel.WarningColor)] public required string WarningColor { get; init; }
    /// <summary>Background of an info snackbar.</summary>
    [CssVar(SnackbarPanel.InfoBg)] public required string InfoBg { get; init; }
    /// <summary>Text color of an info snackbar.</summary>
    [CssVar(SnackbarPanel.InfoColor)] public required string InfoColor { get; init; }
}
