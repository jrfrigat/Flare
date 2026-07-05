using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>
/// Default values for the spacing scale (padding/margin/gap). The step index is a multiple of the
/// 0.125rem (2px) base unit: <c>S2</c> = 0.25rem, <c>S4</c> = 0.5rem, <c>S8</c> = 1rem, and so on.
/// The default ramp (0/2/4/6/8/10/12/16/20/24/32 px for S0..S16) is an 11-step scale that is a superset
/// of a 4px grid (4/8/12/16/24/32 = S2/S4/S6/S8/S12/S16), so a single default serves both fine-nudge
/// and 4px-grid design systems.
/// Mode-independent (the same scale serves light and dark). A theme overrides only what it wants
/// via <c>with</c> -- e.g. a denser theme can tighten the whole scale -- and component CSS resolves
/// every spacing through <c>var(--flare-spacing-*)</c>, so the override applies everywhere at once.
/// </summary>
public sealed record SpacingTokens
{
    /// <summary>0 (no space).</summary>
    [CssVar(Spacing.S0)] public required string S0 { get; init; }
    /// <summary>0.125rem (2px).</summary>
    [CssVar(Spacing.S1)] public required string S1 { get; init; }
    /// <summary>0.25rem (4px).</summary>
    [CssVar(Spacing.S2)] public required string S2 { get; init; }
    /// <summary>0.375rem (6px).</summary>
    [CssVar(Spacing.S3)] public required string S3 { get; init; }
    /// <summary>0.5rem (8px).</summary>
    [CssVar(Spacing.S4)] public required string S4 { get; init; }
    /// <summary>0.625rem (10px).</summary>
    [CssVar(Spacing.S5)] public required string S5 { get; init; }
    /// <summary>0.75rem (12px).</summary>
    [CssVar(Spacing.S6)] public required string S6 { get; init; }
    /// <summary>1rem (16px).</summary>
    [CssVar(Spacing.S8)] public required string S8 { get; init; }
    /// <summary>1.25rem (20px).</summary>
    [CssVar(Spacing.S10)] public required string S10 { get; init; }
    /// <summary>1.5rem (24px).</summary>
    [CssVar(Spacing.S12)] public required string S12 { get; init; }
    /// <summary>2rem (32px).</summary>
    [CssVar(Spacing.S16)] public required string S16 { get; init; }
    /// <summary>3rem (48px).</summary>
    [CssVar(Spacing.S24)] public required string S24 { get; init; }
    /// <summary>4rem (64px).</summary>
    [CssVar(Spacing.S32)] public required string S32 { get; init; }
}
