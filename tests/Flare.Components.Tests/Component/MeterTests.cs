using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// FlareMeter: a segmented part-to-whole bar. It is a PROPORTIONAL zone host - parts are declared as
// FlareZone children carrying a Value weight, and sized in proportion to their sum via flex-grow.
public class C_FlareMeterTests : FlareTestContext
{
    private static RenderFragment TwoSegments => b =>
    {
        b.OpenComponent<FlareZone>(0);
        b.AddAttribute(1, nameof(FlareZone.Value), (double?)75.0);
        b.AddAttribute(2, nameof(FlareZone.Color), FlareColor.Error);
        b.AddAttribute(3, nameof(FlareZone.Label), "DB");
        b.CloseComponent();
        b.OpenComponent<FlareZone>(4);
        b.AddAttribute(5, nameof(FlareZone.Value), (double?)25.0);
        b.AddAttribute(6, nameof(FlareZone.Label), "Other");
        b.CloseComponent();
    };

    [Fact]
    public void RendersOneSegmentPerPositiveValue()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        Assert.Equal(2, cut.FindAll(".flare-meter__seg").Count);
    }

    [Fact]
    public void SegmentValue_DrivesFlexGrow()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        var segs = cut.FindAll(".flare-meter__seg");
        Assert.Contains("flex-grow:75", segs[0].GetAttribute("style"));
        Assert.Contains("flex-grow:25", segs[1].GetAttribute("style"));
    }

    [Fact]
    public void RoleColor_AddsColorClassToSegment()
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, TwoSegments));

        Assert.Contains("flare-color-error", cut.FindAll(".flare-meter__seg")[0].ClassName);
    }

    [Fact]
    public void NonPositiveSegments_AreIgnored_AndMeterIsEmpty()
    {
        RenderFragment zero = b =>
        {
            b.OpenComponent<FlareZone>(0);
            b.AddAttribute(1, nameof(FlareZone.Value), (double?)0.0);
            b.CloseComponent();
        };

        var cut = Render<FlareMeter>(p => p.Add(x => x.ChildContent, zero));

        Assert.Empty(cut.FindAll(".flare-meter__seg"));
        Assert.Contains("flare-meter--empty", cut.Find(".flare-meter").ClassName);
    }

    [Fact]
    public void ShowLegend_RendersOneEntryPerSegment()
    {
        var cut = Render<FlareMeter>(p => p
            .Add(x => x.ShowLegend, true)
            .Add(x => x.ChildContent, TwoSegments));

        Assert.Equal(2, cut.FindAll(".flare-meter__legend-item").Count);
        Assert.Contains("DB", cut.Find(".flare-meter__legend").TextContent);
    }
}

// FlareProgress is a SCALE zone host on the fixed 0-100 range - the same FlareZone child, read as an
// absolute [Start, End] band. Zones force a continuous track (no split gap / stop dot).
public class C_FlareProgressZonesTests : FlareTestContext
{
    private static RenderFragment DangerZone => b =>
    {
        b.OpenComponent<FlareZone>(0);
        b.AddAttribute(1, nameof(FlareZone.Start), (double?)90.0);
        b.AddAttribute(2, nameof(FlareZone.End), (double?)100.0);
        b.AddAttribute(3, nameof(FlareZone.Color), FlareColor.Error);
        b.CloseComponent();
    };

    [Fact]
    public void Zones_RenderBand_WithPercentsAndRoleColor()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, 40d)
            .Add(x => x.Zones, DangerZone));

        var band = cut.Find(".flare-progress__zone");
        Assert.Contains("--_z0:90.00%", band.GetAttribute("style"));
        Assert.Contains("--_z1:100.00%", band.GetAttribute("style"));
        Assert.Contains("flare-color-error", band.ClassName);
    }

    [Fact]
    public void Zones_SwitchTrackToContinuous_NotSplit()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, 40d)
            .Add(x => x.Zones, DangerZone));

        var root = cut.Find(".flare-progress--linear");
        Assert.Contains("flare-progress--with-zones", root.ClassName);
        Assert.DoesNotContain("flare-progress--split", root.ClassName);
        Assert.Empty(cut.FindAll(".flare-progress__remain"));
    }

    [Fact]
    public void NoZones_KeepsSplitTrack()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 40d));

        var root = cut.Find(".flare-progress--linear");
        Assert.Contains("flare-progress--split", root.ClassName);
        Assert.DoesNotContain("flare-progress--with-zones", root.ClassName);
        Assert.Empty(cut.FindAll(".flare-progress__zone"));
    }

    [Fact]
    public void ZeroWidthZone_IsDropped()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, 40d)
            .Add(x => x.Zones, b =>
            {
                b.OpenComponent<FlareZone>(0);
                b.AddAttribute(1, nameof(FlareZone.Start), (double?)50.0);
                b.AddAttribute(2, nameof(FlareZone.End), (double?)50.0);
                b.CloseComponent();
            }));

        Assert.Empty(cut.FindAll(".flare-progress__zone"));
    }
}
