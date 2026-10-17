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
    [CssVar(SnackbarPanel.Radius)] public string Radius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Minimum height of a single-line snackbar.</summary>
    [CssVar(SnackbarPanel.MinHeight)] public required string MinHeight { get; init; }

    /// <summary>Vertical (block) padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.PaddingBlock)] public required string PaddingBlock { get; init; }

    /// <summary>Inset of the snackbar stack from the viewport edge.</summary>
    [CssVar(SnackbarPanel.ProviderInset)] public string ProviderInset { get; init; } = "var(--flare-spacing-12)";

    /// <summary>Opacity of the dismiss button.</summary>
    [CssVar(SnackbarPanel.CloseOpacity)] public required string CloseOpacity { get; init; }

    /// <summary>Narrowest a snackbar gets, however short its message.</summary>
    [CssVar(SnackbarPanel.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Widest a snackbar gets before its message wraps.</summary>
    [CssVar(SnackbarPanel.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Horizontal (inline) padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.PaddingInline)] public string PaddingInline { get; init; } = "var(--flare-spacing-10)";

    /// <summary>Background of a snackbar without a severity.</summary>
    [CssVar(SnackbarPanel.Bg)] public string Bg { get; init; } = "var(--flare-color-inverse-surface)";

    /// <summary>Text color of a snackbar without a severity; the dismiss button follows it.</summary>
    [CssVar(SnackbarPanel.Color)] public string Color { get; init; } = "var(--flare-color-inverse-on-surface)";

    /// <summary>Label color of the action button on a snackbar without a severity. Severity snackbars
    /// color the action with their own text color instead.</summary>
    [CssVar(SnackbarPanel.ActionColor)] public string ActionColor { get; init; } = "var(--flare-color-inverse-primary)";

    /// <summary>Shadow (<c>box-shadow</c>) under every snackbar; <c>none</c> for a flat one.</summary>
    [CssVar(SnackbarPanel.Shadow)] public string Shadow { get; init; } = "var(--flare-elevation-3)";

    // Severity surfaces. The action button and the dismiss button follow each surface's text color.

    /// <summary>Background of an error snackbar.</summary>
    [CssVar(SnackbarPanel.ErrorBg)] public string ErrorBg { get; init; } = "var(--flare-color-error)";
    /// <summary>Text color of an error snackbar.</summary>
    [CssVar(SnackbarPanel.ErrorColor)] public string ErrorColor { get; init; } = "var(--flare-color-on-error)";
    /// <summary>Background of a success snackbar.</summary>
    [CssVar(SnackbarPanel.SuccessBg)] public string SuccessBg { get; init; } = "var(--flare-color-secondary-container)";
    /// <summary>Text color of a success snackbar.</summary>
    [CssVar(SnackbarPanel.SuccessColor)] public string SuccessColor { get; init; } = "var(--flare-color-on-secondary-container)";
    /// <summary>Background of a warning snackbar.</summary>
    [CssVar(SnackbarPanel.WarningBg)] public string WarningBg { get; init; } = "var(--flare-color-tertiary-container)";
    /// <summary>Text color of a warning snackbar.</summary>
    [CssVar(SnackbarPanel.WarningColor)] public string WarningColor { get; init; } = "var(--flare-color-on-tertiary-container)";
    /// <summary>Background of an info snackbar.</summary>
    [CssVar(SnackbarPanel.InfoBg)] public string InfoBg { get; init; } = "var(--flare-color-primary-container)";
    /// <summary>Text color of an info snackbar.</summary>
    [CssVar(SnackbarPanel.InfoColor)] public string InfoColor { get; init; } = "var(--flare-color-on-primary-container)";
}
