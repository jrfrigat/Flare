using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for table - component-specific geometry read by table.css.</summary>
public sealed record TableTokens
{
    /// <summary>Cell Padding H.</summary>
    [CssVar(TableField.CellPaddingH)] public string CellPaddingH { get; init; } = "var(--flare-spacing-8)";

    /// <summary>Cell Padding V.</summary>
    [CssVar(TableField.CellPaddingV)] public string CellPaddingV { get; init; } = "var(--flare-spacing-6)";
}
