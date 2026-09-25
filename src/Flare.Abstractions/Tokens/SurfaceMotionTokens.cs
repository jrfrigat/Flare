namespace Flare.Abstractions.Tokens;

/// <summary>
/// The appearing and leaving motion of one family of transient surfaces. The two phases are separate because
/// design languages rarely mirror them: a surface that takes its time to arrive usually leaves quickly, and
/// some leave with no motion at all.
/// </summary>
public sealed record SurfaceMotionTokens
{
    /// <summary>How the surface arrives.</summary>
    public required MotionPhaseTokens Enter { get; init; }

    /// <summary>How the surface leaves.</summary>
    public required MotionPhaseTokens Exit { get; init; }
}
