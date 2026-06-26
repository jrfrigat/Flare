namespace Flare.Abstractions.Tokens;

/// <summary>Animation duration and easing values shared by component transitions.</summary>
public sealed record MotionTokens
{
    /// <summary>Duration short 1 token.</summary>
    public required string DurationShort1 { get; init; }
    /// <summary>Duration short 2 token.</summary>
    public required string DurationShort2 { get; init; }
    /// <summary>Duration medium 1 token.</summary>
    public required string DurationMedium1 { get; init; }
    /// <summary>Duration medium 2 token.</summary>
    public required string DurationMedium2 { get; init; }
    /// <summary>Duration long 1 token.</summary>
    public required string DurationLong1 { get; init; }
    /// <summary>Duration long 2 token.</summary>
    public required string DurationLong2 { get; init; }
    /// <summary>Easing standard token.</summary>
    public required string EasingStandard { get; init; }
    /// <summary>Easing decelerate token.</summary>
    public required string EasingDecelerate { get; init; }
    /// <summary>Easing accelerate token.</summary>
    public required string EasingAccelerate { get; init; }
    /// <summary>Easing emphasized token.</summary>
    public required string EasingEmphasized { get; init; }
}
