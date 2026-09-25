using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>
/// Animation durations, timing curves and surface motion shared by component transitions. Every value names a
/// ROLE - what the motion is for - rather than a step on one design language's scale, so a language with nine
/// curves and one with six both fill it without losing a curve or repeating one.
/// </summary>
public sealed record MotionTokens
{
    // Curves: three levels of expression, each for movement that stays on screen, that arrives and that leaves,
    // plus a constant rate.

    /// <summary>Curve for an ordinary movement that begins and ends on screen.</summary>
    [CssVar(Motion.EasingStandard)] public required string EasingStandard { get; init; }
    /// <summary>Curve for an ordinary element arriving: fast at first, settling at rest.</summary>
    [CssVar(Motion.EasingDecelerate)] public required string EasingDecelerate { get; init; }
    /// <summary>Curve for an ordinary element leaving: gathering speed as it goes.</summary>
    [CssVar(Motion.EasingAccelerate)] public required string EasingAccelerate { get; init; }
    /// <summary>Curve for a prominent movement that begins and ends on screen, drawing the eye.</summary>
    [CssVar(Motion.EasingEmphasized)] public required string EasingEmphasized { get; init; }
    /// <summary>Curve for a prominent element arriving, such as a dialog or a sheet.</summary>
    [CssVar(Motion.EasingEmphasizedDecelerate)] public required string EasingEmphasizedDecelerate { get; init; }
    /// <summary>Curve for a prominent element leaving.</summary>
    [CssVar(Motion.EasingEmphasizedAccelerate)] public required string EasingEmphasizedAccelerate { get; init; }
    /// <summary>Curve for a quiet movement that begins and ends on screen, gentler than the standard one.</summary>
    [CssVar(Motion.EasingSubtle)] public required string EasingSubtle { get; init; }
    /// <summary>Curve for a quiet element arriving.</summary>
    [CssVar(Motion.EasingSubtleDecelerate)] public required string EasingSubtleDecelerate { get; init; }
    /// <summary>Curve for a quiet element leaving.</summary>
    [CssVar(Motion.EasingSubtleAccelerate)] public required string EasingSubtleAccelerate { get; init; }
    /// <summary>Constant-rate curve, for loops and for fades that must not ease.</summary>
    [CssVar(Motion.EasingLinear)] public required string EasingLinear { get; init; }

    // Durations by role.

    /// <summary>Time of the feedback to hover, press and focus: a state layer, a color or an outline changing.</summary>
    [CssVar(Motion.DurationStateChange)] public required string DurationStateChange { get; init; }
    /// <summary>Time of a small element moving in place: a switch thumb, a check mark, a selection indicator.</summary>
    [CssVar(Motion.DurationSmallMove)] public required string DurationSmallMove { get; init; }
    /// <summary>Time for a small in-page region to appear, such as a helper line or a row detail.</summary>
    [CssVar(Motion.DurationEnterSmall)] public required string DurationEnterSmall { get; init; }
    /// <summary>Time for a medium in-page region to appear, such as an expanding accordion panel or a tab's content.</summary>
    [CssVar(Motion.DurationEnterMedium)] public required string DurationEnterMedium { get; init; }
    /// <summary>Time for a large in-page region to appear, such as a whole pane or a full-width panel.</summary>
    [CssVar(Motion.DurationEnterLarge)] public required string DurationEnterLarge { get; init; }
    /// <summary>Time for a small in-page region to go away.</summary>
    [CssVar(Motion.DurationExitSmall)] public required string DurationExitSmall { get; init; }
    /// <summary>Time for a medium in-page region to go away.</summary>
    [CssVar(Motion.DurationExitMedium)] public required string DurationExitMedium { get; init; }
    /// <summary>Time for a large in-page region to go away.</summary>
    [CssVar(Motion.DurationExitLarge)] public required string DurationExitLarge { get; init; }
    /// <summary>Period of one turn of a spinner - a loop, not a transition.</summary>
    [CssVar(Motion.DurationCycle)] public required string DurationCycle { get; init; }
    /// <summary>Period of one pass of a long loop, such as an indeterminate progress bar.</summary>
    [CssVar(Motion.DurationCycleLong)] public required string DurationCycleLong { get; init; }

    // Transient surfaces: each family appears and leaves in its own way. Compound tokens - each phase expands
    // to several variables in CssVarMap.FlattenDesign, so they carry no [CssVar].

    /// <summary>Motion of a modal dialog and its scrim.</summary>
    public required SurfaceMotionTokens Dialog { get; init; }
    /// <summary>Motion of a sheet attached to an edge of the screen.</summary>
    public required SurfaceMotionTokens Sheet { get; init; }
    /// <summary>Motion of a menu opening from its anchor.</summary>
    public required SurfaceMotionTokens Menu { get; init; }
    /// <summary>
    /// Motion of an anchored popover: the panel of a select, an autocomplete or a date picker as well as a
    /// free popover.
    /// </summary>
    public required SurfaceMotionTokens Popover { get; init; }
    /// <summary>Motion of a tooltip, including the hover delays before it appears and before it goes.</summary>
    public required SurfaceMotionTokens Tooltip { get; init; }
    /// <summary>Motion of a snackbar or toast.</summary>
    public required SurfaceMotionTokens Snackbar { get; init; }
    /// <summary>Motion of a modal navigation drawer.</summary>
    public required SurfaceMotionTokens Drawer { get; init; }

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
