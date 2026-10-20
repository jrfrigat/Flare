using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Snackbar: the default and per-severity surfaces, text, action color and shadow, and
/// the snackbar geometry. Fonts, gaps and motion are NOT tokens here - snackbar.css reuses the shared
/// typescale/spacing/motion scales directly.
/// </summary>
public sealed record SnackbarTokens
{
    /// <summary>Corner radius of the snackbar.</summary>
    [CssVar(SnackbarPanel.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.ExtraSmall);

    /// <summary>Minimum height of a single-line snackbar.</summary>
    [CssVar(SnackbarPanel.MinHeight)] public required string MinHeight { get; init; }

    /// <summary>Vertical (block) padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.PaddingBlock)] public required string PaddingBlock { get; init; }

    /// <summary>Inset of the snackbar stack from the viewport edge.</summary>
    [CssVar(SnackbarPanel.ProviderInset)] public string ProviderInset { get; init; } = Var(Css.Tokens.Spacing.S12);

    /// <summary>
    /// Extra clearance added to <see cref="ProviderInset"/> on the pinned side, for app chrome that
    /// occupies the edge - a bottom navigation bar, a mobile toolbar. The safe-area inset is added
    /// separately by CSS, so a theme that sets this does not have to know the device.
    /// </summary>
    [CssVar(SnackbarPanel.ProviderOffset)] public required string ProviderOffset { get; init; }

    /// <summary>Opacity of the dismiss button.</summary>
    [CssVar(SnackbarPanel.CloseOpacity)] public required string CloseOpacity { get; init; }

    /// <summary>Narrowest a snackbar gets, however short its message.</summary>
    [CssVar(SnackbarPanel.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Widest a snackbar gets before its message wraps.</summary>
    [CssVar(SnackbarPanel.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Horizontal (inline) padding inside the snackbar.</summary>
    [CssVar(SnackbarPanel.PaddingInline)] public string PaddingInline { get; init; } = Var(Css.Tokens.Spacing.S10);

    /// <summary>Background of a snackbar without a severity.</summary>
    [CssVar(SnackbarPanel.Bg)] public string Bg { get; init; } = Var(Css.Tokens.Color.InverseSurface);

    /// <summary>Text color of a snackbar without a severity; the dismiss button follows it.</summary>
    [CssVar(SnackbarPanel.Color)] public string Color { get; init; } = Var(Css.Tokens.Color.InverseOnSurface);

    /// <summary>Label color of the action button on a snackbar without a severity. Severity snackbars
    /// color the action with their own text color instead.</summary>
    [CssVar(SnackbarPanel.ActionColor)] public string ActionColor { get; init; } = Var(Css.Tokens.Color.InversePrimary);

    /// <summary>Shadow (<c>box-shadow</c>) under every snackbar; <c>none</c> for a flat one.</summary>
    [CssVar(SnackbarPanel.Shadow)] public string Shadow { get; init; } = Var(Css.Tokens.Elevation.Level3);

    // Severity surfaces. The action button and the dismiss button follow each surface's text color.

    /// <summary>Background of an error snackbar.</summary>
    [CssVar(SnackbarPanel.ErrorBg)] public string ErrorBg { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Text color of an error snackbar.</summary>
    [CssVar(SnackbarPanel.ErrorColor)] public string ErrorColor { get; init; } = Var(Css.Tokens.Color.OnError);
    /// <summary>Background of a success snackbar.</summary>
    [CssVar(SnackbarPanel.SuccessBg)] public string SuccessBg { get; init; } = Var(Css.Tokens.Color.SecondaryContainer);
    /// <summary>Text color of a success snackbar.</summary>
    [CssVar(SnackbarPanel.SuccessColor)] public string SuccessColor { get; init; } = Var(Css.Tokens.Color.OnSecondaryContainer);
    /// <summary>Background of a warning snackbar.</summary>
    [CssVar(SnackbarPanel.WarningBg)] public string WarningBg { get; init; } = Var(Css.Tokens.Color.TertiaryContainer);
    /// <summary>Text color of a warning snackbar.</summary>
    [CssVar(SnackbarPanel.WarningColor)] public string WarningColor { get; init; } = Var(Css.Tokens.Color.OnTertiaryContainer);
    /// <summary>Background of an info snackbar.</summary>
    [CssVar(SnackbarPanel.InfoBg)] public string InfoBg { get; init; } = Var(Css.Tokens.Color.PrimaryContainer);
    /// <summary>Text color of an info snackbar.</summary>
    [CssVar(SnackbarPanel.InfoColor)] public string InfoColor { get; init; } = Var(Css.Tokens.Color.OnPrimaryContainer);
}
