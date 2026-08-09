using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the alternating-row stripe. Shared rather than per-component: a table, a data
/// grid and a description list all mean the same thing by "striped", and a design language that has
/// an opinion about it has one opinion, not three.
/// </summary>
public sealed record StripeTokens
{
    /// <summary>Paint for every second row - a colour including alpha, or an image. It composites
    /// over the surface behind it, so a translucent wash and a solid container step are both
    /// expressible; the value is the whole paint rather than an opacity, because the two in-box
    /// answers to "what is a stripe" differ in more than strength.</summary>
    [CssVar(StripeField.Background)] public required string Background { get; init; }
}
