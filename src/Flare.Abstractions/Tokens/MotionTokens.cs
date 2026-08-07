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
    /// <summary>Duration short 3 token.</summary>
    [CssVar(Motion.DurationShort3)] public required string DurationShort3 { get; init; }
    /// <summary>Duration short 4 token.</summary>
    [CssVar(Motion.DurationShort4)] public required string DurationShort4 { get; init; }
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

    /// <summary>
    /// Easing for a fast spatial spring: the movement of a small element, such as a switch thumb or a
    /// checkbox tick. A design language with no spring in it should set this to its ordinary easing.
    /// </summary>
    /// <remarks>
    /// A spring is a shape plus the time it takes to settle, so this must be paired with
    /// <see cref="DurationSpringFast"/>. Setting one without the other truncates or stretches the
    /// curve, which is what turns an overshoot into a visible snap.
    /// </remarks>
    [CssVar(Motion.EasingSpringFast)] public required string EasingSpringFast { get; init; }
    /// <summary>
    /// Easing for the default spatial spring: the movement or shape change of a component-sized
    /// element, such as a button morphing its corners or a chip being selected. Pair with
    /// <see cref="DurationSpring"/>.
    /// </summary>
    [CssVar(Motion.EasingSpring)] public required string EasingSpring { get; init; }
    /// <summary>
    /// Easing for a slow spatial spring: the movement of a large surface, such as a sheet or a
    /// full-screen transition. Pair with <see cref="DurationSpringSlow"/>.
    /// </summary>
    [CssVar(Motion.EasingSpringSlow)] public required string EasingSpringSlow { get; init; }

    /// <summary>Settling time of the fast spatial spring; belongs with <see cref="EasingSpringFast"/>.</summary>
    [CssVar(Motion.DurationSpringFast)] public required string DurationSpringFast { get; init; }
    /// <summary>Settling time of the default spatial spring; belongs with <see cref="EasingSpring"/>.</summary>
    [CssVar(Motion.DurationSpring)] public required string DurationSpring { get; init; }
    /// <summary>Settling time of the slow spatial spring; belongs with <see cref="EasingSpringSlow"/>.</summary>
    [CssVar(Motion.DurationSpringSlow)] public required string DurationSpringSlow { get; init; }
}
