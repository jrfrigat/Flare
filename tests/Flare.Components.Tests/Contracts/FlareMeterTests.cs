using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareMeter: a part-to-whole bar. It owns no scale - the parts define the whole - so parts are declared as
// FlareMeterSegment children carrying a Value weight, sized in proportion to their sum via flex-grow.
public class FlareMeterTests : FlareTestContext
{
    private static RenderFragment TwoSegments => b =>
    {
        b.OpenComponent<FlareMeterSegment>(0);
        b.AddAttribute(1, nameof(FlareMeterSegment.Value), (double?)75.0);
        b.AddAttribute(2, nameof(FlareMeterSegment.Color), FlareColor.Error);
        b.AddAttribute(3, nameof(FlareMeterSegment.Label), "DB");
        b.CloseComponent();
        b.OpenComponent<FlareMeterSegment>(4);
        b.AddAttribute(5, nameof(FlareMeterSegment.Value), (double?)25.0);
        b.AddAttribute(6, nameof(FlareMeterSegment.Label), "Other");
        b.CloseComponent();
    };

    [Fact]
    public void RendersOneSegmentPerPositiveValue()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Meter.Seg}").Count);
    }

    [Fact]
    public void SegmentValue_DrivesFlexGrow()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        // 75 / 25 of a 100 total -> already normalized.
        var segs = cut.FindAll($".{Css.Classes.Meter.Seg}");
        Assert.Contains("flex-grow:75", segs[0].GetAttribute("style"));
        Assert.Contains("flex-grow:25", segs[1].GetAttribute("style"));
    }

    // Flex only distributes ALL the free space when the grow factors sum to >= 1 (CSS Flexbox 1, 7.1.1).
    // Raw sub-1 values (fractions of a millisecond) used to leave the track partly empty - in correct
    // proportions, which is why it went unnoticed. The factors must be scaled to a fixed total.
    [Fact]
    public void FractionalValuesSummingBelowOne_AreNormalizedSoTheTrackStillFills()
    {
        // The exact reproduction from the field: a 0.3952 ms call filled only ~40% of the track.
        RenderFragment fractional = b =>
        {
            b.OpenComponent<FlareMeterSegment>(0);
            b.AddAttribute(1, nameof(FlareMeterSegment.Value), (double?)0.3155);
            b.CloseComponent();
            b.OpenComponent<FlareMeterSegment>(2);
            b.AddAttribute(3, nameof(FlareMeterSegment.Value), (double?)0.0797);
            b.CloseComponent();
        };

        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, fractional));

        var grows = cut.FindAll($".{Css.Classes.Meter.Seg}")
            .Select(s => double.Parse(
                Regex.Match(s.GetAttribute("style")!, @"flex-grow:([0-9.E+-]+)").Groups[1].Value,
                CultureInfo.InvariantCulture))
            .ToList();

        Assert.Equal(2, grows.Count);
        Assert.Equal(100d, grows.Sum(), 6);               // fills the whole track
        Assert.Equal(0.3155 / 0.0797, grows[0] / grows[1], 6); // and the proportion is untouched
    }

    [Fact]
    public void RoleColor_AddsColorClassToSegment()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        Assert.Contains(Css.Classes.Color.Error, cut.FindAll($".{Css.Classes.Meter.Seg}")[0].ClassName);
    }

    [Fact]
    public void NonPositiveSegments_AreIgnored_AndMeterIsEmpty()
    {
        RenderFragment zero = b =>
        {
            b.OpenComponent<FlareMeterSegment>(0);
            b.AddAttribute(1, nameof(FlareMeterSegment.Value), (double?)0.0);
            b.CloseComponent();
        };

        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, zero));

        Assert.Empty(cut.FindAll($".{Css.Classes.Meter.Seg}"));
        Assert.Contains(Css.Classes.Meter.Empty, cut.Find($".{Css.Classes.Meter.Root}").ClassName);
    }

    [Fact]
    public void ShowLegend_RendersOneEntryPerSegment()
    {
        var cut = Render<FlareMeter>(p => p
            .Add(x => x.ShowLegend, true)
            .Add(x => x.ChildContent, TwoSegments));

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Meter.LegendItem}").Count);
        Assert.Contains("DB", cut.Find($".{Css.Classes.Meter.Legend}").TextContent);
    }

    // The accessible label must carry the same information a sighted user gets: ShowValues gates the legend
    // and the tooltip, so it must gate the announcement too - a meter that hides its values must not read
    // them out.
    [Fact]
    public void AriaLabel_OmitsValues_WhenShowValuesIsOff()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        var label = cut.Find($".{Css.Classes.Meter.Root}").GetAttribute("aria-label")!;
        Assert.Equal("DB; Other", label);
    }

    [Fact]
    public void AriaLabel_IncludesValues_WhenShowValuesIsOn()
    {
        var cut = Render<FlareMeter>(p => p
            .Add(x => x.ShowValues, true)
            .Add(x => x.ChildContent, TwoSegments));

        var label = cut.Find($".{Css.Classes.Meter.Root}").GetAttribute("aria-label")!;
        Assert.Contains("DB 75", label);
        Assert.Contains("Other 25", label);
    }

    // "G" round-trips a double to full precision - right for storing a value, wrong for reading one out.
    [Fact]
    public void DefaultFormat_IsBounded_NotFullRoundTripPrecision()
    {
        RenderFragment noisy = b =>
        {
            b.OpenComponent<FlareMeterSegment>(0);
            b.AddAttribute(1, nameof(FlareMeterSegment.Value), (double?)0.0627);
            b.AddAttribute(2, nameof(FlareMeterSegment.Label), "Other");
            b.CloseComponent();
        };

        var cut = Render<FlareMeter>(p => p
            .Add(x => x.ShowValues, true)
            .Add(x => x.ChildContent, noisy));

        var label = cut.Find($".{Css.Classes.Meter.Root}").GetAttribute("aria-label")!;
        Assert.DoesNotContain("0.06269999999999998", label);
        Assert.DoesNotContain("0,06269999999999998", label);
    }

    // The two kinds are not interchangeable: a meter part carries a weight, a zone carries a range. Putting
    // the wrong one in must say so loudly rather than render an invisible band.
    [Fact]
    public void RangedZoneInsideMeter_ThrowsWithAClearMessage()
    {
        RenderFragment ranged = b =>
        {
            b.OpenComponent<FlareZone>(0);
            b.AddAttribute(1, nameof(FlareZone.Start), (double?)0.0);
            b.AddAttribute(2, nameof(FlareZone.End), (double?)70.0);
            b.CloseComponent();
        };

        var ex = Assert.Throws<InvalidOperationException>(
            () => Render<FlareMeter>(p => p.Add(x => x.ChildContent, ranged)));

        Assert.Contains("FlareZone", ex.Message);
        Assert.Contains("FlareMeter", ex.Message);
        Assert.Contains("FlareMeterSegment", ex.Message);
    }
}
