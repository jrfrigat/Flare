using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareTableOfContents</c> / <c>FlareOnThisPage</c>. One token-driven layout
/// renders either look a theme may want: a soft "pill" behind the active link, or a "rail" (a thin
/// vertical line with a left marker bar on the active link).
/// </summary>
public sealed record TableOfContentsTokens
{
    /// <summary>Active weight.</summary>
    [CssVar(TableOfContents.ActiveWeight)] public required string ActiveWeight { get; init; }
    /// <summary>Hover bg opacity.</summary>
    [CssVar(TableOfContents.HoverBgOpacity)] public required string HoverBgOpacity { get; init; }
    /// <summary>Line height.</summary>
    [CssVar(TableOfContents.LineHeight)] public required string LineHeight { get; init; }
    /// <summary>Title tracking.</summary>
    [CssVar(TableOfContents.TitleTracking)] public required string TitleTracking { get; init; }
    /// <summary>Title weight.</summary>
    [CssVar(TableOfContents.TitleWeight)] public required string TitleWeight { get; init; }
    /// <summary>Active (current section) link text color.</summary>
    [CssVar(TableOfContents.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Color of inactive links.</summary>
    [CssVar(TableOfContents.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>Color of the title (e.g. "On this page").</summary>
    [CssVar(TableOfContents.TitleColor)] public required string TitleColor { get; init; }

    /// <summary>Color of the vertical rail beside the links; transparent for no rail.</summary>
    [CssVar(TableOfContents.RailColor)] public required string RailColor { get; init; }

    /// <summary>Width of the vertical rail; 0 for no rail.</summary>
    [CssVar(TableOfContents.RailWidth)] public required string RailWidth { get; init; }

    /// <summary>Background behind the active link (the pill fill); transparent for the rail look.</summary>
    [CssVar(TableOfContents.ActiveBg)] public required string ActiveBg { get; init; }

    /// <summary>Corner radius of the active-link surface (full for a pill, 0 for the rail look).</summary>
    [CssVar(TableOfContents.ActiveRadius)] public required string ActiveRadius { get; init; }

    /// <summary>Width of the active-link left marker bar; 0 for no bar (the pill look).</summary>
    [CssVar(TableOfContents.MarkerWidth)] public required string MarkerWidth { get; init; }

    /// <summary>Horizontal padding inside each link (gives the pill its width); 0 for the rail look.</summary>
    [CssVar(TableOfContents.LinkPadX)] public required string LinkPadX { get; init; }

    /// <summary>Indent applied per heading-depth level.</summary>
    [CssVar(TableOfContents.Indent)] public required string Indent { get; init; }
}
