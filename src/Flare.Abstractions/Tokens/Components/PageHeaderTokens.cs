using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for the page header - its spacing, surface and eyebrow casing, read by pageheader.css.</summary>
public sealed record PageHeaderTokens
{
    /// <summary>Padding around the header, as a CSS <c>padding</c> value.</summary>
    [CssVar(PageHeaderField.Padding)] public string Padding { get; init; } = "var(--flare-spacing-16) 0";

    /// <summary>Space between the eyebrow, the title and the description.</summary>
    [CssVar(PageHeaderField.Gap)] public string Gap { get; init; } = "var(--flare-spacing-4)";

    /// <summary>Space between the text block and the actions, across and, on a narrow header, down.</summary>
    [CssVar(PageHeaderField.ActionsGap)] public string ActionsGap { get; init; } = "var(--flare-spacing-16)";

    /// <summary>Header background, as a CSS <c>background</c> value.</summary>
    [CssVar(PageHeaderField.Background)] public required string Background { get; init; }

    /// <summary>Line under the header, as a CSS <c>border-bottom</c> value.</summary>
    [CssVar(PageHeaderField.BorderBottom)] public required string BorderBottom { get; init; }

    /// <summary>Casing of the eyebrow above the title, as a CSS <c>text-transform</c> value.</summary>
    [CssVar(PageHeaderField.EyebrowTextTransform)] public required string EyebrowTextTransform { get; init; }
}
