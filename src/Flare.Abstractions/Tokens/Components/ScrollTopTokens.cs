using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for scroll - component-specific geometry read by scroll.css.</summary>
public sealed record ScrollTopTokens
{
    /// <summary>Top Inset.</summary>
    [CssVar(ScrollTopField.TopInset)] public string TopInset { get; init; } = Var(Css.Tokens.Spacing.S12);

    /// <summary>Top Size.</summary>
    [CssVar(ScrollTopField.TopSize)] public required string TopSize { get; init; }

    /// <summary>Height of an app's own bottom bar, added as a margin under the button so it sits above
    /// that bar's actions. An app that has no such bar leaves it at zero.</summary>
    [CssVar(ScrollTopField.PanelOffset)] public string PanelOffset { get; init; } = Var(Css.Tokens.Spacing.S0);
}
