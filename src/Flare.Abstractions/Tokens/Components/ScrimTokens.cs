using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for scrim - component-specific geometry read by scrim.css.</summary>
public sealed record ScrimTokens
{
    /// <summary>Opacity.</summary>
    [CssVar(ScrimField.Opacity)] public required string Opacity { get; init; }
}
