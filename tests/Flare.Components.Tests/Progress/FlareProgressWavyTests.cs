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
        [Css.Tokens.ProgressField.WavyHeight] = "10px",
        [Css.Tokens.ProgressField.WaveLength] = "40px",
        [Css.Tokens.ProgressField.IndeterminateWaveLength] = "20px",
        [Css.Tokens.ProgressField.WaveAmplitude] = "3px",
        [Css.Tokens.ProgressField.CircularGap] = "4px",
        [Css.Tokens.ProgressField.LinearHeight.Md] = "4px",
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
    [InlineData(20d, 1d)]
    [InlineData(40d, 3d)]
    [InlineData(80d, 4d)]
    public void LinearWaveTilePreservesTheThemePeriodAndAmplitude(double period, double amplitude)
    {
        var theme = new TokenThemeService(new Dictionary<string, string>
        {
            [Css.Tokens.ProgressField.WavyEnabled] = "1",
            [Css.Tokens.ProgressField.WaveLength] = period.ToString(CultureInfo.InvariantCulture) + "px",
            [Css.Tokens.ProgressField.WaveAmplitude] = amplitude.ToString(CultureInfo.InvariantCulture) + "px",
            [Css.Tokens.ProgressField.WavyHeight] = "12px",
        });
        var cut = Render<FlareProgress>(p => p.AddCascadingValue<IThemeService>(theme)
            .Add(x => x.Value, 70d).Add(x => x.Wavy, true));
        var svg = cut.Find("svg");
        var tile = cut.Find("pattern");
        Assert.Null(svg.GetAttribute("viewBox")); // the fill width must never rescale wave geometry
        Assert.Equal("userSpaceOnUse", tile.GetAttribute("patternUnits"));
        Assert.Equal(period, double.Parse(tile.GetAttribute("width")!, CultureInfo.InvariantCulture));
        Assert.Equal($"--_wave-period:{period.ToString(CultureInfo.InvariantCulture)}px;", svg.GetAttribute("style"));

        var curves = cut.Find("path").GetAttribute("d")!.Split('C').Skip(1)
            .Select(part => part.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => double.Parse(value, CultureInfo.InvariantCulture)).ToArray()).ToArray();
        // The central tile runs 0..period, with one crest and trough about its vertical midpoint.
        var tileEnds = curves.Where(curve => curve[4] > 0 && curve[4] <= period).ToArray();
        Assert.Equal(6 - amplitude, tileEnds.Min(curve => curve[5]), 2);
        Assert.Equal(6 + amplitude, tileEnds.Max(curve => curve[5]), 2);
        Assert.Equal(period, tileEnds[^1][4], 2);
        Assert.Equal(6, tileEnds[^1][5], 2);
    }

    [Fact]
    public void LinearWaveTilesDoNotCollideAndKeepTheirIdentityOnValueUpdates()
    {
        var first = Render<FlareProgress>(p => p.AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Wavy, true).Add(x => x.Value, 70d));
        var second = Render<FlareProgress>(p => p.AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Wavy, true).Add(x => x.Value, 25d));
        var id = first.Find("pattern").Id;
        var path = first.Find("path").GetAttribute("d");
        Assert.NotEqual(id, second.Find("pattern").Id);
        first.Render(p => p.Add(x => x.Value, 25d));
        Assert.Equal(id, first.Find("pattern").Id);
        Assert.Equal(path, first.Find("path").GetAttribute("d"));
        Assert.Equal($"url(#{id})", first.Find("rect").GetAttribute("fill"));
        Assert.Equal("100%", first.Find("rect").GetAttribute("width"));
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
    public void Wavy_IndeterminateLinear_RendersTwoMovingWaveSegments()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Wavy, true));

        var root = cut.Find($".{Css.Classes.Progress.Root}");
        Assert.Contains(Css.Classes.Progress.Wavy, root.ClassName);
        Assert.Contains(Css.Classes.Progress.Indeterminate, root.ClassName);
        Assert.Null(root.GetAttribute("aria-valuenow"));
        Assert.Single(cut.FindAll($".{Css.Classes.Progress.IndeterminateFirst}"));
        Assert.Single(cut.FindAll($".{Css.Classes.Progress.IndeterminateSecond}"));
        Assert.Equal(2, cut.FindAll($"svg.{Css.Classes.Progress.Wave}").Count);
        Assert.All(cut.FindAll("pattern"), pattern => Assert.Equal("20", pattern.GetAttribute("width")));
        Assert.Equal(2, cut.FindAll("pattern").Select(pattern => pattern.Id).Distinct().Count());
    }

    [Fact]
    public void Wavy_IndeterminateCircular_RendersWavyPathAtTheRequestedSizeStep()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Size, TrackSize.Md)
            .Add(x => x.Wavy, true));

        var root = cut.Find($".{Css.Classes.Progress.Root}");
        Assert.Contains(Css.Classes.Progress.Wavy, root.ClassName);
        Assert.Contains(Css.Classes.Progress.Indeterminate, root.ClassName);
        Assert.Contains(Css.Classes.Progress.Md, root.ClassName);
        Assert.Null(root.GetAttribute("aria-valuenow"));
        Assert.Single(cut.FindAll($"path.{Css.Classes.Progress.Indicator}"));
    }

    [Fact]
    public void Wavy_ThickLinear_UsesTheSizeStepStrokeAndWaveEnvelope()
    {
        var theme = new TokenThemeService(new Dictionary<string, string>
        {
            [Css.Tokens.ProgressField.WavyEnabled] = "1",
            [Css.Tokens.ProgressField.WavyHeight] = "10px",
            [Css.Tokens.ProgressField.WaveLength] = "40px",
            [Css.Tokens.ProgressField.WaveAmplitude] = "3px",
            [Css.Tokens.ProgressField.LinearHeight.Xl] = "8px",
        });
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(theme)
            .Add(x => x.Value, 60d)
            .Add(x => x.Size, TrackSize.Xl)
            .Add(x => x.Wavy, true));

        var style = cut.Find($".{Css.Classes.Progress.Root}").GetAttribute("style") ?? "";
        Assert.Contains("--_wave-height:14px", style);
        Assert.Contains("--_wave-stroke:8px", style);
    }
}
