using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Guards the tokens <c>FlareProgress</c> reads from C# rather than from CSS.
///
/// Most component geometry is consumed by a stylesheet, where CssAudit can see it. These few cannot
/// be: the wavy path and the ring's gap are computed in the component, because an SVG path has to be
/// built from numbers. That makes them the one place where a token can be emitted correctly, named
/// correctly, and still never reach the paint - which is exactly what happened. The values moved from
/// <c>DesignTokens.Extended</c> into the typed <c>ProgressTokens</c> record during the token-mandate
/// work, and the component's reader kept looking only in <c>Extended</c>. Every read silently fell
/// through to its fallback, so <c>Wavy</c> drew a flat bar in every theme and the ring drew no gap.
///
/// CssAudit could not catch it: every name existed and was in sync. Only asking "does the value the
/// component looks up actually exist where it looks it up" catches this.
/// </summary>
public sealed class ProgressTokenReachTests
{
    private static Dictionary<string, string> Flattened() =>
        new MaterialDesign3ExpressiveTheme().Design.FlattenDesign();

    [Theory]
    // Exactly the names FlareProgress passes to ReadTokenNum/ReadTokenStr.
    [InlineData("--flare-progress-circular-gap")]
    [InlineData("--flare-progress-wavy-enabled")]
    [InlineData("--flare-progress-wavy-height")]
    [InlineData("--flare-progress-wave-length")]
    [InlineData("--flare-progress-wave-amplitude")]
    [InlineData("--flare-progress-wave-speed")]
    [InlineData("--flare-progress-ring-waves")]
    [InlineData("--flare-progress-ring-wave-amplitude")]
    public void EveryTokenTheComponentLooksUp_IsInTheFlattenedDesign(string token)
    {
        Assert.True(Flattened().ContainsKey(token),
            $"{token} is read by FlareProgress but never reaches the flattened design, so the component "
            + "silently uses its fallback. Either the token moved, or the reader is looking in the wrong place.");
    }

    [Fact]
    public void Md3Expressive_ActuallyTurnsTheWavyProgressOn()
    {
        var flat = Flattened();

        // Expressive's signature loading state. "0" here means Wavy renders a plain bar, which is
        // indistinguishable from the parameter being ignored - the shape the reported bug took.
        Assert.Equal("1", flat["--flare-progress-wavy-enabled"]);

        // And the ring must break between the indicator and the remaining track.
        Assert.NotEqual("0", flat["--flare-progress-circular-gap"].TrimEnd('p', 'x'));
    }
}
