using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>
/// The border scale: how a design language draws the rules between and around things. Deliberately five
/// tokens for the whole library rather than one per component - a separator is one decision, and a
/// language that answers it with a hairline, a heavier rule, a dashed line or nothing at all should not
/// have to restate that answer in eighty places.
/// </summary>
public sealed record BorderTokens
{
    /// <summary>Width of a standard rule. Set to <c>0</c> for a language that separates by tone and
    /// elevation alone.</summary>
    [CssVar(Border.Width)] public required string Width { get; init; }

    /// <summary>Width of an emphasised rule - a selected row, a drop target, the active edge of a
    /// stepper. Distinct from <see cref="Width"/> because emphasis has to remain visible in a language
    /// that thins or removes the standard rule.</summary>
    [CssVar(Border.WidthEmphasis)] public required string WidthEmphasis { get; init; }

    /// <summary>Line style shared by both widths (<c>solid</c>, <c>dashed</c>, ...).</summary>
    [CssVar(Border.Style)] public required string Style { get; init; }

    /// <summary>The divider rule as a <c>border</c> shorthand - the hairline between rows, cells,
    /// sections and panes. Compose it from the width, style and outline-variant colour rather than
    /// restating a length, so a change to <see cref="Width"/> carries here.</summary>
    [CssVar(Border.Divider)] public required string Divider { get; init; }

    /// <summary>The container rule as a <c>border</c> shorthand - the edge a surface draws around
    /// itself. Stronger than <see cref="Divider"/>; typically the outline colour rather than its
    /// variant.</summary>
    [CssVar(Border.Outline)] public required string Outline { get; init; }
}
