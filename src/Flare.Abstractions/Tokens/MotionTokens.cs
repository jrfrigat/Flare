using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>Animation duration and easing values shared by component transitions.</summary>
public sealed record MotionTokens
{
    /// <summary>Duration short 1 token.</summary>
    [CssVar(Motion.DurationShort1)] public required string DurationShort1 { get; init; }
    /// <summary>Duration short 2 token.</summary>
    [CssVar(Motion.DurationShort2)] public required string DurationShort2 { get; init; }
    /// <summary>Duration medium 1 token.</summary>
    [CssVar(Motion.DurationMedium1)] public required string DurationMedium1 { get; init; }
    /// <summary>Duration medium 2 token.</summary>
    [CssVar(Motion.DurationMedium2)] public required string DurationMedium2 { get; init; }
    /// <summary>Duration long 1 token.</summary>
    [CssVar(Motion.DurationLong1)] public required string DurationLong1 { get; init; }
    /// <summary>Duration long 2 token.</summary>
    [CssVar(Motion.DurationLong2)] public required string DurationLong2 { get; init; }
    /// <summary>Easing standard token.</summary>
    [CssVar(Motion.EasingStandard)] public required string EasingStandard { get; init; }
    /// <summary>Easing decelerate token.</summary>
    [CssVar(Motion.EasingDecelerate)] public required string EasingDecelerate { get; init; }
    /// <summary>Easing accelerate token.</summary>
    [CssVar(Motion.EasingAccelerate)] public required string EasingAccelerate { get; init; }
    /// <summary>Easing emphasized token.</summary>
    [CssVar(Motion.EasingEmphasized)] public required string EasingEmphasized { get; init; }
}
