using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareProgress is a SCALE zone host on the fixed 0-100 range - the same FlareZone child, read as an
// absolute [Start, End] band. Zones force a continuous track (no split gap / stop dot).
public class FlareProgressZonesTests : FlareTestContext
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

        var band = cut.Find($".{Css.Classes.Progress.Zone}");
        Assert.Contains("--_z0:90.00%", band.GetAttribute("style"));
        Assert.Contains("--_z1:100.00%", band.GetAttribute("style"));
        Assert.Contains(Css.Classes.Color.Error, band.ClassName);
    }

    [Fact]
    public void Zones_SwitchTrackToContinuous_NotSplit()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, 40d)
            .Add(x => x.Zones, DangerZone));

        var root = cut.Find($".{Css.Classes.Progress.Linear}");
        Assert.Contains(Css.Classes.Progress.WithZones, root.ClassName);
        Assert.DoesNotContain(Css.Classes.Progress.Split, root.ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.Progress.Remain}"));
    }

    [Fact]
    public void NoZones_KeepsSplitTrack()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 40d));

        var root = cut.Find($".{Css.Classes.Progress.Linear}");
        Assert.Contains(Css.Classes.Progress.Split, root.ClassName);
        Assert.DoesNotContain(Css.Classes.Progress.WithZones, root.ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.Progress.Zone}"));
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

        Assert.Empty(cut.FindAll($".{Css.Classes.Progress.Zone}"));
    }

    // Mirror of the meter guard: a weighted meter part has no meaning on a host-owned scale.
    [Fact]
    public void MeterSegmentInsideProgress_ThrowsWithAClearMessage()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Render<FlareProgress>(p => p
            .Add(x => x.Value, 40d)
            .Add(x => x.Zones, b =>
            {
                b.OpenComponent<FlareMeterSegment>(0);
                b.AddAttribute(1, nameof(FlareMeterSegment.Value), (double?)5.0);
                b.CloseComponent();
            })));

        Assert.Contains("FlareMeterSegment", ex.Message);
        Assert.Contains("FlareProgress", ex.Message);
        Assert.Contains("FlareZone", ex.Message);
    }
}
