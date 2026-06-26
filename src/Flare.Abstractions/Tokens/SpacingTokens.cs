namespace Flare.Core.Tokens;

/// <summary>
/// Default values for the spacing scale (padding/margin/gap). The step index is a multiple of the
/// 0.125rem (2px) base unit: <c>S2</c> = 0.25rem, <c>S4</c> = 0.5rem, <c>S8</c> = 1rem, and so on.
/// The default ramp (0/2/4/6/8/10/12/16/20/24/32 px for S0..S16) matches the Fluent UI 2 spacing ramp
/// exactly (None/XXS/XS/SNudge/S/MNudge/M/L/XL/XXL/XXXL) and is a superset of the Material Design 3
/// 4dp grid (4/8/12/16/24/32 = S2/S4/S6/S8/S12/S16), so a single default serves both themes.
/// Mode-independent (the same scale serves light and dark). A theme overrides only what it wants
/// via <c>with</c> -- e.g. a denser theme can tighten the whole scale -- and component CSS resolves
/// every spacing through <c>var(--flare-spacing-*)</c>, so the override applies everywhere at once.
/// </summary>
public sealed record SpacingTokens
{
    /// <summary>0 (no space).</summary>
    public string S0 { get; init; } = "0";
    /// <summary>0.125rem (2px).</summary>
    public string S1 { get; init; } = "0.125rem";
    /// <summary>0.25rem (4px).</summary>
    public string S2 { get; init; } = "0.25rem";
    /// <summary>0.375rem (6px).</summary>
    public string S3 { get; init; } = "0.375rem";
    /// <summary>0.5rem (8px).</summary>
    public string S4 { get; init; } = "0.5rem";
    /// <summary>0.625rem (10px).</summary>
    public string S5 { get; init; } = "0.625rem";
    /// <summary>0.75rem (12px).</summary>
    public string S6 { get; init; } = "0.75rem";
    /// <summary>1rem (16px).</summary>
    public string S8 { get; init; } = "1rem";
    /// <summary>1.25rem (20px).</summary>
    public string S10 { get; init; } = "1.25rem";
    /// <summary>1.5rem (24px).</summary>
    public string S12 { get; init; } = "1.5rem";
    /// <summary>2rem (32px).</summary>
    public string S16 { get; init; } = "2rem";
    /// <summary>3rem (48px).</summary>
    public string S24 { get; init; } = "3rem";
    /// <summary>4rem (64px).</summary>
    public string S32 { get; init; } = "4rem";
}
