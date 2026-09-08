using Flare.Theme.FluentUI2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Guards the tokens <c>FlareProgress</c> reads from C# rather than from CSS.
///
/// Most component geometry is consumed by a stylesheet, where CssAudit can see it. These few cannot
/// be: the wavy path is computed in the component because an SVG path has to be built from numbers.
/// The public token constants and values live in the supporting theme package; this guard proves that
/// those theme-owned extension values still reach the generic renderer through the flattened map.
///
/// CssAudit could not catch it: every name existed and was in sync. Only asking "does the value the
/// component looks up actually exist where it looks it up" catches this.
/// </summary>
public sealed class ProgressTokenReachTests
{
    private static Dictionary<string, string> Flattened() =>
        new MaterialDesign3ExpressiveTheme().Design.FlattenDesign();

    [Theory]
    // Exactly the theme extension names FlareProgress reads from the flattened design.
    [InlineData(Css.Tokens.ProgressField.CircularGap)]
    [InlineData(Css.Tokens.Md3e.Progress.Height)]
    [InlineData(Css.Tokens.Md3e.Progress.Length)]
    [InlineData(Css.Tokens.Md3e.Progress.IndeterminateLength)]
    [InlineData(Css.Tokens.Md3e.Progress.Amplitude)]
    [InlineData(Css.Tokens.Md3e.Progress.LinearIndeterminateDuration)]
    [InlineData(Css.Tokens.Md3e.Progress.Speed)]
    [InlineData(Css.Tokens.Md3e.Progress.RingCount)]
    [InlineData(Css.Tokens.Md3e.Progress.RingLength)]
    [InlineData(Css.Tokens.Md3e.Progress.RingAmplitude)]
    public void EveryTokenTheComponentLooksUp_IsInTheFlattenedDesign(string token)
    {
        Assert.True(Flattened().ContainsKey(token),
            $"{token} is read by FlareProgress but never reaches the flattened design, so the component "
            + "silently uses its fallback. Either the token moved, or the reader is looking in the wrong place.");
    }

    [Fact]
    public void Md3Expressive_ProvidesTheWaveRendererConfiguration()
    {
        var flat = Flattened();

        Assert.Equal("20px", flat[Css.Tokens.Md3e.Progress.IndeterminateLength]);
        Assert.Equal("15px", flat[Css.Tokens.Md3e.Progress.RingLength]);
        Assert.Equal("1750ms", flat[Css.Tokens.Md3e.Progress.LinearIndeterminateDuration]);
        Assert.Equal("1500ms", flat[Css.Tokens.Md3e.Progress.CircularIndeterminateRotationDuration]);
        Assert.Equal("6000ms", flat[Css.Tokens.Md3e.Progress.CircularIndeterminateProgressDuration]);

        // And the ring must break between the indicator and the remaining track.
        Assert.NotEqual("0", flat[Css.Tokens.ProgressField.CircularGap].TrimEnd('p', 'x'));
    }

    [Fact]
    public void FlatThemes_DoNotEmitWaveRendererTokens()
    {
        foreach (var design in new[]
                 {
                     new MaterialDesign3Theme().Design.FlattenDesign(),
                     new FluentUI2Theme().Design.FlattenDesign(),
                 })
        {
            Assert.False(design.ContainsKey(Css.Tokens.Md3e.Progress.Length));
            Assert.False(design.ContainsKey(Css.Tokens.Md3e.Progress.LinearIndeterminateDuration));
        }
    }
}
