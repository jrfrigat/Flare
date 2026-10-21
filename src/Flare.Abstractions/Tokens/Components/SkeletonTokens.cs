using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for skeleton - component-specific geometry read by skeleton.css.</summary>
public sealed record SkeletonTokens
{
    /// <summary>Pulse Min Opacity.</summary>
    [CssVar(SkeletonField.PulseMinOpacity)] public required string PulseMinOpacity { get; init; }

    /// <summary>Wave Opacity.</summary>
    [CssVar(SkeletonField.WaveOpacity)] public required string WaveOpacity { get; init; }

    /// <summary>Height of a Rect placeholder that sets no <c>Height</c> of its own, so the smallest
    /// skeleton still draws something. Defaults to the spacing scale's body-text step.</summary>
    [CssVar(SkeletonField.RectHeight)] public string RectHeight { get; init; } = Var(Css.Tokens.Spacing.S8);
}
