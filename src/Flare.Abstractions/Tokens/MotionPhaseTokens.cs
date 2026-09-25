namespace Flare.Abstractions.Tokens;

/// <summary>
/// How a surface moves through one phase of its presence - appearing or leaving. The values describe the
/// surface in its HIDDEN position; the core interpolates between that and the resting surface, so a theme
/// states the character of the movement and never writes a keyframe.
/// </summary>
/// <remarks>
/// A phase is two tracks that run independently from the moment it is triggered: the shape track
/// (<see cref="Offset"/>, <see cref="Scale"/>, <see cref="Reveal"/>, <see cref="Blur"/>) timed by
/// <see cref="Delay"/>, <see cref="Duration"/> and <see cref="Easing"/>, and the fade track timed by
/// <see cref="FadeDelay"/>, <see cref="FadeDuration"/> and <see cref="FadeEasing"/>. A language that fades and
/// moves together sets both tracks alike; one that clips open and fades in a fraction of the time, or collapses
/// only after it has faded, sets them apart. A theme that wants no movement at all sets both durations to
/// <c>0ms</c>, which leaves the surface appearing or vanishing at once.
/// </remarks>
public sealed record MotionPhaseTokens
{
    /// <summary>
    /// Wait between the trigger and the start of the shape track - the hover delay before a tooltip appears,
    /// or a collapse that waits for its content to fade first.
    /// </summary>
    public required string Delay { get; init; }

    /// <summary>Time the shape track takes: the travel of the offset, scale, reveal and blur.</summary>
    public required string Duration { get; init; }

    /// <summary>Timing curve of the shape track.</summary>
    public required string Easing { get; init; }

    /// <summary>
    /// Distance of the hidden surface from its resting place, along the axis the component emerges on and
    /// toward the side it emerges from: the anchor of a menu, popover or tooltip, the attached edge of a
    /// sheet, drawer or snackbar, and above for a dialog. A length, or a percentage of the surface's own size
    /// along that axis (<c>100%</c> starts it fully outside); <c>0px</c> means it does not travel.
    /// </summary>
    public required string Offset { get; init; }

    /// <summary>Scale of the hidden surface, a number (<c>0.85</c>); <c>1</c> means it does not grow.</summary>
    public required string Scale { get; init; }

    /// <summary>Opacity of the hidden surface, a number from <c>0</c> to <c>1</c>; <c>1</c> means it does not fade.</summary>
    public required string Opacity { get; init; }

    /// <summary>
    /// Share of the surface still visible in the hidden position, clipped from the side it emerges from - a
    /// container that grows open from a third of its height. A percentage; <c>100%</c> means no clip.
    /// </summary>
    public required string Reveal { get; init; }

    /// <summary>Blur radius of the hidden surface, a length; <c>0px</c> means it stays sharp.</summary>
    public required string Blur { get; init; }

    /// <summary>Wait between the trigger and the start of the fade track.</summary>
    public required string FadeDelay { get; init; }

    /// <summary>Time the fade track takes to move between <see cref="Opacity"/> and full opacity.</summary>
    public required string FadeDuration { get; init; }

    /// <summary>Timing curve of the fade track.</summary>
    public required string FadeEasing { get; init; }
}
