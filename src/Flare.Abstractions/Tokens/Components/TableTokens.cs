using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for table - component-specific geometry read by table.css.</summary>
public sealed record TableTokens
{
    /// <summary>Cell Padding H.</summary>
    [CssVar(TableField.CellPaddingH)] public required string CellPaddingH { get; init; }

    /// <summary>Cell Padding V.</summary>
    [CssVar(TableField.CellPaddingV)] public required string CellPaddingV { get; init; }

    /// <summary>Stripe Opacity.</summary>
    [CssVar(TableField.StripeOpacity)] public required string StripeOpacity { get; init; }
}
