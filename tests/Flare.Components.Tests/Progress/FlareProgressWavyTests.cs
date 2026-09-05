using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareProgressWavyTests : FlareTestContext
{
    // Theme that opts into wavy progress (mirrors the MD3 theme tokens).
    private static TokenThemeService WavyTheme() => new(new Dictionary<string, string>
    {
        [Css.Tokens.ProgressField.WavyEnabled] = "1",
        [Css.Tokens.ProgressField.WaveLength] = "40px",
        [Css.Tokens.ProgressField.WaveAmplitude] = "3px",
        [Css.Tokens.ProgressField.CircularGap] = "4px",
    });

    [Fact]
    public void Wavy_NoThemeOptIn_RendersPlain()
    {
        // Point 2: without --flare-progress-wavy-enabled (e.g. Fluent), Wavy="true" stays plain.
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 60d).Add(x => x.Wavy, true));
        Assert.DoesNotContain(Css.Classes.Progress.Wavy, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void Wavy_Linear_OptedIn_UsesSplitTrackWithWaveSvg()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Value, 60d).Add(x => x.Wavy, true));
        var root = cut.Find($".{Css.Classes.Progress.Root}").ClassName;
        Assert.Contains(Css.Classes.Progress.Wavy, root);
        Assert.Contains(Css.Classes.Progress.Split, root);          // inherits gap + stop indicator
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Bar} svg.{Css.Classes.Progress.Wave} path"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Remain}"));
    }

    [Fact]
    public void Wavy_Circular_OptedIn_TrackIsSmoothCircle_IndicatorIsWavyPath()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 60d).Add(x => x.Wavy, true));
        Assert.Contains(Css.Classes.Progress.Wavy, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
        // Point 1: track stays a smooth <circle>, only the active indicator is a wavy <path>.
        Assert.NotEmpty(cut.FindAll($"circle.{Css.Classes.Progress.Track}"));
        var ind = cut.FindAll($"path.{Css.Classes.Progress.Indicator}");
        Assert.NotEmpty(ind);
        Assert.Equal("100", ind[0].GetAttribute("pathLength"));
        // Point 4: the wavy indicator flows via the ring-wave CSS animation (rotate + dashoffset).
    }

    [Theory]
    [InlineData(0d)]
    [InlineData(25d)]
    [InlineData(60d)]
    [InlineData(100d)]
    public void Wavy_Circular_DashPatternPeriodIsTheWholePath(double value)
    {
        // The ring's flow rotates the path a full turn and walks stroke-dashoffset by 100 to cancel
        // it. That only lands back on itself if the dash pattern's period IS the path length, which
        // with pathLength=100 means the two values must sum to exactly 100. The original four-value
        // window - `0 lead len 100` - had period lead+len+100, so the sweep dragged the visible arc
        // onto the trailing gap: it changed length and position every frame and at times collapsed to
        // a fragment. A regression here would not throw or fail to render; it would just come apart
        // while animating, which no other assertion in this file would notice.
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, value).Add(x => x.Wavy, true));

        var dash = cut.Find($"path.{Css.Classes.Progress.Indicator}").GetAttribute("stroke-dasharray");
        var parts = dash!.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();

        Assert.Equal(2, parts.Length);
        Assert.Equal(100d, parts[0] + parts[1], 2);
        Assert.All(parts, v => Assert.True(v >= 0d, $"negative dash segment in '{dash}'"));
    }

    [Fact]
    public void Wavy_Circular_PublishesTheLeadingGapForTheKeyframe()
    {
        // The gap before the arc cannot live in the dash array any more (that would break the period
        // above), so it moves into stroke-dashoffset - and the keyframe has to be able to sweep from
        // it. The component hands it over as --_ring-lead; without it the arc starts flush against
        // the track with no break, which is the other half of the reported bug.
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 60d).Add(x => x.Wavy, true));

        var style = cut.Find($"path.{Css.Classes.Progress.Indicator}").GetAttribute("style") ?? "";
        Assert.Contains("--_ring-lead:", style);
        var lead = double.Parse(style.Split(':')[1].TrimEnd(';'), CultureInfo.InvariantCulture);
        Assert.True(lead < 0d, $"the lead offsets the pattern backwards, so it must be negative; got {lead}");
    }

    [Fact]
    public void Wavy_Indeterminate_FallsBackToFlat()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Wavy, true));
        Assert.DoesNotContain(Css.Classes.Progress.Wavy, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }
}
