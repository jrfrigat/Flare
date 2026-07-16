using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>
/// The spacing scale (padding/margin/gap). It is an INDEX scale: the number in each member name is how many
/// base units that step is worth, so <c>S4</c> is twice <c>S2</c> and half <c>S8</c> at every theme. The theme
/// owns the base unit, and therefore the whole ramp - the core states the proportions, never the sizes.
/// The 13 steps are a superset of an even-index grid (<c>S2</c>/<c>S4</c>/<c>S6</c>/<c>S8</c>/<c>S12</c>/
/// <c>S16</c>), so one scale serves both fine-nudge and grid-aligned design systems.
/// Mode-independent (the same scale serves light and dark). A theme overrides only what it wants
/// via <c>with</c> -- e.g. a denser theme can tighten the whole scale -- and component CSS resolves
/// every spacing through <c>var(--flare-spacing-*)</c>, so the override applies everywhere at once.
/// </summary>
public sealed record SpacingTokens
{
    /// <summary>No space. Kept as a token so a layout can say "no gap here" through the scale.</summary>
    [CssVar(Spacing.S0)] public required string S0 { get; init; }
    /// <summary>One base unit - the finest nudge the scale offers.</summary>
    [CssVar(Spacing.S1)] public required string S1 { get; init; }
    /// <summary>Two base units.</summary>
    [CssVar(Spacing.S2)] public required string S2 { get; init; }
    /// <summary>Three base units.</summary>
    [CssVar(Spacing.S3)] public required string S3 { get; init; }
    /// <summary>Four base units.</summary>
    [CssVar(Spacing.S4)] public required string S4 { get; init; }
    /// <summary>Five base units.</summary>
    [CssVar(Spacing.S5)] public required string S5 { get; init; }
    /// <summary>Six base units.</summary>
    [CssVar(Spacing.S6)] public required string S6 { get; init; }
    /// <summary>Eight base units.</summary>
    [CssVar(Spacing.S8)] public required string S8 { get; init; }
    /// <summary>Ten base units.</summary>
    [CssVar(Spacing.S10)] public required string S10 { get; init; }
    /// <summary>Twelve base units.</summary>
    [CssVar(Spacing.S12)] public required string S12 { get; init; }
    /// <summary>Sixteen base units.</summary>
    [CssVar(Spacing.S16)] public required string S16 { get; init; }
    /// <summary>Twenty-four base units.</summary>
    [CssVar(Spacing.S24)] public required string S24 { get; init; }
    /// <summary>Thirty-two base units - the coarsest step.</summary>
    [CssVar(Spacing.S32)] public required string S32 { get; init; }
}
