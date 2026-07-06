using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Snackbar. Surface/text/action colors, fonts, elevation, widths, viewport offsets,
/// gaps and motion are NOT tokens here - snackbar.css reuses the shared color/typescale/elevation/spacing/
/// motion scales directly. Only the snackbar-specific geometry the CSS reads are tokens.
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
}
