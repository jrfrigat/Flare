using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareZone on a slider: the ranged band read against the host's own Min..Max scale. This is the only place
// the slider's zone geometry is covered - the notch cut under the handle in particular - so these stay even
// though FlareProgress zones are exercised separately in MeterTests.
public class FlareSliderZonesTests : FlareTestContext
{
    private static RenderFragment Zones(RenderFragment body) => body;

    [Fact]
    public void Zones_RenderBands_WithRoleColorClassAndPercents()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Value, 40)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareZone>(0);
                b.AddAttribute(1, nameof(FlareZone.Start), 0d);
                b.AddAttribute(2, nameof(FlareZone.End), 60d);
                b.AddAttribute(3, nameof(FlareZone.Color), FlareColor.Success);
                b.CloseComponent();
                b.OpenComponent<FlareZone>(4);
                b.AddAttribute(5, nameof(FlareZone.Start), 60d);
                b.AddAttribute(6, nameof(FlareZone.End), 100d);
                b.AddAttribute(7, nameof(FlareZone.Color), FlareColor.Error);
                b.CloseComponent();
            })));

        var bands = cut.FindAll($".{Css.Classes.Slider.Zone}");
        // The handle sits at 40%, inside the first zone, so that zone is cut by the notch into two spans
        // (0-40 and 40-60) exactly like the rail underneath it - three spans in total with the second zone.
        Assert.Equal(3, bands.Count);
        Assert.Contains(bands, z => z.ClassName!.Contains(Css.Classes.Color.Success));
        Assert.Contains(bands, z => z.ClassName!.Contains(Css.Classes.Color.Error));
        // First span starts flush on the track start with the full track radius.
        Assert.Contains(bands, z => z.GetAttribute("style")!.Contains("left:0")
            && z.GetAttribute("style")!.Contains("right:calc(100% - 40.00% + var(--_gap))"));
    }

    // The bug this pins: a zone used to paint straight through the handle, filling the notch gap that the
    // rail leaves - so the slider lost its gap wherever a zone sat under the handle.
    [Fact]
    public void Zone_UnderTheHandle_IsCutByTheNotch_LikeTheRail()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Value, 40)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareZone>(0);
                b.AddAttribute(1, nameof(FlareZone.Start), (double?)0d);
                b.AddAttribute(2, nameof(FlareZone.End), (double?)100d);
                b.CloseComponent();
            })));

        // One declared zone spanning the whole scale -> two spans, split at the 40% handle.
        var bands = cut.FindAll($".{Css.Classes.Slider.Zone}");
        Assert.Equal(2, bands.Count);

        // Both sides of the notch are inset by the gap and take the gap radius, so the handle's gap shows
        // through the zone instead of being painted over.
        Assert.Contains("right:calc(100% - 40.00% + var(--_gap))", bands[0].GetAttribute("style")!);
        Assert.Contains("left:calc(40.00% + var(--_gap))", bands[1].GetAttribute("style")!);
        Assert.Contains("border-radius:var(--_trk-radius) var(--_gap-radius) var(--_gap-radius) var(--_trk-radius)", bands[0].GetAttribute("style")!);
        Assert.Contains("border-radius:var(--_gap-radius) var(--_trk-radius) var(--_trk-radius) var(--_gap-radius)", bands[1].GetAttribute("style")!);
    }

    // A zone is a band on the same rail as the active/inactive segments, so it must speak the same shape
    // language: outer ends keep the track radius, interior edges get the gap radius + notch inset. Without
    // this a zone is a raw rectangle that paints over the track's rounded ends (and adjacent zones butt
    // together with no separation).
    [Fact]
    public void Zone_TakesTrackRadiusOnOuterEnds_AndGapRadiusInside()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareZone>(0);
                b.AddAttribute(1, nameof(FlareZone.Start), (double?)0d);
                b.AddAttribute(2, nameof(FlareZone.End), (double?)70d);
                b.CloseComponent();
                b.OpenComponent<FlareZone>(3);
                b.AddAttribute(4, nameof(FlareZone.Start), (double?)70d);
                b.AddAttribute(5, nameof(FlareZone.End), (double?)100d);
                b.CloseComponent();
            })));

        var bands = cut.FindAll($".{Css.Classes.Slider.Zone}");
        var first = bands[0].GetAttribute("style")!;
        var second = bands[1].GetAttribute("style")!;

        // [0,70]: outer start (track radius, flush), interior end (gap radius, inset).
        Assert.Contains("left:0", first);
        Assert.Contains("border-radius:var(--_trk-radius) var(--_gap-radius) var(--_gap-radius) var(--_trk-radius)", first);
        // [70,100]: interior start (inset by the gap -> a visible gap from the first zone), outer end.
        Assert.Contains("left:calc(70.00% + var(--_gap))", second);
        Assert.Contains("right:0", second);
        Assert.Contains("border-radius:var(--_gap-radius) var(--_trk-radius) var(--_trk-radius) var(--_gap-radius)", second);
    }

    [Fact]
    public void Zone_CustomColor_InlinesLocalToken()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareZone>(0);
                b.AddAttribute(1, nameof(FlareZone.Start), 10d);
                b.AddAttribute(2, nameof(FlareZone.End), 50d);
                b.AddAttribute(3, nameof(FlareZone.Color), FlareColor.Custom("#ff0000"));
                b.CloseComponent();
            })));

        var band = cut.Find($".{Css.Classes.Slider.Zone}");
        Assert.DoesNotContain("flare-color-", band.ClassName);
        Assert.Contains(Css.Tokens.LocalColor.Main, band.GetAttribute("style"));
    }

    [Fact]
    public void Zone_ZeroWidth_IsDropped()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareZone>(0);
                b.AddAttribute(1, nameof(FlareZone.Start), 50d);
                b.AddAttribute(2, nameof(FlareZone.End), 50d);
                b.CloseComponent();
            })));

        Assert.Empty(cut.FindAll($".{Css.Classes.Slider.Zone}"));
    }
}
