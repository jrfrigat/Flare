namespace Flare.Css.Tokens;

/// <summary>
/// CSS custom properties for the spacing scale (padding/margin/gap). The step index is a
/// multiple of the 0.125rem (2px) base unit, so the names map 1:1 to physical sizes:
/// <c>2</c> = 0.25rem, <c>4</c> = 0.5rem, <c>8</c> = 1rem, etc. The values are theme-overridable
/// (a denser theme can shrink the whole scale), so consumers should reference these tokens
/// instead of hard-coding rem/px literals. Defaults live in <see cref="Flare.Abstractions.Tokens.SpacingTokens"/>.
/// </summary>
public static class Spacing
{
    /// <summary>0 (no space).</summary>
    public const string S0 = "--flare-spacing-0";
    /// <summary>0.125rem (2px).</summary>
    public const string S1 = "--flare-spacing-1";
    /// <summary>0.25rem (4px).</summary>
    public const string S2 = "--flare-spacing-2";
    /// <summary>0.375rem (6px).</summary>
    public const string S3 = "--flare-spacing-3";
    /// <summary>0.5rem (8px).</summary>
    public const string S4 = "--flare-spacing-4";
    /// <summary>0.625rem (10px).</summary>
    public const string S5 = "--flare-spacing-5";
    /// <summary>0.75rem (12px).</summary>
    public const string S6 = "--flare-spacing-6";
    /// <summary>1rem (16px).</summary>
    public const string S8 = "--flare-spacing-8";
    /// <summary>1.25rem (20px).</summary>
    public const string S10 = "--flare-spacing-10";
    /// <summary>1.5rem (24px).</summary>
    public const string S12 = "--flare-spacing-12";
    /// <summary>2rem (32px).</summary>
    public const string S16 = "--flare-spacing-16";
    /// <summary>3rem (48px).</summary>
    public const string S24 = "--flare-spacing-24";
    /// <summary>4rem (64px).</summary>
    public const string S32 = "--flare-spacing-32";
}
