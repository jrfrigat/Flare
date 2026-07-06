using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for link - component-specific geometry read by link.css.</summary>
public sealed record LinkTokens
{
    /// <summary>Focus Ring Width.</summary>
    [CssVar(LinkField.FocusRingWidth)] public required string FocusRingWidth { get; init; }

    /// <summary>Hover Opacity.</summary>
    [CssVar(LinkField.HoverOpacity)] public required string HoverOpacity { get; init; }
}
