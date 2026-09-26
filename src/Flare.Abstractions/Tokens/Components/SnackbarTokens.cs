using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Snackbar: the default surface, text, action color and shadow, and the snackbar
/// geometry. Fonts, gaps, the severity surfaces and motion are NOT tokens here - snackbar.css reuses the
/// shared color/typescale/spacing/motion scales directly.
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
}
