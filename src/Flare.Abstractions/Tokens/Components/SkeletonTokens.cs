using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for skeleton - component-specific geometry read by skeleton.css.</summary>
public sealed record SkeletonTokens
{
    /// <summary>Pulse Min Opacity.</summary>
    [CssVar(SkeletonField.PulseMinOpacity)] public required string PulseMinOpacity { get; init; }

    /// <summary>Wave Opacity.</summary>
    [CssVar(SkeletonField.WaveOpacity)] public required string WaveOpacity { get; init; }
}
