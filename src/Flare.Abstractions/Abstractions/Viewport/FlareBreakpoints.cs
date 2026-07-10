using System.Collections.Generic;
using System.Linq;

namespace Flare.Components;

/// <summary>
/// The default lower-bound (min-width, px) for each <see cref="Breakpoint"/> tier and helpers for
/// resolving a pixel width to a tier. These defaults mirror the CSS media-query boundaries shipped in
/// <c>responsive.css</c> / <c>grid.css</c> so the C# viewport service and the CSS utilities agree on the
/// exact same thresholds. Individual subscriptions may override the map (see
/// <see cref="ViewportSubscribeOptions.Breakpoints"/>).
/// </summary>
public static class FlareBreakpoints
{
    /// <summary>
    /// The built-in tier lower bounds in pixels: Xs 0, Sm 600, Md 960, Lg 1280, Xl 1920, Xxl 2560.
    /// A width is in tier <c>T</c> when it is greater than or equal to <c>T</c>'s bound and below the
    /// next tier's bound.
    /// </summary>
    public static IReadOnlyDictionary<Breakpoint, int> Defaults { get; } = new Dictionary<Breakpoint, int>
    {
        [Breakpoint.Xs] = 0,
        [Breakpoint.Sm] = 600,
        [Breakpoint.Md] = 960,
        [Breakpoint.Lg] = 1280,
        [Breakpoint.Xl] = 1920,
        [Breakpoint.Xxl] = 2560,
    };

    /// <summary>The discrete tiers in ascending order (Xs..Xxl).</summary>
    public static IReadOnlyList<Breakpoint> Tiers { get; } =
        new[] { Breakpoint.Xs, Breakpoint.Sm, Breakpoint.Md, Breakpoint.Lg, Breakpoint.Xl, Breakpoint.Xxl };

    /// <summary>The lower bound (min-width, px) of <paramref name="breakpoint"/> in the default scale.</summary>
    public static int MinWidth(Breakpoint breakpoint) =>
        Defaults.TryGetValue(breakpoint, out var px) ? px : 0;

    /// <summary>
    /// Resolves <paramref name="width"/> (px) to the tier it falls in, using the supplied
    /// <paramref name="definitions"/> or <see cref="Defaults"/> when null. The largest tier whose bound
    /// is met wins; a width below every bound resolves to <see cref="Breakpoint.Xs"/>.
    /// </summary>
    public static Breakpoint FromWidth(double width, IReadOnlyDictionary<Breakpoint, int>? definitions = null)
    {
        var map = definitions ?? Defaults;
        var result = Breakpoint.Xs;
        var best = int.MinValue;
        foreach (var pair in map)
        {
            if (width >= pair.Value && pair.Value >= best)
            {
                best = pair.Value;
                result = pair.Key;
            }
        }
        return result;
    }
}
