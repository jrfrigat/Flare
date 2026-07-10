namespace Flare.Components;

/// <summary>
/// Strongly typed range comparisons over the <see cref="Breakpoint"/> scale. These replace the
/// range pseudo-values (e.g. <c>SmAndDown</c>) that some libraries mix into the enum: the enum stays a
/// clean list of discrete tiers and the "and-up / and-down / between" semantics live here.
/// </summary>
public static class BreakpointExtensions
{
    /// <summary>True when <paramref name="current"/> is <paramref name="other"/> or a larger tier
    /// (the CSS <c>min-width: {other}</c> case).</summary>
    public static bool IsAtLeast(this Breakpoint current, Breakpoint other) => current >= other;

    /// <summary>True when <paramref name="current"/> is <paramref name="other"/> or a smaller tier
    /// (the CSS <c>max-width</c> / and-down case).</summary>
    public static bool IsAtMost(this Breakpoint current, Breakpoint other) => current <= other;

    /// <summary>True when <paramref name="current"/> is within the inclusive range
    /// <paramref name="min"/>..<paramref name="max"/>.</summary>
    public static bool IsBetween(this Breakpoint current, Breakpoint min, Breakpoint max) =>
        current >= min && current <= max;

    /// <summary>The lower bound (min-width, px) of this tier in the default scale.</summary>
    public static int MinWidth(this Breakpoint breakpoint) => FlareBreakpoints.MinWidth(breakpoint);

    /// <summary>
    /// A <c>min-width</c> CSS media-query string for this tier, e.g. <c>(min-width: 960px)</c> for
    /// <see cref="Breakpoint.Md"/>. Pair with <c>IBrowserViewportService.MatchesAsync</c>.
    /// </summary>
    public static string ToMinWidthQuery(this Breakpoint breakpoint) =>
        $"(min-width: {FlareBreakpoints.MinWidth(breakpoint)}px)";
}
