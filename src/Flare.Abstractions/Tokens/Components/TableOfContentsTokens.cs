using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareTableOfContents</c> / <c>FlareOnThisPage</c>. The defaults render the
/// MD3 look (a soft "pill" behind the active link); FluentUI2 overrides them to the rail look (a thin
/// vertical line with a left marker bar on the active link).
/// </summary>
public sealed record TableOfContentsTokens
{
    /// <summary>Active (current section) link text color. MD3 = on-secondary-container (text on the pill).</summary>
    [CssVar(TableOfContents.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Color of inactive links. MD3 = on-surface-variant.</summary>
    [CssVar(TableOfContents.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>Color of the title ("On this page"). MD3 = on-surface-variant.</summary>
    [CssVar(TableOfContents.TitleColor)] public required string TitleColor { get; init; }

    /// <summary>Color of the vertical rail beside the links. MD3 has no rail (transparent).</summary>
    [CssVar(TableOfContents.RailColor)] public required string RailColor { get; init; }

    /// <summary>Width of the vertical rail. MD3 = 0 (no rail); Fluent = 1px.</summary>
    [CssVar(TableOfContents.RailWidth)] public required string RailWidth { get; init; }

    /// <summary>Background behind the active link. MD3 = secondary-container (the pill); Fluent = transparent.</summary>
    [CssVar(TableOfContents.ActiveBg)] public required string ActiveBg { get; init; }

    /// <summary>Corner radius of the active-link surface. MD3 = full (pill); Fluent = 0.</summary>
    [CssVar(TableOfContents.ActiveRadius)] public required string ActiveRadius { get; init; }

    /// <summary>Width of the active-link left marker bar. MD3 = 0 (pill, no bar); Fluent = 3px.</summary>
    [CssVar(TableOfContents.MarkerWidth)] public required string MarkerWidth { get; init; }

    /// <summary>Horizontal padding inside each link (gives the MD3 pill its width). MD3 = 0.75rem; Fluent = 0.</summary>
    [CssVar(TableOfContents.LinkPadX)] public required string LinkPadX { get; init; }

    /// <summary>Indent applied per heading-depth level.</summary>
    [CssVar(TableOfContents.Indent)] public required string Indent { get; init; }
}
