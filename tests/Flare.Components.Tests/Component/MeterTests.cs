using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// FlareMeter: a segmented part-to-whole bar. Segments are declared as FlareMeterSegment children and
// sized in proportion to their values via flex-grow.
public class C_FlareMeterTests : FlareTestContext
{
    private static RenderFragment TwoSegments => b =>
    {
        b.OpenComponent<FlareMeterSegment>(0);
        b.AddAttribute(1, nameof(FlareMeterSegment.Value), 75.0);
        b.AddAttribute(2, nameof(FlareMeterSegment.Color), FlareColor.Error);
        b.AddAttribute(3, nameof(FlareMeterSegment.Label), "DB");
        b.CloseComponent();
        b.OpenComponent<FlareMeterSegment>(4);
        b.AddAttribute(5, nameof(FlareMeterSegment.Value), 25.0);
        b.AddAttribute(6, nameof(FlareMeterSegment.Label), "Other");
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
            b.OpenComponent<FlareMeterSegment>(0);
            b.AddAttribute(1, nameof(FlareMeterSegment.Value), 0.0);
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

// FlareSlider buffered/secondary band (media "loaded so far").
public class C_FlareSliderBufferTests : FlareTestContext
{
    [Fact]
    public void BufferValue_RendersBufferBand()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.BufferValue, 60.0));

        var band = cut.Find(".flare-slider__zone--buffer");
        Assert.Contains("--_z1:60.00%", band.GetAttribute("style"));
    }

    [Fact]
    public void NoBufferValue_RendersNoBand()
    {
        var cut = Render<FlareSlider>();

        Assert.Empty(cut.FindAll(".flare-slider__zone--buffer"));
    }
}
