using System.Globalization;
using Flare.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// <see cref="FlareGauge"/>: the projection from a value to the screen, and the contract that a band is a
/// <see cref="FlareZone"/> rather than a gauge-specific type.
/// </summary>
public class GaugeTests : FlareTestContext
{
    private static string Offset(IRenderedComponent<FlareGauge> cut) =>
        cut.Find(".flare-gauge__fill").GetAttribute("style") ?? string.Empty;

    [Fact]
    public void RendersArcByDefault()
    {
        var cut = Render<FlareGauge>(p => p.Add(x => x.Value, 50));

        Assert.Contains("flare-gauge--arc", cut.Find(".flare-gauge").ClassName ?? string.Empty);
        Assert.NotEmpty(cut.FindAll(".flare-gauge__track"));
    }

    // The fill is the whole track revealed by a dash, so the offset IS the percentage still to fill. That
    // is what makes a value change an animatable transition rather than a redrawn path.
    [Theory]
    [InlineData(0, "100.00")]
    [InlineData(25, "75.00")]
    [InlineData(100, "0.00")]
    public void FillOffsetIsTheRemainderOfTheScale(double value, string expected)
    {
        var cut = Render<FlareGauge>(p => p.Add(x => x.Value, value));

        Assert.Contains($"stroke-dashoffset:{expected}", Offset(cut));
    }

    [Fact]
    public void ValueOutsideTheScalePinsRatherThanEscapes()
    {
        var over = Render<FlareGauge>(p => p.Add(x => x.Value, 400));
        var under = Render<FlareGauge>(p => p.Add(x => x.Value, -400));

        Assert.Contains("stroke-dashoffset:0.00", Offset(over));
        Assert.Contains("stroke-dashoffset:100.00", Offset(under));
        Assert.Equal("100", over.Find(".flare-gauge").GetAttribute("aria-valuenow"));
        Assert.Equal("0", under.Find(".flare-gauge").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void ScaleIsRespectedRatherThanAssumedZeroToHundred()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Min, 200)
            .Add(x => x.Max, 400)
            .Add(x => x.Value, 250));

        Assert.Contains("stroke-dashoffset:75.00", Offset(cut));
    }

    // A radial gauge reads with a needle, an arc by filling - the idiom of each shape, and the reason
    // Needle is a nullable override rather than a plain bool.
    [Fact]
    public void RadialUsesANeedleAndArcUsesAFill()
    {
        var radial = Render<FlareGauge>(p => p.Add(x => x.Shape, GaugeShape.Radial).Add(x => x.Value, 50));
        var arc = Render<FlareGauge>(p => p.Add(x => x.Shape, GaugeShape.Arc).Add(x => x.Value, 50));

        Assert.NotEmpty(radial.FindAll(".flare-gauge__needle"));
        Assert.Empty(radial.FindAll(".flare-gauge__fill"));
        Assert.NotEmpty(arc.FindAll(".flare-gauge__fill"));
        Assert.Empty(arc.FindAll(".flare-gauge__needle"));
    }

    [Fact]
    public void NeedleRotationCarriesTheValue()
    {
        // The default radial sweep is 270 degrees, so half the scale is 135.
        var cut = Render<FlareGauge>(p => p.Add(x => x.Shape, GaugeShape.Radial).Add(x => x.Value, 50));

        var transform = cut.Find(".flare-gauge__needle-group").GetAttribute("transform");
        Assert.StartsWith("rotate(135.00", transform);
    }

    // The viewBox is computed from the sweep: a shape that does not use the whole circle must not be
    // padded out to a square, or the drawing is small and floating in empty space.
    [Fact]
    public void ViewBoxFitsTheSweep()
    {
        var arc = Render<FlareGauge>(p => p.Add(x => x.Shape, GaugeShape.Arc));
        var ring = Render<FlareGauge>(p => p
            .Add(x => x.Shape, GaugeShape.Arc)
            .Add(x => x.StartAngle, 0d)
            .Add(x => x.EndAngle, 360d));

        var arcBox = Box(arc);
        var ringBox = Box(ring);

        // A semicircle is wider than it is tall; a full turn is square.
        Assert.True(arcBox.H < arcBox.W, $"semicircle should be shorter than it is wide, got {arcBox.W}x{arcBox.H}");
        Assert.Equal(ringBox.W, ringBox.H, 1);
    }

    [Fact]
    public void FullTurnStillDrawsAnArc()
    {
        // One SVG arc cannot express a full turn - its endpoints coincide and nothing is drawn - so the
        // geometry splits it in two. The path must therefore carry two arc commands, not one.
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.StartAngle, 0d)
            .Add(x => x.EndAngle, 360d));

        var d = cut.Find(".flare-gauge__track").GetAttribute("d") ?? string.Empty;
        Assert.Equal(2, d.Split('A').Length - 1);
    }

    // Bands are FlareZone children: the same Start/End-on-a-host-scale primitive the slider and progress
    // bar take, rather than a gauge-only range type.
    [Fact]
    public void BandsComeFromZoneChildren()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Value, 50)
            .AddChildContent<FlareZone>(z => z
                .Add(x => x.Start, 80d)
                .Add(x => x.End, 100d)
                .Add(x => x.Color, FlareColor.Error)));

        Assert.Single(cut.FindAll(".flare-gauge__band"));
    }

    [Fact]
    public void ZoneWithAMissingBoundIsDropped()
    {
        var cut = Render<FlareGauge>(p => p
            .AddChildContent<FlareZone>(z => z.Add(x => x.Start, 80d)));

        Assert.Empty(cut.FindAll(".flare-gauge__band"));
    }

    // What a screen reader is told the reading MEANS: the band it lands in, when the application named one.
    [Fact]
    public void AccessibleValueTextNamesTheBandTheValueFallsIn()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Value, 90)
            .AddChildContent<FlareZone>(z => z
                .Add(x => x.Start, 80d)
                .Add(x => x.End, 100d)
                .Add(x => x.Label, "Critical")));

        Assert.Equal("90 (Critical)", cut.Find(".flare-gauge").GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void AccessibleValueTextIsJustTheNumberOutsideEveryBand()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Value, 20)
            .AddChildContent<FlareZone>(z => z
                .Add(x => x.Start, 80d)
                .Add(x => x.End, 100d)
                .Add(x => x.Label, "Critical")));

        Assert.Equal("20", cut.Find(".flare-gauge").GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void ReportsItselfAsAMeterWithTheScale()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Min, 10)
            .Add(x => x.Max, 90)
            .Add(x => x.Value, 40));

        var root = cut.Find(".flare-gauge");
        Assert.Equal("meter", root.GetAttribute("role"));
        Assert.Equal("10", root.GetAttribute("aria-valuemin"));
        Assert.Equal("90", root.GetAttribute("aria-valuemax"));
        Assert.Equal("40", root.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void TicksFollowTheIntervalNotACount()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Shape, GaugeShape.Linear)
            .Add(x => x.TickInterval, 25d));

        // 0, 25, 50, 75, 100.
        Assert.Equal(5, cut.FindAll(".flare-gauge__tick").Count);
    }

    [Fact]
    public void MinorTicksFillInBetweenTheMajorOnes()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Shape, GaugeShape.Linear)
            .Add(x => x.TickInterval, 50d)
            .Add(x => x.MinorTickInterval, 25d));

        Assert.Equal(5, cut.FindAll(".flare-gauge__tick").Count);
        // 25 and 75 are minor; 0, 50 and 100 are major.
        Assert.Equal(2, cut.FindAll(".flare-gauge__tick--minor").Count);
    }

    [Fact]
    public void TargetIsMarkedWithoutASecondPointer()
    {
        var cut = Render<FlareGauge>(p => p.Add(x => x.Value, 40).Add(x => x.Target, 70d));

        Assert.Single(cut.FindAll(".flare-gauge__target"));
        Assert.Empty(cut.FindAll(".flare-gauge__needle"));
    }

    [Fact]
    public void ReadoutFollowsTheFormat()
    {
        var cut = Render<FlareGauge>(p => p
            .Add(x => x.Value, 0.63)
            .Add(x => x.Max, 1)
            .Add(x => x.Format, "P0"));

        Assert.Equal(0.63.ToString("P0", CultureInfo.CurrentCulture), cut.Find(".flare-gauge__value").TextContent.Trim());
    }

    [Fact]
    public void AnimationClassIsOptOut()
    {
        var on = Render<FlareGauge>();
        var off = Render<FlareGauge>(p => p.Add(x => x.Animate, false));

        Assert.Contains("flare-gauge--animate", on.Find(".flare-gauge").ClassName ?? string.Empty);
        Assert.DoesNotContain("flare-gauge--animate", off.Find(".flare-gauge").ClassName ?? string.Empty);
    }

    private static (double W, double H) Box(IRenderedComponent<FlareGauge> cut)
    {
        var parts = (cut.Find(".flare-gauge svg").GetAttribute("viewBox") ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(v => double.Parse(v, CultureInfo.InvariantCulture))
            .ToArray();
        return (parts[2], parts[3]);
    }
}
