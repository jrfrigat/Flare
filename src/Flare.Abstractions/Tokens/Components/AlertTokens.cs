using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareAlert</c>.</summary>
public sealed record AlertTokens
{
    /// <summary>Body opacity.</summary>
    [CssVar(Alert.BodyOpacity)] public required string BodyOpacity { get; init; }
    /// <summary>Close opacity.</summary>
    [CssVar(Alert.CloseOpacity)] public required string CloseOpacity { get; init; }
    /// <summary>Border radius of the alert container (a theme typically maps this to its shape scale).</summary>
    [CssVar(Alert.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.ExtraSmall);

    /// <summary>Border width for outlined and text variants (filled uses 0).</summary>
    [CssVar(Alert.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Internal padding of the alert container.</summary>
    [CssVar(Alert.Padding)] public required string Padding { get; init; }

    /// <summary>Gap between icon and content.</summary>
    [CssVar(Alert.Gap)] public required string Gap { get; init; }

    // Severity surfaces. The defaults are the status containers every theme defines; a design language
    // without status roles points them at its accent containers instead. The outline default follows the
    // fill, so an outlined alert reads as one colour story unless a theme separates the two.

    /// <summary>Fill of an info alert.</summary>
    [CssVar(Alert.InfoBg)] public string InfoBg { get; init; } = Var(Css.Tokens.Color.InfoContainer);
    /// <summary>Text and icon colour of an info alert.</summary>
    [CssVar(Alert.InfoColor)] public string InfoColor { get; init; } = Var(Css.Tokens.Color.OnInfoContainer);
    /// <summary>Outline of an outlined info alert.</summary>
    [CssVar(Alert.InfoBorder)] public string InfoBorder { get; init; } = Var(Css.Tokens.Color.InfoContainer);
    /// <summary>Fill of a success alert.</summary>
    [CssVar(Alert.SuccessBg)] public string SuccessBg { get; init; } = Var(Css.Tokens.Color.SuccessContainer);
    /// <summary>Text and icon colour of a success alert.</summary>
    [CssVar(Alert.SuccessColor)] public string SuccessColor { get; init; } = Var(Css.Tokens.Color.OnSuccessContainer);
    /// <summary>Outline of an outlined success alert.</summary>
    [CssVar(Alert.SuccessBorder)] public string SuccessBorder { get; init; } = Var(Css.Tokens.Color.SuccessContainer);
    /// <summary>Fill of a warning alert.</summary>
    [CssVar(Alert.WarningBg)] public string WarningBg { get; init; } = Var(Css.Tokens.Color.WarningContainer);
    /// <summary>Text and icon colour of a warning alert.</summary>
    [CssVar(Alert.WarningColor)] public string WarningColor { get; init; } = Var(Css.Tokens.Color.OnWarningContainer);
    /// <summary>Outline of an outlined warning alert.</summary>
    [CssVar(Alert.WarningBorder)] public string WarningBorder { get; init; } = Var(Css.Tokens.Color.WarningContainer);
    /// <summary>Fill of an error alert.</summary>
    [CssVar(Alert.ErrorBg)] public string ErrorBg { get; init; } = Var(Css.Tokens.Color.ErrorContainer);
    /// <summary>Text and icon colour of an error alert.</summary>
    [CssVar(Alert.ErrorColor)] public string ErrorColor { get; init; } = Var(Css.Tokens.Color.OnErrorContainer);
    /// <summary>Outline of an outlined error alert.</summary>
    [CssVar(Alert.ErrorBorder)] public string ErrorBorder { get; init; } = Var(Css.Tokens.Color.ErrorContainer);
}
